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

        public override string ToString() 
        {
            if(Class.Actions.ContainsKey("toString")) {
                return "";
                //return Class.Actions["toString"]
            }
            else {
                return "~>" + Class.Name;
            }
        }

        /*public void Call(string action, StackFrame frame, List<VTSParameter> args) 
        {
            if(Class.Actions.ContainsKey(action)) {
                StackFrame sf = new StackFrame(Class.Actions[action], 0){ Parent = frame };
                for(int i = 0; i < args.Count; i++) {
                    //
                }
            }
            else {
                throw new VTSException("NameError", frame, "Class " + Class.Name + 
                    " doesn't have a member method " + action, null);
            }
        }*/
    }
}