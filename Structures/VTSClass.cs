using System;
using System.Collections.Generic;
using Jay.VTS;
using Jay.Xtend;
using Jay.VTS.Parser;
using System.Linq;
using Jay.VTS.Execution;

namespace Jay.VTS.Structures
{
    public class VTSClass
    {
        public string Name;
        public Dictionary<string, string> Fields;
        public Dictionary<string, VTSAction> Actions;
        public Dictionary<VTSOperator, VTSAction> Operators;
        /*caller, args, stackframe => variable*/
        public Dictionary<string, Func<VTSVariable, List<VTSVariable>, StackFrame, VTSVariable>> Internals;

        public static explicit operator VTSClass(CodeBlock code) {
            return new VTSClass() { 
                Name = code.Split.Inner[1].Content, 
                Actions = new Dictionary<string, VTSAction>(),
                Fields = new Dictionary<string, string>(),
                Operators = new Dictionary<VTSOperator, VTSAction>()
            };
        }

        public VTSVariable Create(StackFrame frame, List<VTSVariable> args) {
            VTSVariable created = new VTSVariable() {
                Class = this,
                Mutable = true,
                Fields = new Dictionary<string, object>()
            };
            Fields.Keys.ForEach(field => created.Fields[field] = CoreStructures.Void);
            created.Call("new", frame, args);
            return created;
        }

        public bool Contains(string structure) => Fields.ContainsKey(structure) || Actions.ContainsKey(structure) 
            || Operators.Keys.ToList().Select(x => x.ActionName).Contains(structure);

        public void PrintActions() => Console.WriteLine("  [" + Actions.Count + "] Actions in <" + Name + ">: "
            + string.Join(", ", Actions.Values));

        public override string ToString() => "VTSClass::" + Name;
    }
}