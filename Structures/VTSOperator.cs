using System;
using Jay.VTS;
using System.Linq;
using Jay.VTS.Parser;

namespace Jay.VTS.Structures
{
    public class VTSOperator : IComparable
    {
        public string Name { get; private set; }
        public string ActionName { get; private set; }
        public string Operator { get; private set; }
        private int _prec;
        public static string[] _lgl = new string[]
            { "_add",   "_sub", "_mul", "_div", "_mod", "_eql", "_lrg", "_sml", "_asg", "_dis", "_con"/*, "_neg"*/ };
        public static string[] _ops = new string[]
            { "+",      "-",    "*",    "/",    "%",    "==",   ">",    "<",    "=",    "|",    "&"/*,    "!"*/ };

        public static VTSOperator ADD = new VTSOperator() {         Name = "Add",           ActionName = _lgl[ 0], Operator = _ops[ 0], _prec = 4 };
        public static VTSOperator SUBTRACT = new VTSOperator() {    Name = "Subtract",      ActionName = _lgl[ 1], Operator = _ops[ 1], _prec = 4 };
        public static VTSOperator MULTIPLY = new VTSOperator() {    Name = "Multiply",      ActionName = _lgl[ 2], Operator = _ops[ 2], _prec = 5 };
        public static VTSOperator DIVIDE = new VTSOperator() {      Name = "Divide",        ActionName = _lgl[ 3], Operator = _ops[ 3], _prec = 5 };
        public static VTSOperator MODULUS = new VTSOperator() {     Name = "Modulus",       ActionName = _lgl[ 4], Operator = _ops[ 4], _prec = 3 };
        public static VTSOperator EQUALS = new VTSOperator() {      Name = "Equal to",      ActionName = _lgl[ 5], Operator = _ops[ 5], _prec = 2 };
        public static VTSOperator LARGER = new VTSOperator() {      Name = "Larger than",   ActionName = _lgl[ 6], Operator = _ops[ 6], _prec = 2 };
        public static VTSOperator SMALLER = new VTSOperator() {     Name = "Smaller than",  ActionName = _lgl[ 7], Operator = _ops[ 7], _prec = 2 };
        public static VTSOperator ASSIGN = new VTSOperator() {      Name = "Assign",        ActionName = _lgl[ 8], Operator = _ops[ 8], _prec = 0 };
        public static VTSOperator OR = new VTSOperator() {          Name = "Or",            ActionName = _lgl[ 9], Operator = _ops[ 9], _prec = 1 };
        public static VTSOperator AND = new VTSOperator() {         Name = "And",           ActionName = _lgl[10], Operator = _ops[10], _prec = 1 };
        //public static VTSOperator NEGATION = new VTSOperator() {    Name = "Negation",      ActionName = _lgl[11], Operator = _ops[11], _prec = 1 };

        public static bool IsOperator(string name) => _lgl.Contains(name);

        public static explicit operator VTSOperator(string name) {
            if     (name == _lgl[ 0]) return ADD;
            else if(name == _lgl[ 1]) return SUBTRACT;
            else if(name == _lgl[ 2]) return MULTIPLY;
            else if(name == _lgl[ 3]) return DIVIDE;
            else if(name == _lgl[ 4]) return MODULUS;
            else if(name == _lgl[ 5]) return EQUALS;
            else if(name == _lgl[ 6]) return LARGER;
            else if(name == _lgl[ 7]) return SMALLER;
            else if(name == _lgl[ 8]) return ASSIGN;
            else if(name == _lgl[ 9]) return OR;
            else if(name == _lgl[10]) return AND;
            //else if(name == _lgl[11]) return NEGATION;
            else throw new VTSException("NameError", "---", "Operator with name " + name + " doesn't exist.");
        }

        public static explicit operator VTSOperator(LineElement elem) {
            string name = elem.Content;
            if     (name == _ops[ 0]) return ADD;
            else if(name == _ops[ 1]) return SUBTRACT;
            else if(name == _ops[ 2]) return MULTIPLY;
            else if(name == _ops[ 3]) return DIVIDE;
            else if(name == _ops[ 4]) return MODULUS;
            else if(name == _ops[ 5]) return EQUALS;
            else if(name == _ops[ 6]) return LARGER;
            else if(name == _ops[ 7]) return SMALLER;
            else if(name == _ops[ 8]) return ASSIGN;
            else if(name == _ops[ 9]) return OR;
            else if(name == _ops[10]) return AND;
            //else if(name == _ops[11]) return NEGATION;
            else throw new VTSException("SymbolError", "---", "Operator with symbol " + name + " doesn't exist.");
        }

        public int CompareTo(Object obj)
        {
            if(obj == null) return 1;
            VTSOperator other = obj as VTSOperator;
            if (other != null)
                return _prec.CompareTo(other._prec);
            else
                throw new ArgumentException("Other is not an operator.");
        }

        public static bool operator <(VTSOperator one, VTSOperator two) => one._prec < two._prec;
        public static bool operator >(VTSOperator one, VTSOperator two) => one._prec > two._prec;
        public static bool operator <=(VTSOperator one, VTSOperator two) => one._prec <= two._prec;
        public static bool operator >=(VTSOperator one, VTSOperator two) => one._prec >= two._prec;

        public override string ToString() => Operator + "[" + Name + "]";

        private VTSOperator(){}
    }
}