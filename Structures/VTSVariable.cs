using System.Collections.Generic;

namespace Jay.VTS.Structures
{
    public class VTSVariable
    {
        public VTSClass Class;
        public bool Mutable;
        public Dictionary<string, object> Fields;
    }
}