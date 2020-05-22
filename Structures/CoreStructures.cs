using System;
using Jay.VTS.Execution;
using System.Collections.Generic;

namespace Jay.VTS.Structures
{
    public static class CoreStructures
    {
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
                        Console.Write(args[0]);
                        return Void;
                    }
                }),
                ["stderr"] = ((args, frame) => {
                    if(args.Count != 1) {
                        throw new VTSException("ArgumentError", frame, "core.stderr requires exactly one argument, " 
                            + args.Count + " given.", null);
                    }
                    else {
                        Console.Error.Write(args[0]);
                        return Void;
                    }
                })
            }
        };

        public static VTSClass VoidClass = new VTSClass() {
            Name = "Void", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>(), Operators = new Dictionary<VTSOperator, VTSAction>(),
            Internals = new Dictionary<string, Func<List<VTSParameter>, StackFrame, VTSVariable>>()
        };

        public static VTSVariable Void = new VTSVariable() {
            Class = VoidClass, Mutable = false, Fields = new Dictionary<string, object>()
        };

        public static VTSVariable Core = new VTSVariable() {
            Class = CoreClass, Mutable = false, Fields = new Dictionary<string, object>()
        };
    }
}