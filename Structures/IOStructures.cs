using System.Collections.Generic;
using Jay.VTS.Execution;
using Jay.VTS.Enums;
using System.IO;

namespace Jay.VTS.Structures
{
    public class IOStructures
    {
        public static void AppendIOModule() {
            Interpreter.Instance.Classes["File"] = new VTSClass() {
                Name = "File", Fields = new Dictionary<string, string>(),
                Actions = new Dictionary<string, VTSAction>(),
                Operators = new Dictionary<VTSOperator, VTSAction>(),
                Internals = new Dictionary<string, System.Func<VTSVariable, List<VTSVariable>, StackFrame, VTSVariable>>()
            };
        }
    }
}