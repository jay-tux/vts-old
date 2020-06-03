using Jay.VTS;
using System;
using System.Net;
using Jay.VTS.Execution;
using Jay.Xtend;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
                    }),
                    ["getString"] = ((caller, args, frame) => {
                        if(args.Count != 1)
                            throw VTSException.ArgCountException("WebClient", "getString", 1, (uint)args.Count, frame);
                        if(args[0].Class != CoreStructures.VTSString) {
                            throw VTSException.TypeException(CoreStructures.VTSString, args[0].Class, frame);
                        }

                        try {
                            string url = (string)((VTSVariable)caller.Fields["url"]).Fields["value"];
                            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);
                            Request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                            Request.Proxy = null;
                            Request.Method = (string)args[0].Fields["value"];
                            using (WebResponse Response = Request.GetResponse())
                            {
                                using (StreamReader Reader = new StreamReader(Response.GetResponseStream()))
                                {
                                    return new VTSVariable() {
                                        Class = CoreStructures.VTSString, Mutable = false,
                                        Fields = new Dictionary<string, object>() {
                                            ["value"] = Reader.ReadToEnd()
                                        }
                                    };
                                }
                            }
                        }
                        catch(Exception e) {
                            throw new VTSException("WebError", frame, 
                                "Failed downloading url " + (string)((VTSVariable)caller.Fields["url"]).Fields["value"] + 
                                " via " + (string)args[0].Fields["value"], null);
                        }
                    })
                }
            };
        }
    }
}