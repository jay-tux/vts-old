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

        /*public StackFrame(CodeBlock Root) {
            this.Root = Root;
            Logger.Log("        ==== INITIALIZING DEFAULT STACKFRAME ====       ");
            Logger.Log((string)Root);
            Logger.Log("        ==== DEFAULT STACKFRAME INITIALIZED  ====       ");
            FindEntry(this.Root);
            this.Variables = new Dictionary<string, VTSVariable>();
        }*/

        public static StackFrame FindEntry(CodeBlock master) 
        {
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
            this.Root = Root;
            this.Index = EntryIndex;
            Pointer = Root.Contents[EntryIndex];
            this.Variables = new Dictionary<string, VTSVariable>();
        }

        public void Crash(FrameEventArgs eventArgs) => OnStackFrameReturns(eventArgs);

        public void AddHandler(EventHandler<FrameEventArgs> handler) => StackFrameReturns += handler;

        /*private void FindEntry(CodeBlock Target) {
            if(Target.Contents != null) {
                Target.Contents.ForEach(x => {
                    if(x.Type == "file" || x.Type == "root") {
                        FindEntry(x);
                    }
                    else if(x.Type == "entry") {
                        if(x.Contents == null || x.Contents.Count == 0) { 
                            OnStackFrameReturns(new FrameEventArgs().SetExitCode(FrameEventArgs.Exits.Return));
                        }
                        Pointer = x.Contents[0];
                        Index = 0;
                    }
                });
            }

            if(Target == Root && Pointer == null) {
                throw new VTSException("NoEntryError", this, "VTS-Entry is not set.", null);
            }
        }*/

        public void Execute() {
            try {
                Console.WriteLine("Currently at: " + (string)Pointer);
                Interpreter.Instance.PrintAll();
                PrintScope();
                bool finished = false;
                //while(!finished && index < )
                RunExpression(BlockParse.ParseSingleBlock(this, Pointer));
                PrintScope();
            }
            catch(VTSException vtse) {
                Crash(new FrameEventArgs() {
                    ExitCode = FrameEventArgs.Exits.CodeException,
                    Error = vtse
                });
            }
        }

        private void RunExpression(Expression e) {
            Logger.Log(" == Running Expression " + e.ToString() + " ==");
            if(e == null) return;
            if(!e.IsBlock) throw new VTSException("ParseError", this, "Expression should be a block type.", null);
            Stack<VTSVariable> vars = new Stack<VTSVariable>();
            foreach(Expression sub in e.Block) {
                LineElement content = sub.Content;
                if(content.Type == ElementType.Identifier) {
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
                        else if(Variables.ContainsKey(operand1.Name)) {
                            //update in frame
                            Variables[operand1.Name] = operand2;
                        }
                        else {
                            //add in frame
                            Variables[operand1.Name] = operand2;
                        }
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
                    Logger.Log("  -> Encountered Member; popping values (" + sub.ArgCount + ") and caller");
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
                    args.ForEach(x => Logger.Log("      -> " + x.ToString()));
                    //call method
                    VTSVariable result = caller.Call(content.Content, this, args);
                    Logger.Log("    -> Result is " + result.ToString());
                    //push result
                    vars.Push(result);
                }
                else {
                    throw new VTSException("ExpressionError", this, "Only Identifiers, Literals, Operators and Calls " +
                        "are allowed in an Expression.", null);
                }
                Logger.Log("  ----------- Current Stack: ----------");
                vars.ToArray().ForEach(x => Logger.Log(" ^: " + x.ToString()));
                Logger.Log(" ------------                ----------");
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
                Variables.ForEach(x => { 
                    /*if(x == null) { Logger.Log("(null/empty entry)"); }
                    else*/ if(x.Value == null) { Logger.Log("(null/empty variable)"); }
                    else if(x.Value.Class == null) { Logger.Log("(typeless variable)"); }
                    else if(x.Value.Class.Name == null) { Logger.Log("(unnamed class type)"); }
                    else { Logger.Log(" -> " + x.Key + ": " + x.Value.Class.Name); }
                });
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