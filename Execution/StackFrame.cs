using System;
using Jay.Xtend;
using System.Linq;
using Jay.VTS.Parser;
using Jay.Logging;
using Jay.VTS.Enums;
using Jay.VTS.Structures;
using System.Collections.Generic;

namespace Jay.VTS.Execution
{
    public class StackFrame
    {
        public StackFrame Parent;
        public CodeBlock Root;
        public CodeBlock Pointer;
        private int Index;
        public event EventHandler<FrameEventArgs> StackFrameReturns;
        public Dictionary<string, VTSVariable> Variables;
        public VTSClass ParentClass;
        protected VTSVariable TempValue;
        public bool IsCopyFrame;
        private bool Finished = false;

        public static void Overview() => Interpreter.Instance.PrintAll();

        public static StackFrame FindEntry(CodeBlock master) 
        {
            if(master.Type == "root") Overview();
            //search entry (recursively) in file/root blocks
            if(master.Type == "root" || master.Type == "file") {
                bool found = false;
                StackFrame result = null;
                for(int i = 0; i < master.Contents.Count && !found; i++) {
                    result = FindEntry(master.Contents[i]);
                    if(result != null) {
                        found = true;
                    }
                }
                return result;
            }
            else if(master.Type == "entry") {
                //create stackframe
                StackFrame rootFrame = new StackFrame(master, 0);
                return rootFrame;
            }
            return null;
        }

        public StackFrame(CodeBlock Root, int EntryIndex) {
            this.IsCopyFrame = false;
            this.Root = Root;
            this.Index = EntryIndex;
            //Pointer = Root.Contents[EntryIndex];
            this.Variables = new Dictionary<string, VTSVariable>();
            TempValue = CoreStructures.Void;
        }

        public void Crash(FrameEventArgs eventArgs) => OnStackFrameReturns(eventArgs);

        public void AddHandler(EventHandler<FrameEventArgs> handler) => StackFrameReturns += handler;

        public bool HasVar(string name) => 
            Variables.ContainsKey(name) || CoreStructures.BuiltinVariables.ContainsKey(name) || (IsCopyFrame && Parent.HasVar(name));
        
        public VTSVariable GetVariable(string name) =>
            Variables.ContainsKey(name) ? Variables[name] : 
                CoreStructures.BuiltinVariables.ContainsKey(name) ? CoreStructures.BuiltinVariables[name] :
                IsCopyFrame ? Parent.GetVariable(name) : CoreStructures.Void;

        public void SetVariable(string name, VTSVariable newValue) {
            if(IsCopyFrame && Parent.HasVar(name)) {
                Parent.SetVariable(name, newValue);
            }
            else {
                Variables[name] = newValue;
            }
        }

        public void Execute() {
            try {
                //Console.WriteLine("Currently at: " + (string)Pointer);
                //Interpreter.Instance.PrintAll();
                //PrintScope();
                FrameEventArgs args = new FrameEventArgs() { ExitCode = FrameEventArgs.Exits.EOF };
                while(!Finished && Index < Root.Contents.Count) {
                    Pointer = Root.Contents[Index];
                    Logger.Log(" ------ Index: " + Index + " ------ ", LogType.EXECUTION);
                    if(Pointer.Split.Inner.Count > 0) {
                        //Is nonempty
                        LineElement lead = Pointer.Split.Inner[0];
                        switch(lead.Type) {
                            case ElementType.Control:
                                Logger.Log("Encountered control call (either if, else or while)", LogType.MESSAGE);
                                ControlCall(Pointer);
                                break;
                            case ElementType.Return:
                                //return; set expression value as return value and stop execution.
                                Logger.Log("StackFrame finished with return call.", LogType.MESSAGE);
                                Finished = true;
                                RunExpression(BlockParse.ParseSingleBlock(this, Pointer));
                                args.ExitCode = FrameEventArgs.Exits.ReturnValue;
                                args.ReturnValue = TempValue;
                                break;
                            default:
                                //other kinds; evaluate expression
                                RunExpression(BlockParse.ParseSingleBlock(this, Pointer));
                                break;
                        }
                    }
                    //else: empty expression; move on
                    Index++;
                }
                //RunExpression(BlockParse.ParseSingleBlock(this, Pointer));
                PrintScope();
                OnStackFrameReturns(args);
            }
            catch(VTSException vtse) {
                Crash(new FrameEventArgs() {
                    ExitCode = FrameEventArgs.Exits.CodeException,
                    Error = vtse
                });
            }
            #if !DEBUG
            catch(Exception e) {
                Crash(new FrameEventArgs() {
                    ExitCode = FrameEventArgs.Exits.InternalException,
                    InternalError = e.Message
                });
            }
            #endif
        }

