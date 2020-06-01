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

        public void Execute() {
            try {
                //Console.WriteLine("Currently at: " + (string)Pointer);
                //Interpreter.Instance.PrintAll();
                //PrintScope();
                FrameEventArgs args = new FrameEventArgs() { ExitCode = FrameEventArgs.Exits.EOF };
                while(!Finished && Index < Root.Contents.Count) {
                    Pointer = Root.Contents[Index];
                    Logger.Log(" ------ Index: " + Index + " ------ ");
                    if(Pointer.Split.Inner.Count > 0) {
                        //Is nonempty
                        LineElement lead = Pointer.Split.Inner[0];
                        switch(lead.Type) {
                            case ElementType.Control:
                                Logger.Log("Encountered control call (either if, else or while)");
                                ControlCall(Pointer);
                                break;
                            case ElementType.Return:
                                //return; set expression value as return value and stop execution.
                                Logger.Log("StackFrame finished with return call.");
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
        }

        private void ControlCall(CodeBlock block) 
        {
            Logger.Log(" === Trying to resolve control call ===");
            RunExpression(BlockParse.ParseSingleBlock(this, block.Split.Inner[1]));
            if(TempValue.Class == CoreStructures.VTSBool && (bool)TempValue.Fields["value"] == true) {
                Logger.Log(" === CONTROL RESULTED IN TRUE ===");
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
                    Logger.Log("Is while loop; jumpback...");
                    Index--;
                }
            }
            else {
                Logger.Log(" === CONTROL RESULTED IN FALSE ===");
            }
        }

        private void RunExpression(Expression e) {
            if(e == null) return;
            Logger.Log(" == Running Expression " + e.ToString() + " ==");if(!e.IsBlock) throw new VTSException("ParseError", this, "Expression should be a block type.", null);
            Stack<VTSVariable> vars = new Stack<VTSVariable>();
            foreach(Expression sub in e.Block) {
                LineElement content = sub.Content;
                if(content.Type == ElementType.Return) { /*skip return*/ }
                else if(content.Type == ElementType.Identifier) {
                    Logger.Log("  -> Encountered Identifier; pushing");
                    string rfr = content.Content;
                    if(Interpreter.Instance.ContainsClass(rfr)) {
                        //Is class, push class
                        vars.Push(Interpreter.Instance.Classes[rfr].TypeRef);
                    }
                    else if(Variables.ContainsKey(rfr)) {
                        //Is scope variable, push var ref
                        vars.Push(Variables[rfr]);
                    }
                    else if(IsCopyFrame && Parent.Variables.ContainsKey(rfr)) {
                        //Is parent variable in loop/if-clause
                        vars.Push(Parent.Variables[rfr]);
                    }
                    else if(CoreStructures.BuiltinVariables.ContainsKey(rfr)) {
                        //Is global variable, push var ref
                        vars.Push(CoreStructures.BuiltinVariables[rfr]);
                    }
                    else {
                        //Is null reference
                        vars.Push(VTSVariable.UNDEFINED(rfr));
                        /*throw new VTSException("NameError", this, 
                            "The type or identifier " + rfr + " is not defined.", null);*/
                    }
                }
                else if(content.Type == ElementType.Literal) {
                    Logger.Log("  -> Encountered Literal; parsing and pushing");
                    //parse literal and push to stack
                    vars.Push(Literal.Parse(content.Content));
                }
                else if(content.Type == ElementType.Operator) {
                    Logger.Log("  -> Encountered Operator; popping operands (2)");
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
                        Logger.Log("");
                        Logger.Log("Operand 1: " + operand1.ToString(this) + "$" + 
                            (operand1.Class == null ? "(typeless)" : operand1.Class.Name) + "~'" + operand1.Name + "'");
                        Logger.Log("Operand 2: " + operand2.ToString(this) + "$" + 
                            (operand2.Class == null ? "(typeless)" : operand2.Class.Name) + "~'" + operand2.Name + "'");
                        if(operand1.Name == "this") {
                            //error: can't change this reference
                            throw new VTSException("ReferenceError", this, "Cannot assign to <this> because it's read-only.", null);
                        }
                        else if(IsCopyFrame && Parent.Variables.ContainsKey(operand1.Name)) {
                            //update in parent, in if or while
                            Parent.Variables[operand1.Name] = operand2;
                            operand2.Name = operand1.Name;
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
                        else if(Variables.ContainsKey(operand1.Name)) {
                            //update in frame
                            Variables[operand1.Name] = operand2;
                            if(operand2.Name == null || !Variables.ContainsKey(operand2.Name)) {
                                operand2.Name = operand1.Name;
                            }
                        }
                        else {
                            //add in frame
                            Variables[operand1.Name] = operand2;
                            if(operand2.Name == null || !Variables.ContainsKey(operand2.Name)) {
                                operand2.Name = operand1.Name;
                            }
                        }       
                        if(Variables.ContainsKey(operand1.Name)) {
                            Logger.Log("Getting result from local vars [" + 
                                string.Join(", ", Variables.Keys) + "]...");
                            Logger.Log("Result   : " + Variables[operand1.Name].ToString(this) + "$" + 
                                (Variables[operand1.Name].Class == null ? "(typeless)" : Variables[operand1.Name].Class.Name) + 
                                "~'" + Variables[operand1.Name].Name + "'");
                        }    
                        else if(Parent.Variables.ContainsKey(operand1.Name)){
                            Logger.Log("Getting result from parent's vars [" +
                                string.Join(", ", Parent.Variables.Keys) + "]...");
                            Logger.Log("Result   : " + Parent.Variables[operand1.Name].ToString(this) + "$" + 
                                (Parent.Variables[operand1.Name].Class == null ? "(typeless)" : Parent.Variables[operand1.Name].Class.Name) + 
                                "~'" + Parent.Variables[operand1.Name].Name + "'");
                        }
                        else {
                            Logger.Log("Issue: var " + operand1.Name + " neither in local nor parent's vars...");
                        }
                        Logger.Log("");
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
                    Logger.Log("  -> Encountered Member <" + content.Content + ">; popping values (" + sub.ArgCount + ") and caller");
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
                    Logger.Log("    -> Popped arguments, caller is " + caller.ToString());
                    Logger.Log("    -> Arguments are (in order): ");
                    if(args.Count == 0) Logger.Log("      -> (no args)");
                    else args.ForEach(x => Logger.Log("      -> " + x.ToString()));
                    //call method
                    VTSVariable result = caller.Call(content.Content, this, args);
                    if(result == null) Logger.Log("Somehow, we didn't get a result?");
                    Logger.Log("    -> Result is " + result.ToString());
                    //push result
                    vars.Push(result);
                }
                else {
                    throw new VTSException("ExpressionError", this, "Only Identifiers, Literals, Operators and Calls " +
                        "are allowed in an Expression.", null);
                }
                Logger.Log("  ----------- Current Stack: ----------");
                vars.ToArray().ForEach(x => {
                    if(x.Class == null) {
                        Logger.Log(" ^: Uninitialized variable: " + x.Name);
                    }
                    else if(x.IsTypeRef) {
                        Logger.Log(" ^: TypeRef$" + x.Class.Name);
                    }
                    else {
                        Logger.Log(" ^: " + x.Name + "#" + x.Class.Name + "::" + x.ToString(this));
                    }
                });
                Logger.Log(" ------------                ----------");
            }
            if(vars.Count == 1) {
                TempValue = vars.Pop();
            }
            else if(vars.Count > 1) {
                throw new VTSException("ExpressionError", this, "Unbalanced expression. <" + vars.Count + "> variables too much.", null);
            }
            Logger.Log(" == Finished running Expression ==");
        }

        public void PrintScope() {
            Logger.Log(" === Current Scope Variables === ");
            if(Variables == null) {
                Logger.Log("  -- Variables Dictionary is null --");
            }
            else if(CoreStructures.BuiltinVariables == null) {
                Logger.Log("  -- Core Variables Dictionary is null --");
            }
            else {
                Logger.Log("^: Local variables");
                Variables.ForEach(x => { 
                    /*if(x == null) { Logger.Log("(null/empty entry)"); }
                    else*/ if(x.Value == null) { Logger.Log("(null/empty variable)"); }
                    else if(x.Value.Class == null) { Logger.Log("(typeless variable)"); }
                    else if(x.Value.Class.Name == null) { Logger.Log("(unnamed class type)"); }
                    else { Logger.Log(" -> " + x.Key + ": " + x.Value.Class.Name); }
                });
                if(IsCopyFrame && Parent.Variables != null) {
                    Logger.Log("^: Inherited Copy variables");
                    Parent.Variables.ForEach(x => {
                        if(x.Value == null) { Logger.Log("(null/empty variable)"); }
                        else if(x.Value.Class == null) { Logger.Log("(typeless variable)"); }
                        else if(x.Value.Class.Name == null) { Logger.Log("(unnamed class type)"); }
                        else { Logger.Log(" -> " + x.Key + ": " + x.Value.Class.Name); }
                    });
                }
                Logger.Log("^: Global variables");
                CoreStructures.BuiltinVariables.ForEach(x => { 
                    if(x.Value == null) { Logger.Log(x.Key + ": (null/empty variable)"); }
                    else if(x.Value.Class == null) { Logger.Log(x.Key + ": (typeless variable)"); }
                    else if(x.Value.Class.Name == null) { Logger.Log(x.Key + ": (unnamed class type)"); }
                    else { Logger.Log(" -> " + x.Key + ": " + x.Value.Class.Name); }
                });
            }
            Logger.Log(" === End of Overview === ");
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