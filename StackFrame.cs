using System;

namespace Jay.VTS 
{
    public class StackFrame
    {
        public StackFrame Parent;
        public CodeBlock Root;
        public CodeBlock Pointer;

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
            Console.WriteLine("In StackFrame: " + (string)this + "; line: " + (int)this);
        }

        public static explicit operator string(StackFrame val) => val.Root.File;
        public static explicit operator int(StackFrame val) => val.Pointer.Lineno;
    }
}