using System;
using System.Linq;
using Jay.VTS.Parser;

namespace Jay.VTS.Execution
{
    public class StackFrame
    {
        public StackFrame Parent;
        public CodeBlock Root;
        public CodeBlock Pointer;
        private int Index;
        public event EventHandler<FrameEventArgs> StackFrameReturns;

        public StackFrame(CodeBlock Root) {
            this.Root = Root;
            FindEntry(this.Root);
        }

        public StackFrame(CodeBlock Root, CodeBlock Entry) {
            this.Root = Root;
            Pointer = Entry;
        }

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
            Console.WriteLine("Currently at: " + (string)Pointer);
            Interpreter.Instance.PrintAll();
        }

        protected virtual void OnStackFrameReturns(FrameEventArgs e) {
            EventHandler<FrameEventArgs> handler= StackFrameReturns;
            if(handler != null) handler(this, e);
        }

        public static explicit operator string(StackFrame val) => val.Root.File;
        public static explicit operator int(StackFrame val) => val.Pointer.Lineno;
    }
}