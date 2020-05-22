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
    }
}