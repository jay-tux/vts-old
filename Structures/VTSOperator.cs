using System;
using Jay.VTS;
using System.Linq;

namespace Jay.VTS.Structures
{
    public class VTSOperator
    {
        public string Name { get; private set; }
        public string ActionName { get; private set; }
        public string Operator { get; private set; }
        private static string[] _lgl = new string[]{ "_add", "_sub", "_mul", "_div", "_mod", "_eql", "_lrg", "_sml" };

        public static VTSOperator ADD = new VTSOperator() { Name = "Add", ActionName = _lgl[0], Operator = "+" };
        public static VTSOperator SUBTRACT = new VTSOperator() { Name = "Subtract", ActionName = _lgl[1], Operator = "-" };
        public static VTSOperator MULTIPLY = new VTSOperator() { Name = "Multiply", ActionName = _lgl[2], Operator = "*" };
        public static VTSOperator DIVIDE = new VTSOperator() { Name = "Divide", ActionName = _lgl[3], Operator = "/" };
        public static VTSOperator MODULUS = new VTSOperator() { Name = "Modulus", ActionName = _lgl[4], Operator = "%" };
        public static VTSOperator EQUALS = new VTSOperator() { Name = "Equal to", ActionName = _lgl[5], Operator = "==" };
        public static VTSOperator LARGER = new VTSOperator() { Name = "Larger than", ActionName = _lgl[6], Operator = ">" };
        public static VTSOperator SMALLER = new VTSOperator() { Name = "Smaller than", ActionName = _lgl[7], Operator = "<" };

        public static bool IsOperator(string name) => _lgl.Contains(name);

        public static explicit operator VTSOperator(string name) {
            if(name== _lgl[0]) return ADD;
            else if(name== _lgl[1]) return SUBTRACT;
            else if(name== _lgl[2]) return MULTIPLY;
            else if(name== _lgl[3]) return DIVIDE;
            else if(name== _lgl[4]) return MODULUS;
            else if(name== _lgl[5]) return EQUALS;
            else if(name== _lgl[6]) return LARGER;
            else if(name== _lgl[7]) return SMALLER;
            else throw new VTSException("NameError", "---", "Operator with name " + name + " doesn't exist.");
        }

        public override string ToString() => Operator + "[" + Name + "]";

        private VTSOperator(){}
    }
}