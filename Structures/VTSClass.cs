using System;
using System.Collections.Generic;
using Jay.VTS;

namespace Jay.VTS.Structures
{
    public class VTSClass
    {
        public string Name;
        public Dictionary<string, string> Fields;
        public Dictionary<string, VTSAction> Actions;
        public Dictionary<VTSOperator, VTSAction> Operators;
    }
}