        private void ControlCall(CodeBlock block) 
        {
            Logger.Log(" === Trying to resolve control call ===", LogType.EXECUTION);
            RunExpression(BlockParse.ParseSingleBlock(this, block.Split.Inner[1]));
            if(TempValue.Class == CoreStructures.VTSBool && (bool)TempValue.Fields["value"] == true) {
                Logger.Log(" === CONTROL RESULTED IN TRUE ===", LogType.MESSAGE);
                StackFrame copy = new StackFrame(block, 0) {
                    IsCopyFrame = true,
                    Parent = this
                };
                copy.StackFrameReturns += (src, args) => {
                    if(args.ExitCode == FrameEventArgs.Exits.CodeException ||
                        args.ExitCode == FrameEventArgs.Exits.InternalException) {
                        Crash(args);
                        Finished = true;
                    }
                    else if(args.ExitCode == FrameEventArgs.Exits.Return ||
                        args.ExitCode == FrameEventArgs.Exits.ReturnValue) {
                        Crash(args);
                        Finished = true;
                    }
                    else {
                    }
                };
                copy.Execute();
                if(!Finished && block.Split.Inner[0].Content == "while") {
                    Logger.Log("Is while loop; jumpback...", LogType.EXECUTION);
                    Index--;
                }
            }
            else {
                Logger.Log(" === CONTROL RESULTED IN FALSE ===", LogType.MESSAGE);
            }
        }

