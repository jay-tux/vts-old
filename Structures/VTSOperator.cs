using System;
using Jay.VTS;

namespace Jay.VTS.Structures
{
    public class VTSOperator
    {
        public string Name { get; private set; }
        public string ActionName { get; private set; }
        public string Operator { get; private set; }

        public static VTSOperator ADD = new VTSOperator() { Name = "Add", ActionName = "_add", Operator = "+" };
        public static VTSOperator SUBTRACT = new VTSOperator() { Name = "Subtract", ActionName = "_sub", Operator = "-" };
        public static VTSOperator MULTIPLY = new VTSOperator() { Name = "Multiply", ActionName = "_mul", Operator = "*" };
        public static VTSOperator DIVIDE = new VTSOperator() { Name = "Divide", ActionName = "_div", Operator = "/" };
        public static VTSOperator MODULUS = new VTSOperator() { Name = "Modulus", ActionName = "_mod", Operator = "%" };
        public static VTSOperator EQUALS = new VTSOperator() { Name = "Equal to", ActionName = "_eql", Operator = "==" };
        public static VTSOperator LARGER = new VTSOperator() { Name = "Larger than", ActionName = "_lrg", Operator = ">" };
        public static VTSOperator SMALLER = new VTSOperator() { Name = "Smaller than", ActionName = "_sml", Operator = "<" };

        private VTSOperator(){}
    }
}