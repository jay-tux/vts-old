using System;
using Jay.Logging;
using System.Collections.Generic;
using Jay.VTS;
using Jay.VTS.Parser;
using Jay.VTS.Execution;
using Jay.VTS.Enums;

namespace Jay.VTS.Structures
{
    public class VTSAction
    {
        public string Name;
        public CodeBlock Instructions;
        public StackFrame Parent;
        public event EventHandler<FrameEventArgs> ActionReturns;
        public VTSVariable Result;
        public List<string> ArgNames;
        public bool IsInternalCall;
        public Func<VTSVariable, VTSVariable, StackFrame, VTSVariable> InternalCall;

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

        public static explicit operator VTSAction(CodeBlock code) { 
            VTSAction action = new VTSAction() {
                Name = code.Split.Inner[1].Content,
                Instructions = code,
                ArgNames = new List<string>()
            };
            code.Split[2].Inner.ForEach(arg => {
                Logger.Log("   -> Encountered argument " + arg.ToOneliner());
                if(arg.Type == ElementType.Void) { Logger.Log("   -> Is Void. Ignoring."); }
                else if(arg.Type == ElementType.Separator) { Logger.Log("    -> Is Comma. Ignoring."); }
                else { 
                    Logger.Log("   -> Is real Argument. Adding.");
                    action.ArgNames.Add(arg.Content);
                }
            });
            Logger.Log("  -> Expecting " + action.ArgNames.Count + 
                " arguments: [ " + string.Join(", ", action.ArgNames) + " ].");
            return action;
        }
        
        public override string ToString() => "VTSAction::" + Name + " [" + ArgNames.Count + " args]";
    }
}