        private void RunExpression(Expression e) {
            if(e == null) return;
            Logger.Log(" == Running Expression " + e.ToString() + " ==", LogType.DEBUG);
            if(!e.IsBlock) throw new VTSException("ParseError", this, "Expression should be a block type.", null);
            Stack<VTSVariable> vars = new Stack<VTSVariable>();
            foreach(Expression sub in e.Block) {
                LineElement content = sub.Content;
                if(content.Type == ElementType.Return || content.Type == ElementType.None) { /*skip return*/ }
                else if(content.Type == ElementType.Identifier) {
                    Logger.Log("  -> Encountered Identifier; pushing", LogType.DEBUG);
                    string rfr = content.Content;
                    if(Interpreter.Instance.ContainsClass(rfr)) {
                        //Is class, push class
                        Logger.Log("            Is Class: pushing type reference.", LogType.DEBUG);
                        vars.Push(Interpreter.Instance.Classes[rfr].TypeRef);
                    }
                    else if(HasVar(rfr)) {
                        //Is scope variable, push var ref
                        Logger.Log("            Is Variable reference.", LogType.DEBUG);
                        vars.Push(GetVariable(rfr));
                    }
                    else {
                        //Is null reference
                        Logger.Log("            Is Nothing, pushing undefined var.", LogType.DEBUG);
                        vars.Push(VTSVariable.UNDEFINED(rfr));
                        /*throw new VTSException("NameError", this, 
                            "The type or identifier " + rfr + " is not defined.", null);*/
                    }
                }
                else if(content.Type == ElementType.Literal) {
                    Logger.Log("  -> Encountered Literal; parsing and pushing", LogType.DEBUG);
                    //parse literal and push to stack
                    vars.Push(Literal.Parse(content.Content));
                }
                else if(content.Type == ElementType.Operator) {
                    Logger.Log("  -> Encountered Operator; popping operands (2)", LogType.DEBUG);
                    //pop 2 vars from stack
                    if(vars.Count < 2) {
                        throw new VTSException("ArgumentError", this,
                            "Unable to pop all the required arguments to call <" + content.Content + ">", null);
                    }
                    VTSVariable operand2 = vars.Pop();
                    VTSVariable operand1 = vars.Pop();
                    //try to run operator
                    if((VTSOperator)content == VTSOperator.ASSIGN) {
                        //special case: assignment (can be null)
                        Logger.Log("", LogType.EXECUTION);
                        Logger.Log("Operand 1: " + operand1.ToString(this) + "$" + 
                            (operand1.Class == null ? "(typeless)" : operand1.Class.Name) + "~'" + operand1.Name + "'",
                            LogType.DEBUG);
                        Logger.Log("Operand 2: " + operand2.ToString(this) + "$" + 
                            (operand2.Class == null ? "(typeless)" : operand2.Class.Name) + "~'" + operand2.Name + "'",
                            LogType.DEBUG);
                        if(operand1.Name == "this") {
                            //error: can't change this reference
                            throw new VTSException("ReferenceError", this, "Cannot assign to <this> because it's read-only.", null);
                        }
                        else if(CoreStructures.BuiltinVariables.ContainsKey(operand1.Name)) {
                            //error: cannot change global constants
                            throw new VTSException("ReferenceError", this, "Cannot assign to global constant <" +
                                operand1.Name + "> because it's read-only", null);
                        }
                        //iffy/incorrect: operand1 contains name of field, not a ref to object.
                        /*else if(operand1.Class.Fields.ContainsKey(operand1.Name)) {
                            if(operand1.Mutable) {
                                //mutable: change field
                                UPDATEDNAME.Fields[operand1.Name] = operand2;
                            }
                            else {
                                //error: can't change immutable value
                                throw new VTSException("ReferenceError", this, "Cannot assign to immutable variable <" +
                                    UPDATEDNAME + ">", null);
                            }
                        }*/
                        else {
                            //add in frame
                            SetVariable(operand1.Name, operand2);
                            if(operand2.Name == null || !Variables.ContainsKey(operand2.Name)) {
                                operand2.Name = operand1.Name;
                            }
                        }       
                        VTSVariable result = GetVariable(operand1.Name);
                        Logger.Log("Result   : " + result.ToString(this) + "$" + 
                            (result.Class == null ? "(typeless)" : result.Class.Name) + 
                            "~'" + result.Name + "'", LogType.DEBUG);
                        Logger.Log("", LogType.DEBUG);
                    }
                    else {
                        //normal case, call
                        if(operand1.Class == null) {
                            throw VTSException.NullRef(operand1.Name, this);
                        }
                        else if(operand2.Class == null) {
                            throw VTSException.NullRef(operand2.Name, this);
                        }
                        VTSVariable result = operand1.Call(content, this, operand2);
                        //push on stack
                        vars.Push(result);
                    }
                    //push result
                }
                else if(content.Type == ElementType.Member) {
                    Logger.Log("  -> Encountered Member <" + content.Content + ">; popping values (" + sub.ArgCount + ") and caller",
                        LogType.DEBUG);
                    //Try popping args
                    List<VTSVariable> args = new List<VTSVariable>();
                    for(int i = 0; i < sub.ArgCount; i++) {
                        if(vars.Count == 0) {
                            //not enough args
                            throw new VTSException("ArgumentError", this, 
                                "Unable to pop all the required arguments to call <" + content.Content + ">", null);
                        }
                        VTSVariable popped = vars.Pop();
                        if(popped.Class == null) throw VTSException.NullRef(popped.Name, this);
                        args.Insert(0, popped);
                    }

                    //try popping caller
                    if(vars.Count == 0) {
                        //caller is undefined 
                        throw new VTSException("ArgumentError", this,
                            "Unable to pop all the required arguments to call <" + content.Content + ">", null);
                    }
                    VTSVariable caller = vars.Pop();
                    if(caller.Class == null) throw VTSException.NullRef(caller.Name, this);
                    Logger.Log("    -> Popped arguments, caller is " + caller.ToString(), LogType.DEBUG);
                    Logger.Log("    -> Arguments are (in order): ", LogType.DEBUG);
                    if(args.Count == 0) Logger.Log("      -> (no args)", LogType.DEBUG);
                    else args.ForEach(x => Logger.Log("      -> " + x.ToString(), LogType.DEBUG));
                    //call method
                    VTSVariable result = caller.Call(content.Content, this, args);
                    if(result == null) Logger.Log("Somehow, we didn't get a result?", LogType.WARNING);
                    else Logger.Log("    -> Result is " + result.ToString(), LogType.DEBUG);
                    //push result
                    vars.Push(result);
                }
                else if(content.Type == ElementType.Field) {
                    Logger.Log("Encountered field; replacing previous value (owner) with field", LogType.DEBUG);
                    //encountered field
                    //get owner of field
                    if(vars.Count == 0) throw new VTSException("ArgumentError", this, "Unable to pop owner of field", null);
                    VTSVariable owner = vars.Pop();
                    //get field variable from owner
                    string fieldName = content.Content;
                    if(owner.Class.Fields.ContainsKey(fieldName)) {
                        VTSVariable replacement;
                        if(owner.Fields[fieldName] is VTSVariable) {
                            //field is real field
                            Logger.Log("  -> Real field", LogType.MESSAGE);
                            replacement = (VTSVariable)owner.Fields[fieldName];
                        }
                        else {
                            //field is value field from default type
                            Logger.Log("  -> Pseudo field", LogType.MESSAGE);
                            replacement = owner;
                        }
                        vars.Push(replacement);
                    }
                    else {
                        throw new VTSException("NameError", this, "Class " + owner.Class.Name + 
                            " doesn't have a field " + fieldName, null);
                    }
                }
                else {
                    throw new VTSException("ExpressionError", this, "Only Identifiers, Literals, Operators and Calls " +
                        "are allowed in an Expression.", null);
                }
                Logger.Log("  ----------- Current Stack: ----------", LogType.STRUTURAL);
                vars.ToArray().ForEach(x => {
                    if(x == null) {
                        Logger.Log(" ^: Empty stack entry.", LogType.WARNING);
                    }
                    else if(x.Class == null) {
                        Logger.Log(" ^: Uninitialized variable: " + x.Name, LogType.WARNING);
                    }
                    else if(x.IsTypeRef) {
                        Logger.Log(" ^: TypeRef$" + x.Class.Name, LogType.STRUTURAL);
                    }
                    else {
                        Logger.Log(" ^: " + x.Name + "#" + x.Class.Name + "::" + x.ToString(this), LogType.STRUTURAL);
                    }
                });
                Logger.Log(" ------------                ----------", LogType.STRUTURAL);
            }
            if(vars.Count == 1) {
                TempValue = vars.Pop();
            }
            else if(vars.Count > 1) {
                throw new VTSException("ExpressionError", this, "Unbalanced expression. <" + vars.Count + "> variables too much.", null);
            }
            Logger.Log(" == Finished running Expression ==", LogType.EXECUTION);
        }

