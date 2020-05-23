using System;
using System.Collections.Generic;
using Jay.VTS;
using Jay.VTS.Parser;
using Jay.VTS.Execution;

namespace Jay.VTS.Structures
{
    public class VTSAction
    {
        public string Name;
        public CodeBlock Instructions;
        public StackFrame Parent;
        public event EventHandler<FrameEventArgs> ActionReturns;
        public VTSVariable Result;

        public void Execute(StackFrame parent){
            Parent = parent;
            StackFrame execution = new StackFrame(Instructions, 0) { Parent = parent };
            execution.StackFrameReturns += (thrw, args) => OnActionReturns(args);
            execution.Execute();
        }

        public void Execute(StackFrame parent, VTSVariable caller) {
            Parent = parent;
            StackFrame execution = new StackFrame(Instructions, 0) { Parent = parent };
            execution.Variables["this"] = caller;
            execution.ParentClass = caller.Class;
            execution.StackFrameReturns += (thrw, args) => OnActionReturns(args);
            execution.Execute();
        }

        protected virtual void OnActionReturns(FrameEventArgs e) {
            Result = e.ReturnValue;
            EventHandler<FrameEventArgs> handler = ActionReturns;
            if(handler != null) handler(this, e);
        }

        public static explicit operator VTSAction(CodeBlock code) => 
            new VTSAction() { Name = code.Split.Inner[1].Content, Instructions = code };
        public override string ToString() => "VTSAction::" + Name;
    }
}