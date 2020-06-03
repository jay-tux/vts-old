using Jay.VTS;
using System;
using System.Net;
using Jay.VTS.Execution;
using Jay.Xtend;
using System.Collections.Generic;
using System.Linq;

namespace Jay.VTS.Structures
{
    public class HTTPStructures
    {
        public static void AppendHTTPModule()
        {
            Interpreter.Instance.Classes["WebClient"] = new VTSClass() {  
                Name = "WebClient", Operators = new Dictionary<VTSOperator, VTSAction>(),
                Actions = new Dictionary<string, VTSAction>(),
                Fields = new Dictionary<string, string>() {
                    ["url"] = "string"
                }, 
                Internals = new Dictionary<string, Func<VTSVariable, List<VTSVariable>, StackFrame, VTSVariable>>() {
                    ["new"] = ((caller, args, frame) => {
                        if(args.Count != 1)
                            throw VTSException.ArgCountException("WebClient", "new", 1, (uint)args.Count, frame);
                        if(args[0].Class != CoreStructures.VTSString) 
                            throw VTSException.TypeException(CoreStructures.VTSString, args[0].Class, frame);
                        caller.Fields["url"] = args[0];
                        return caller;
                    })
                }
            };
        }
    }
}