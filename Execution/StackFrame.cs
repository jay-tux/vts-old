using System;
using Jay.Xtend;
using System.Linq;
using Jay.VTS.Parser;
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

        public void AddHandler(EventHandler<FrameEventArgs> handler) => StackFrameReturns += handler;

        private void FindEntry(CodeBlock Target) {
            if(Target.Contents != null) {
                Target.Contents.ForEach(x => {
                    if(x.Type == "file" || x.Type == "root") {
                        FindEntry(x);
                    }
                    else if(x.Type == "entry") {
                        Pointer = x;
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
        }

        public void PrintScope() {
            Console.WriteLine(" === Current Scope Variables === ");
            Variables.ForEach(x => Console.WriteLine(" -> " + x.Key + ": " + x.Value.Class.Name));
            CoreStructures.BuiltinVariables.ForEach(x => Console.WriteLine(" -> " + x.Key + ": " + x.Value.Class.Name));
            Console.WriteLine(" === End of Overview === ");
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