using System;
using Jay.Xtend;
using System.Linq;
using Jay.VTS.Parser;
using Jay.Logging;
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

        public StackFrame(CodeBlock Root) {
            this.Root = Root;
            FindEntry(this.Root);
            this.Variables = new Dictionary<string, VTSVariable>();
        }

        public StackFrame(CodeBlock Root, int EntryIndex) {
            this.Root = Root;
            this.Index = EntryIndex;
            Pointer = Root.Contents[EntryIndex];
            this.Variables = new Dictionary<string, VTSVariable>();
        }

        public void Crash(FrameEventArgs eventArgs) => OnStackFrameReturns(eventArgs);

        public void AddHandler(EventHandler<FrameEventArgs> handler) => StackFrameReturns += handler;

        private void FindEntry(CodeBlock Target) {
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
        }

        public void Execute() {
            //Console.WriteLine("Currently at: " + (string)Pointer);
            Interpreter.Instance.PrintAll();
            PrintScope();
            BlockParse.ParseSingleBlock(this, Pointer);
            PrintScope();
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