        public void PrintScope() {
            Logger.Log(" === Current Scope Variables === ", LogType.STRUTURAL);
            if(Variables == null) {
                Logger.Log("  -- Variables Dictionary is null --", LogType.WARNING);
            }
            else if(CoreStructures.BuiltinVariables == null) {
                Logger.Log("  -- Core Variables Dictionary is null --", LogType.WARNING);
            }
            else {
                Logger.Log("^: Local variables", LogType.STRUTURAL);
                Variables.ForEach(x => { 
                    /*if(x == null) { Logger.Log("(null/empty entry)"); }
                    else*/ if(x.Value == null) { Logger.Log("(null/empty variable)", LogType.WARNING); }
                    else if(x.Value.Class == null) { Logger.Log("(typeless variable)", LogType.WARNING); }
                    else if(x.Value.Class.Name == null) { Logger.Log("(unnamed class type)", LogType.WARNING); }
                    else { Logger.Log(" -> " + x.Key + ": " + x.Value.Class.Name, LogType.STRUTURAL); }
                });
                if(IsCopyFrame && Parent.Variables != null) {
                    Logger.Log("^: Directly Inherited Copy variables", LogType.STRUTURAL);
                    Parent.Variables.ForEach(x => {
                        if(x.Value == null) { Logger.Log("(null/empty variable)", LogType.WARNING); }
                        else if(x.Value.Class == null) { Logger.Log("(typeless variable)", LogType.WARNING); }
                        else if(x.Value.Class.Name == null) { Logger.Log("(unnamed class type)", LogType.WARNING); }
                        else { Logger.Log(" -> " + x.Key + ": " + x.Value.Class.Name, LogType.STRUTURAL); }
                    });
                    Logger.Log("^: Indirectly Inherited Copy variables not shown.", LogType.STRUTURAL);
                }
                Logger.Log("^: Global variables", LogType.STRUTURAL);
                CoreStructures.BuiltinVariables.ForEach(x => { 
                    if(x.Value == null) { Logger.Log(x.Key + ": (null/empty variable)", LogType.WARNING); }
                    else if(x.Value.Class == null) { Logger.Log(x.Key + ": (typeless variable)", LogType.WARNING); }
                    else if(x.Value.Class.Name == null) { Logger.Log(x.Key + ": (unnamed class type)", LogType.WARNING); }
                    else { Logger.Log(" -> " + x.Key + ": " + x.Value.Class.Name, LogType.STRUTURAL); }
                });
            }
            Logger.Log(" === End of Overview === ", LogType.STRUTURAL);
        }

        protected virtual void OnStackFrameReturns(FrameEventArgs e) {
            EventHandler<FrameEventArgs> handler = StackFrameReturns;
            if(handler != null) handler(this, e);
        }

        public static explicit operator string(StackFrame val) => 
            val.Root == null ? "<root-of-code>" : val.Root.File;
        public static explicit operator int(StackFrame val) => 
            val.Pointer == null ? 0 : val.Pointer.Lineno;
    }
}