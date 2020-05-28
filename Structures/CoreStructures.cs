using System;
using Jay.VTS.Execution;
using System.Collections.Generic;

namespace Jay.VTS.Structures
{
    public static class CoreStructures
    {
        #region builtin classes
        public static VTSClass OperatorClass = new VTSClass() {
            Name = "Operator", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>(), Operators = new Dictionary<VTSOperator, VTSAction>(),
            Internals = new Dictionary<string, Func<List<VTSParameter>, StackFrame, VTSVariable>>() {
                ["add"]   = ((args, frame) => { /**/ }),
                ["sub"]   = ((args, frame) => { /**/ }),
                ["mult"]  = ((args, frame) => { /**/ }),
                ["div"]   = ((args, frame) => { /**/ }),
                ["equal"] = ((args, frame) => { /**/ }),
                ["lrgr"]  = ((args, frame) => { /**/ }),
                ["smllr"] = ((args, frame) => { /**/ }),
                ["and"]   = ((args, frame) => { /**/ }),
                ["or"]    = ((args, frame) => { /**/ }),
                ["not"]   = ((args, frame) => { /**/ })
            }
        };
        public static VTSClass ControlClass = new VTSClass() {
            Name = "Control", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>(), Operators = new Dictionary<VTSOperator, VTSAction>(),
            Internals = new Dictionary<string, Func<List<VTSParameter>, StackFrame, VTSVariable>>() {
                ["if"]     = ((args, frame) => { /**/ }),
                ["else"]   = ((args, frame) => { /**/ }),
                ["elseif"] = ((args, frame) => { /**/ }),
                ["while"]  = ((args, frame) => { /**/ }),
                ["for"]    = ((args, frame) => { /**/ }),
                ["return"] = ((args, frame) => { /**/ })
            }
        };
        public static VTSClass CoreClass = new VTSClass() {
            Name = "Core", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>(){
                //
            }, Operators = new Dictionary<VTSOperator, VTSAction>(),
            Internals = new Dictionary<string, Func<List<VTSParameter>, StackFrame, VTSVariable>>() {
                ["stdout"] = ((args, frame) => {
                    if(args.Count != 1) {
                        throw new VTSException("ArgumentError", frame, "core.stdout requires exactly one argument, " 
                            + args.Count + " given.", null);
                    }
                    else {
                        Console.Write(args[0].Value);
                        return Void;
                    }
                }),
                ["stderr"] = ((args, frame) => {
                    if(args.Count != 1) {
                        throw new VTSException("ArgumentError", frame, "core.stderr requires exactly one argument, " 
                            + args.Count + " given.", null);
                    }
                    else {
                        Console.Error.Write(args[0].Value);
                        return Void;
                    }
                }),
                ["assign"] = ((args, frame) => { /**/ })
            }
        };

        public static VTSClass VoidClass = new VTSClass() {
            Name = "Void", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>(), Operators = new Dictionary<VTSOperator, VTSAction>(),
            Internals = new Dictionary<string, Func<List<VTSParameter>, StackFrame, VTSVariable>>()
        };
        #endregion

        #region builtin constants
        public static VTSVariable Void = new VTSVariable() {
            Class = VoidClass, Mutable = false, Fields = new Dictionary<string, object>()
        };

        public static VTSVariable Core = new VTSVariable() {
            Class = CoreClass, Mutable = false, Fields = new Dictionary<string, object>()
        };

        public static VTSVariable Control = new VTSVariable() {
            Class = ControlClass, Mutable = false, Fields = new Dictionary<string, string>()
        };

        public static VTSVariable Operator = new VTSVariable() {
            Class = OperatorClass, Mutable = false, Fields = new Dictionary<string, string>()
        };
        #endregion

        public static Dictionary<string, VTSVariable> BuiltinVariables = new Dictionary<string, VTSVariable>() {
            ["void"] = Void, ["core"] = Core, ["op"] = Operator, ["control"] = Control
        };

        public static List<VTSClass> BuiltinClasses = new List<VTSClass>() { 
            CoreClass, VoidClass, OperatorClass, ControlClass
        };
    }
}