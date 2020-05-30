using Jay.VTS.Structures;
using Jay.VTS.Execution;
using System.Collections.Generic;

namespace Jay.VTS.Structures
{
    public class VTSVariable
    {
        public VTSClass Class;
        public bool Mutable;
        public Dictionary<string, object> Fields;

        public override string ToString() => "~>" + Class.Name;

        public string ToString(StackFrame caller) => Class.Actions.ContainsKey("toString") ? 
                Call("toString", caller, new List<VTSVariable>()).Fields["value"].ToString() : 
                ("~>" + Class.Name);

        public VTSVariable Call(string action, StackFrame frame, List<VTSVariable> args) 
        {
            if(Class.Actions.ContainsKey(action)) {
                //INTERNAL CALL IS OPERATOR ONLY
                /*if(Class.Actions[action].IsInternalCall) {
                    return Class.Actions[action].InternalCall(this, args, frame);
                }*/
                if(Class.Actions[action].ArgNames.Count != args.Count) {
                    throw new VTSException("ArgumentError", frame, "Action <" + Class.Name + "." + action + 
                        "> expects " + Class.Actions[action].ArgNames.Count + " arguments, " + args.Count + " given.",
                        null);
                }
                StackFrame sf = new StackFrame(Class.Actions[action].Instructions, 0){ 
                    Parent = frame,
                    Variables = new Dictionary<string, VTSVariable>(),
                    ParentClass = Class
                };
                sf.Variables["this"] = this;
                for(int i = 0; i < args.Count; i++) {
                    sf.Variables[Class.Actions[action].ArgNames[i]] = args[i];
                }
                VTSVariable result = null;
                sf.StackFrameReturns += (src, res) => {
                    switch(res.ExitCode) {
                        case FrameEventArgs.Exits.ReturnValue: 
                            result = res.ReturnValue; 
                            break;
                        case FrameEventArgs.Exits.Return: 
                            result = CoreStructures.Void;
                            break;
                        case FrameEventArgs.Exits.InternalException:
                        case FrameEventArgs.Exits.CodeException: 
                            frame.Crash(res);
                            break;
                    }
                };
                return result;
            }
            else if(Class.Internals.ContainsKey(action)) {
                return Class.Internals[action](this, args, frame);
            }
            else {
                throw new VTSException("NameError", frame, "Class " + Class.Name + 
                    " doesn't have a member method " + action, null);
            }
        }
    }
}