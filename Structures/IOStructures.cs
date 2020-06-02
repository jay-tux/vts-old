using System.Collections.Generic;
using System;
using Jay.VTS.Execution;
using Jay.VTS.Enums;
using System.IO;
using System.Linq;
using Jay.VTS;
using System.Security;
using Jay.Logging;
using Jay.Xtend;

namespace Jay.VTS.Structures
{
    public class IOStructures
    {
        public static VTSException FileNotFoundException(string filePath, StackFrame frame)
            => new VTSException("IOError", frame, "File or Directory <" + filePath + "> not found.", null);
        public static VTSException PermissionsMissingException(string filePath, StackFrame frame)
            => new VTSException("IOError", frame, "Missing permissions on file <" + filePath + ">.", null);
        public static VTSException GenericFileException(string filePath, StackFrame frame) 
            => new VTSException("IOError", frame, "Failed to interact with file <" + filePath + ">.", null);

        public static void AppendIOModule() 
        {
            Interpreter.Instance.Classes["File"] = new VTSClass() {
                Name = "File", Fields = new Dictionary<string, string>() {
                    ["path"] = "string"
                },
                Actions = new Dictionary<string, VTSAction>(),
                Operators = new Dictionary<VTSOperator, VTSAction>(),
                Internals = new Dictionary<string, System.Func<VTSVariable, List<VTSVariable>, StackFrame, VTSVariable>>() {
                    ["new"] = ((caller, args, frame) => {
                        if(args.Count != 1) {
                            throw VTSException.ArgCountException("File", "new", 1, (uint)args.Count, frame);
                        }
                        if(args[0].Class != CoreStructures.VTSString) {
                            throw VTSException.TypeException(CoreStructures.VTSString, args[0].Class, frame);
                        }
                        try {
                            if(!File.Exists((string)args[0].Fields["value"])) {
                                File.Create((string)(args[0].Fields["value"])).Close();
                                //throw FileNotFoundException((string)args[0].Fields["value"], frame);
                            }
                        }
                        catch(VTSException v) { throw v; }
                        catch(Exception _) {
                            throw GenericFileException((string)args[0].Fields["value"], frame);
                        }
                        caller.Fields["path"] = new VTSVariable() {
                            Class = CoreStructures.VTSString, Mutable = false, Fields = new Dictionary<string, object>() {
                                ["value"] = args[0].Fields["value"]
                            }
                        };
                        return caller;
                    }),
                    ["getText"] = ((caller, args, frame) => {
                        if(args.Count != 0) {
                            throw VTSException.ArgCountException("File", "getText", 0, (uint)args.Count, frame);
                        }
                        try {
                            VTSVariable path = ((VTSVariable)caller.Fields["path"]);
                            return new VTSVariable() {
                                Class = CoreStructures.VTSString, Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = File.ReadAllText((string)path.Fields["value"])
                                }
                            };
                        }
                        catch(Exception e) when (e is SecurityException || e is UnauthorizedAccessException) {
                            throw PermissionsMissingException((string)
                                ((VTSVariable)caller.Fields["path"]).Fields["value"], frame);
                        }
                    }),
                    ["getLines"] = ((caller, args, frame) => {
                        if(args.Count != 0)
                            throw VTSException.ArgCountException("File", "getLines", 0, (uint)args.Count, frame);
                        try {
                            VTSVariable path = ((VTSVariable)caller.Fields["path"]);
                            List<VTSVariable> result = new List<VTSVariable>();
                            File.ReadAllLines((string)path.Fields["value"]).ForEach(x => result.Add(new VTSVariable() {
                                Class = CoreStructures.VTSString, Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = x
                                }
                            }));
                            return new VTSVariable() {
                                Class = CoreStructures.ListClass, Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = result, ["count"] = new VTSVariable() {
                                        Class = CoreStructures.VTSInt, Mutable = false,
                                        Fields = new Dictionary<string, object>() {
                                            ["value"] = result.Count
                                        }
                                    }
                                }
                            };
                        }
                        catch(Exception e) when (e is SecurityException || e is UnauthorizedAccessException) {
                            throw PermissionsMissingException((string)
                                ((VTSVariable)caller.Fields["path"]).Fields["value"], frame);
                        }
                    }),
                    ["writeText"] = ((caller, args, frame) => {
                        if(args.Count != 1) {
                            throw VTSException.ArgCountException("File", "writeText", 1, (uint)args.Count, frame);
                        }
                        try {
                            File.WriteAllText((string)((VTSVariable)caller.Fields["path"]).Fields["value"],
                                ((VTSVariable)args[0]).ToString(frame));
                            return CoreStructures.Void;
                        }
                        catch(Exception e) when (e is SecurityException || e is UnauthorizedAccessException) {
                            throw PermissionsMissingException(
                                (string)((VTSVariable)caller.Fields["path"]).Fields["value"],
                                frame
                            );
                        }
                    }),
                    ["writeLines"] = ((caller, args, frame) => {
                        if(args.Count != 1) {
                            throw VTSException.ArgCountException("File", "writeText", 1, (uint)args.Count, frame);
                        }
                        if(args[0].Class != CoreStructures.ListClass) {
                            throw VTSException.TypeException(CoreStructures.ListClass, args[0].Class, frame);
                        }
                        try {
                            string filePath = (string)((VTSVariable)caller.Fields["path"]).Fields["value"];
                            List<string> toWrite = ((List<VTSVariable>)args[0].Fields["value"])
                                .Select(x => {
                                    if(x.Class != CoreStructures.VTSString) {
                                        throw VTSException.TypeException(CoreStructures.VTSString, x.Class, frame);
                                    }
                                    return (string)x.Fields["value"];
                                }).ToList();
                            File.WriteAllLines(filePath, toWrite);
                            return CoreStructures.Void;
                        }
                        catch(Exception e) when (e is SecurityException || e is UnauthorizedAccessException) {
                            throw PermissionsMissingException(
                                (string)((VTSVariable)caller.Fields["path"]).Fields["value"],
                                frame
                            );
                        }
                    }),
                    ["appendText"] = ((caller, args, frame) => {
                        if(args.Count != 1) {
                            throw VTSException.ArgCountException("File", "appendText", 1, (uint)args.Count, frame);
                        }
                        try {
                            File.AppendAllText((string)((VTSVariable)caller.Fields["path"]).Fields["value"],
                                ((VTSVariable)args[0]).ToString(frame));
                            return CoreStructures.Void;
                        }
                        catch(Exception e) when (e is SecurityException || e is UnauthorizedAccessException) {
                            throw PermissionsMissingException(
                                (string)((VTSVariable)caller.Fields["path"]).Fields["value"],
                                frame
                            );
                        }
                    }),
                    ["appendLines"] = ((caller, args, frame) => {
                        if(args.Count != 1) {
                            throw VTSException.ArgCountException("File", "appendText", 1, (uint)args.Count, frame);
                        }
                        if(args[0].Class != CoreStructures.ListClass) {
                            throw VTSException.TypeException(CoreStructures.ListClass, args[0].Class, frame);
                        }
                        try {
                            string filePath = (string)((VTSVariable)caller.Fields["path"]).Fields["value"];
                            List<string> toWrite = ((List<VTSVariable>)args[0].Fields["value"])
                                .Select(x => {
                                    if(x.Class != CoreStructures.VTSString) {
                                        throw VTSException.TypeException(CoreStructures.VTSString, x.Class, frame);
                                    }
                                    return (string)x.Fields["value"];
                                }).ToList();
                            File.AppendAllLines(filePath, toWrite);
                            return CoreStructures.Void;
                        }
                        catch(Exception e) when (e is SecurityException || e is UnauthorizedAccessException) {
                            throw PermissionsMissingException(
                                (string)((VTSVariable)caller.Fields["path"]).Fields["value"],
                                frame
                            );
                        }
                    })
                }
            };

            Interpreter.Instance.Classes["Directory"] = new VTSClass() {
                Name = "Directory", Fields = new Dictionary<string, string>() {
                    ["path"] = "string"
                },
                Actions = new Dictionary<string, VTSAction>(),
                Operators = new Dictionary<VTSOperator, VTSAction>(),
                Internals = new Dictionary<string, System.Func<VTSVariable, List<VTSVariable>, StackFrame, VTSVariable>>() {
                    ["new"] = ((caller, args, frame) => {
                        if(args.Count != 1) {
                            throw VTSException.ArgCountException("Directory", "new", 1, (uint)args.Count, frame);
                        }
                        if(args[0].Class != CoreStructures.VTSString) {
                            throw VTSException.TypeException(CoreStructures.VTSString, args[0].Class, frame);
                        }
                        try {
                            if(!Directory.Exists((string)args[0].Fields["value"])) {
                                Directory.CreateDirectory((string)(args[0].Fields["value"]));
                                //throw FileNotFoundException((string)args[0].Fields["value"], frame);
                            }
                            caller.Fields["path"] = args[0];
                        }
                        catch(VTSException v) { throw v; }
                        catch(Exception _) {
                            throw GenericFileException((string)args[0].Fields["value"], frame);
                        }
                        return caller;
                    }),
                    ["getFiles"] = ((caller, args, frame) => {
                        if(args.Count != 0) {
                            throw VTSException.ArgCountException("Directory", "getFiles", 1, (uint)args.Count, frame);
                        }
                        List<VTSVariable> result = new List<VTSVariable>();
                        try {
                            Directory.GetFiles((string)((VTSVariable)caller.Fields["path"]).Fields["value"])
                                .ForEach(x => result.Add(new VTSVariable() {
                                    Class = CoreStructures.VTSString, Mutable = false, Fields = new Dictionary<string, object>() {
                                        ["value"] = x
                                    }
                                }));
                            return new VTSVariable() {
                                Class = CoreStructures.ListClass, Mutable = false, Fields = new Dictionary<string, object>() {
                                    ["value"] = result,
                                    ["count"] = new VTSVariable() {
                                        Class = CoreStructures.VTSInt, Mutable = false, Fields = new Dictionary<string, object>() {
                                            ["value"] = result.Count
                                        }
                                    }
                                }
                            };
                        }
                        catch(Exception) {
                            throw GenericFileException((string)((VTSVariable)caller.Fields["path"]).Fields["value"], frame);
                        }
                    }),
                    ["getDirs"] = ((caller, args, frame) => {
                        if(args.Count != 0) {
                            throw VTSException.ArgCountException("Directory", "getDirs", 1, (uint)args.Count, frame);
                        }
                        List<VTSVariable> result = new List<VTSVariable>();
                        try {
                            Directory.GetDirectories((string)((VTSVariable)caller.Fields["path"]).Fields["value"])
                                .ForEach(x => result.Add(new VTSVariable() {
                                    Class = CoreStructures.VTSString, Mutable = false, Fields = new Dictionary<string, object>() {
                                        ["value"] = x
                                    }
                                }));
                            return new VTSVariable() {
                                Class = CoreStructures.ListClass, Mutable = false, Fields = new Dictionary<string, object>() {
                                    ["value"] = result,
                                    ["count"] = new VTSVariable() {
                                        Class = CoreStructures.VTSInt, Mutable = false, Fields = new Dictionary<string, object>() {
                                            ["value"] = result.Count
                                        }
                                    }
                                }
                            };
                        }
                        catch(Exception) {
                            throw GenericFileException((string)((VTSVariable)caller.Fields["path"]).Fields["value"], frame);
                        }
                    }),
                    ["getCwd"] = ((caller, args, frame) => {
                        if(args.Count != 0) {
                            throw VTSException.ArgCountException("Directory", "getCwd", 0, (uint)args.Count, frame);
                        }
                        return new VTSVariable() {
                            Class = CoreStructures.VTSString, Mutable = false,
                            Fields = new Dictionary<string, object>() {
                                ["value"] = Directory.GetCurrentDirectory()
                            }
                        };
                    })
                }
            };
        }
    }
}