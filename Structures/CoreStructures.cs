using System;
using Jay.VTS.Execution;
using System.Collections.Generic;

namespace Jay.VTS.Structures
{
    public static class CoreStructures
    {
        #region builtin classes
        #region Object Class
        public static VTSClass ObjectClass = new VTSClass() {
            Name = "Object", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>(), 
            Operators = new Dictionary<VTSOperator, VTSAction>(){
                //
            },
            Internals = new Dictionary<string, Func<List<VTSParameter>, StackFrame, VTSVariable>>() {
                ["tostring"] = ((args, frame) => { 
                    if(args.Count == 0) return new VTSVariable() {
                        
                    }; 
                    throw new VTSException("ArgumentError", frame, "object.tostring expects zero arguments.", null);
                }),
                ["hashcode"] = ((args, frame) => { 
                    if(args.Count == 0) return new VTSVariable() {}; 
                    throw new VTSException("ArgumentError", frame, "object.hashcode expects zero arguments.", null);
                })
            }
        };
        #endregion
        #region Core Class
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
                })
            }
        };
        #endregion
        #region Void Class
        public static VTSClass VoidClass = new VTSClass() {
            Name = "Void", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>(), Operators = new Dictionary<VTSOperator, VTSAction>(),
            Internals = new Dictionary<string, Func<List<VTSParameter>, StackFrame, VTSVariable>>()
        };
        #endregion
        #endregion

        #region builtin constants
        public static VTSVariable Void = new VTSVariable() {
            Class = VoidClass, Mutable = false, Fields = new Dictionary<string, object>()
        };

        public static VTSVariable Core = new VTSVariable() {
            Class = CoreClass, Mutable = false, Fields = new Dictionary<string, object>()
        };
        #endregion

        public static Dictionary<string, VTSVariable> BuiltinVariables = new Dictionary<string, VTSVariable>() {
            ["void"] = Void, ["core"] = Core
        };

        public static List<VTSClass> BuiltinClasses = new List<VTSClass>() { 
            CoreClass, VoidClass
        };
    }
    public static class Primitives 
    {
        public static VTSClass VTSInt = new VTSClass() {
            Name = "int", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>() { ["value"] = "int" }
        };
        public static VTSClass VTSString = new VTSClass() {
            Name = "string", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>() { ["value"] = "string" }
        };
        public static VTSClass VTSFloat = new VTSClass() {
            Name = "float", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>() { ["value"] = "float" }
        };
        public static VTSClass VTSList = new VTSClass() {
            Name = "list", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>() { ["value"] = "List<T>" }
        };
    }
}