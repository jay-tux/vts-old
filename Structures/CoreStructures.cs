using System;
using Jay.Xtend;
using Jay.VTS.Execution;
using Jay.VTS.Structures;
using System.Collections.Generic;

namespace Jay.VTS.Structures
{
    public static class CoreStructures
    {
        #region builtin classes
        public static VTSClass CoreClass = new VTSClass() {
            Name = "Core", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>(){
                //
            }, Operators = new Dictionary<VTSOperator, VTSAction>(),
            Internals = new Dictionary<string, Func<VTSVariable, List<VTSVariable>, StackFrame, VTSVariable>>() {
                ["stdout"] = ((caller, args, frame) => {
                    if(args.Count != 1) {
                        throw VTSException.ArgCountException("core", "stdout", 1, (uint)args.Count, frame);
                    }
                    else {
                        Console.Write(args[0].ToString(frame));
                        return Void;
                    }
                }),
                ["stderr"] = ((caller, args, frame) => {
                    if(args.Count != 1) {
                        throw VTSException.ArgCountException("core", "stderr", 1, (uint)args.Count, frame);
                    }
                    else {
                        Console.Error.Write(args[0].ToString(frame));
                        return Void;
                    }
                }),
                ["toString"] = ((caller, args, frame) => {
                    if(args.Count != 0) 
                        throw VTSException.ArgCountException("core", "toString", 0, (uint)args.Count, frame);
                    else return new VTSVariable() { 
                        Class = VTSString, Mutable = false, 
                        Fields = new Dictionary<string, object>() { ["value"] = "core" }
                    };
                }),
                ["vardump"] = ((caller, args, frame) => {
                    if(args.Count != 0) 
                        throw VTSException.ArgCountException("core", "vardump", 0, (uint)args.Count, frame);
                    Console.WriteLine("\n ==== Global Variables: ====");
                    BuiltinVariables.Keys.ForEach(x => Console.WriteLine(x + "-> " + BuiltinVariables[x].ToString(frame)));
                    Console.WriteLine(" ==== Scope  Variables: ====");
                    frame.Variables.Keys.ForEach(x => Console.WriteLine(x + "-> " + frame.Variables[x].ToString(frame)));
                    if(frame.IsCopyFrame) {
                        Console.WriteLine(" ==== Super  Variables: ====");
                        frame.Parent.Variables.Keys.ForEach(x =>
                            Console.WriteLine(x + "-> " + frame.Parent.Variables[x].ToString(frame)));
                    }
                    Console.WriteLine(" ==== Stack Frame Dump: ====");
                    StackFrame temp = frame;
                    while(temp != null) {
                        Console.WriteLine("^-> " + temp.Pointer.File + "#" + temp.Pointer.Lineno + ": " + temp.Pointer.Line);
                        temp = temp.Parent;
                    }
                    Console.WriteLine(" ====  End  of  Dump  ====\n");
                    return Void;
                }),
                ["wait"] = ((caller, args, frame) => {
                    if(args.Count != 0)
                        throw VTSException.ArgCountException("core", "wait", 0, (uint)args.Count, frame);
                    Console.ReadKey();
                    return Void;
                }),
                ["typedump"] = ((caller, args, frame) => {
                    if(args.Count != 0) 
                        throw VTSException.ArgCountException("core", "typedump", 0, (uint)args.Count, frame);
                    Console.WriteLine(" ===== Current Memory Structures: =====");
                    Interpreter.Instance.Classes.ForEach(cls => {
                        Console.WriteLine(" -> " + cls.Key + " [" + cls.Value.Actions.Keys.Count +" actions; " + 
                            cls.Value.Internals.Keys.Count + " internals]");
                        cls.Value.Fields.Keys.ForEach(fld => Console.WriteLine("   -> Field::" + fld));
                        cls.Value.Actions.Values.ForEach(act => Console.WriteLine("   -> Action::" + act));
                        cls.Value.Internals.Keys.ForEach(ntr => Console.WriteLine("   -> Internal::" + ntr));
                        cls.Value.Operators.ForEach(x => Console.WriteLine("   -> Operator<" + x.Key + ">::" + x.Value));
                    });
                    Console.WriteLine(" ===== Current Modules: =====");
                    Interpreter.Instance.VTSModuleStatus.ForEach(x => Console.WriteLine(x.Key + ": activated? " + x.Value));
                    Console.WriteLine(" ===== End of Overview ===== \n\n");
                    return Void;
                })
            }
        };
        public static VTSClass VoidClass = new VTSClass() {
            Name = "Void", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>(), Operators = new Dictionary<VTSOperator, VTSAction>(),
            Internals = new Dictionary<string, Func<VTSVariable, List<VTSVariable>, StackFrame, VTSVariable>>() {
                ["toString"] = ((caller, args, frame) => {
                    if(args.Count != 0) 
                        throw VTSException.ArgCountException("core", "toString", 0, (uint)args.Count, frame);
                    else return new VTSVariable() { 
                        Class = VTSString, Mutable = false, 
                        Fields = new Dictionary<string, object>() { ["value"] = "(void)" }
                    };
                })
            }
        };
        public static VTSClass ListClass = new VTSClass() {
            Name = "list", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>() { ["value"] = "List<T>", ["count"] = "int" },
            Operators = new Dictionary<VTSOperator, VTSAction>(),
            Internals = new Dictionary<string, Func<VTSVariable, List<VTSVariable>, StackFrame, VTSVariable>>() {
                ["toString"] = ((caller, args, frame) => {
                    if(args.Count != 0)
                        throw VTSException.ArgCountException("list", "toString", 0, (uint)args.Count, frame);
                    else return new VTSVariable() {
                        Class = VTSString, Mutable = false,
                        Fields = new Dictionary<string, object>() { ["value"] = "[list`" + 
                            (int)((VTSVariable)caller.Fields["count"]).Fields["value"] + "]" }
                    };
                }),
                ["new"] = ((caller, args, frame) => {
                    if(args.Count != 0) 
                        throw VTSException.ArgCountException("list", "new", 0, (uint)args.Count, frame);
                    caller.Fields["value"] = new List<VTSVariable>();
                    caller.Fields["count"] = new VTSVariable() { 
                        Class = VTSInt, Mutable = false, Fields = new Dictionary<string, object>() {
                            ["value"] = 0
                        }
                    };
                    return caller;
                }),
                ["add"] = ((caller, args, frame) => {
                    if(args.Count != 1)
                        throw VTSException.ArgCountException("list", "add", 1, (uint)args.Count, frame);
                    ((List<VTSVariable>)caller.Fields["value"]).Add(args[0]);
                    ((VTSVariable)caller.Fields["count"]).Fields["value"] = 
                        (int)((VTSVariable)caller.Fields["count"]).Fields["value"] + 1;
                    return Void;
                }),
                ["get"] = ((caller, args, frame) => {
                    if(args.Count != 1)
                        throw VTSException.ArgCountException("list", "get", 1, (uint)args.Count, frame);
                    VTSVariable arg = args[0];
                    if(arg.Class != VTSInt) 
                        throw VTSException.TypeException(VTSInt, arg.Class, frame);
                    if((int)arg.Fields["value"] < 0 || 
                        (int)arg.Fields["value"] >= ((List<VTSVariable>)caller.Fields["value"]).Count)
                        throw new VTSException("RangeError", frame, 
                            "Index " + (int)arg.Fields["value"] + " out of range.", null);
                    return ((List<VTSVariable>)caller.Fields["value"])[(int)arg.Fields["value"]];
                }),
                ["set"] = ((caller, args, frame) => {
                    if(args.Count != 2)
                        throw VTSException.ArgCountException("list", "get", 1, (uint)args.Count, frame);
                    VTSVariable index = args[0];
                    VTSVariable val = args[1];
                    if(index.Class != VTSInt) throw VTSException.TypeException(VTSInt, index.Class, frame);
                    if((int)index.Fields["value"] < 0 || 
                        (int)index.Fields["value"] >= ((List<VTSVariable>)caller.Fields["value"]).Count)
                        throw new VTSException("RangeError", frame,
                            "Index " + (int)index.Fields["value"] + " out of range.", null);
                    ((List<VTSVariable>)caller.Fields["value"])[(int)index.Fields["value"]] = val;
                    return val;
                }),
                ["remove"] = ((caller, args, frame) => {
                    if(args.Count != 1)
                        throw VTSException.ArgCountException("list", "remove", 1, (uint)args.Count, frame);
                    VTSVariable index = args[0];
                    if(index.Class != VTSInt) throw VTSException.TypeException(VTSInt, index.Class, frame);
                    if((int)index.Fields["value"] < 0 ||
                        (int)index.Fields["value"] >= ((List<VTSVariable>)caller.Fields["value"]).Count)
                        throw new VTSException("RangeError", frame,
                            "Index " + (int)index.Fields["value"] + " out of range.", null);
                    VTSVariable res = ((List<VTSVariable>)caller.Fields["value"])[(int)index.Fields["value"]];
                    ((VTSVariable)caller.Fields["count"]).Fields["value"] = 
                        (int)((VTSVariable)caller.Fields["count"]).Fields["value"] - 1;
                    ((List<VTSVariable>)caller.Fields["value"]).RemoveAt((int)index.Fields["value"]);
                    return res;
                })
            }
        };
        #endregion
        
        #region Primitives
        public static VTSClass VTSInt = new VTSClass() {
            Name = "int", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>() { ["value"] = "int" },
            Operators = new Dictionary<VTSOperator, VTSAction>(){
                [VTSOperator.ADD] = new VTSAction() { Name = VTSOperator.ADD.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => { 
                        if(var1.Class == VTSInt && var2.Class == VTSInt)  
                            return new VTSVariable() {
                                Class = VTSInt,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = (int)var1.Fields["value"] + (int)var2.Fields["value"]
                                }
                            }; 
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.ADD, frame);
                    }
                },
                [VTSOperator.SUBTRACT] = new VTSAction() { Name = VTSOperator.SUBTRACT.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => { 
                        if(var1.Class == VTSInt && var2.Class == VTSInt)  
                            return new VTSVariable() {
                                Class = VTSInt,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = (int)var1.Fields["value"] - (int)var2.Fields["value"]
                                }
                            };
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.SUBTRACT, frame);
                    }
                },
                [VTSOperator.MULTIPLY] = new VTSAction() { Name = VTSOperator.MULTIPLY.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => { 
                        if(var1.Class == VTSInt && var2.Class == VTSInt)  
                            return new VTSVariable() {
                                Class = VTSInt,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = (int)var1.Fields["value"] * (int)var2.Fields["value"]
                                }
                            };
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.MULTIPLY, frame);
                    }
                },
                [VTSOperator.DIVIDE] = new VTSAction() { Name = VTSOperator.DIVIDE.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => { 
                        if(var1.Class == VTSInt && var2.Class == VTSInt)  
                            return new VTSVariable() {
                                Class = VTSInt,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = (int)var1.Fields["value"] / (int)var2.Fields["value"]
                                }
                            };
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.DIVIDE, frame);
                    }
                },
                [VTSOperator.MODULUS] = new VTSAction() { Name = VTSOperator.MODULUS.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => { 
                        if(var1.Class == VTSInt && var2.Class == VTSInt)  
                            return new VTSVariable() {
                                Class = VTSInt,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = (int)var1.Fields["value"] % (int)var2.Fields["value"]
                                }
                            };
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.MODULUS, frame);
                    }
                },
                [VTSOperator.EQUALS] = new VTSAction() { Name = VTSOperator.EQUALS.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => { 
                        if(var1.Class == VTSInt && var2.Class == VTSInt)  
                            return new VTSVariable() {
                                Class = VTSBool,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = ((int)var1.Fields["value"] == (int)var2.Fields["value"])
                                }
                            };
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.EQUALS, frame);
                    }
                },
                [VTSOperator.LARGER] = new VTSAction() { Name = VTSOperator.LARGER.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => { 
                        if(var1.Class == VTSInt && var2.Class == VTSInt)  
                            return new VTSVariable() {
                                Class = VTSBool,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = ((int)var1.Fields["value"] > (int)var2.Fields["value"])
                                }
                            };
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.LARGER, frame);
                    }
                },
                [VTSOperator.SMALLER] = new VTSAction() { Name = VTSOperator.SMALLER.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => { 
                        if(var1.Class == VTSInt && var2.Class == VTSInt)  
                            return new VTSVariable() {
                                Class = VTSBool,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = ((int)var1.Fields["value"] < (int)var2.Fields["value"])
                                }
                            };
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.SMALLER, frame);
                    }
                }
            },
            Internals = new Dictionary<string, Func<VTSVariable, List<VTSVariable>, StackFrame, VTSVariable>>() {
                ["toFloat"] = ((caller, args, frame) => {
                    if(args.Count != 0) throw VTSException.ArgCountException("int", "toFloat", 0, (uint)args.Count, frame);
                    else return new VTSVariable() {
                        Class = VTSFloat,
                        Mutable = false,
                        Fields = new Dictionary<string, object>() {
                            ["value"] = (float)caller.Fields["value"]
                        }
                    };
                }),

                ["toString"] = ((caller, args, frame) => {
                    if(args.Count != 0) throw VTSException.ArgCountException("int", "toString", 0, (uint)args.Count, frame);
                    else return new VTSVariable() {
                        Class = VTSString,
                        Mutable = false,
                        Fields = new Dictionary<string, object>() {
                            ["value"] = caller.Fields["value"].ToString()
                        }
                    };
                })
            }
        };
        public static VTSClass VTSString = new VTSClass() {
            Name = "string", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>() { ["value"] = "string" },
            Operators = new Dictionary<VTSOperator, VTSAction>() {
                [VTSOperator.ADD] = new VTSAction() { Name = VTSOperator.ADD.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => {
                        if(var1.Class == VTSString && var2.Class == VTSString) {
                            return new VTSVariable() {
                                Class = VTSString,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = var1.Fields["value"].ToString() + var2.Fields["value"].ToString()
                                }
                            };
                        }
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.ADD, frame);
                    }
                },
                [VTSOperator.EQUALS] = new VTSAction() { Name = VTSOperator.ADD.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => {
                        if(var1.Class == VTSString && var2.Class == VTSString) {
                            return new VTSVariable() {
                                Class = VTSBool,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = (var1.Fields["value"].ToString() == var2.Fields["value"].ToString())
                                }
                            };
                        }
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.EQUALS, frame);
                    }
                }
            },
            Internals = new Dictionary<string, Func<VTSVariable, List<VTSVariable>, StackFrame, VTSVariable>>() {
                ["toString"] = ((caller, args, frame) => {
                    if(args.Count != 0) 
                        throw VTSException.ArgCountException("string", "toString", 0, (uint)args.Count, frame);
                    else return caller;
                })
            }
        };
        public static VTSClass VTSFloat = new VTSClass() {
            Name = "float", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>() { ["value"] = "float" },
            Operators = new Dictionary<VTSOperator, VTSAction>(){
                [VTSOperator.ADD] = new VTSAction() { Name = VTSOperator.ADD.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => { 
                        if(var1.Class == VTSFloat && var2.Class == VTSFloat)  
                            return new VTSVariable() {
                                Class = VTSFloat,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = (float)var1.Fields["value"] + (float)var2.Fields["value"]
                                }
                            }; 
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.ADD, frame);
                    }
                },
                [VTSOperator.SUBTRACT] = new VTSAction() { Name = VTSOperator.SUBTRACT.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => { 
                        if(var1.Class == VTSFloat && var2.Class == VTSFloat)  
                            return new VTSVariable() {
                                Class = VTSFloat,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = (float)var1.Fields["value"] - (float)var2.Fields["value"]
                                }
                            };
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.SUBTRACT, frame);
                    }
                },
                [VTSOperator.MULTIPLY] = new VTSAction() { Name = VTSOperator.MULTIPLY.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => { 
                        if(var1.Class == VTSFloat && var2.Class == VTSFloat)  
                            return new VTSVariable() {
                                Class = VTSFloat,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = (float)var1.Fields["value"] * (float)var2.Fields["value"]
                                }
                            };
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.MULTIPLY, frame);
                    }
                },
                [VTSOperator.DIVIDE] = new VTSAction() { Name = VTSOperator.DIVIDE.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => { 
                        if(var1.Class == VTSFloat && var2.Class == VTSFloat)  
                            return new VTSVariable() {
                                Class = VTSFloat,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = (float)var1.Fields["value"] / (float)var2.Fields["value"]
                                }
                            };
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.DIVIDE, frame);
                    }
                },
                [VTSOperator.EQUALS] = new VTSAction() { Name = VTSOperator.EQUALS.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => { 
                        if(var1.Class == VTSFloat && var2.Class == VTSFloat)  
                            return new VTSVariable() {
                                Class = VTSBool,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = ((float)var1.Fields["value"] == (float)var2.Fields["value"])
                                }
                            };
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.EQUALS, frame);
                    }
                },
                [VTSOperator.LARGER] = new VTSAction() { Name = VTSOperator.LARGER.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => { 
                        if(var1.Class == VTSFloat && var2.Class == VTSFloat)  
                            return new VTSVariable() {
                                Class = VTSBool,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = ((float)var1.Fields["value"] > (float)var2.Fields["value"])
                                }
                            };
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.LARGER, frame);
                    }
                },
                [VTSOperator.SMALLER] = new VTSAction() { Name = VTSOperator.SMALLER.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => { 
                        if(var1.Class == VTSFloat && var2.Class == VTSFloat)  
                            return new VTSVariable() {
                                Class = VTSBool,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = ((float)var1.Fields["value"] < (float)var2.Fields["value"])
                                }
                            };
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.SMALLER, frame);
                    }
                }
            },
            Internals = new Dictionary<string, Func<VTSVariable, List<VTSVariable>, StackFrame, VTSVariable>>() {
                ["toInt"] = ((caller, args, frame) => {
                    if(args.Count != 0) throw VTSException.ArgCountException("float", "toFloat", 0, (uint)args.Count, frame);
                    else return new VTSVariable() {
                        Class = VTSFloat,
                        Mutable = false,
                        Fields = new Dictionary<string, object>() {
                            ["value"] = (int)((float)caller.Fields["value"])
                        }
                    };
                }),

                ["toString"] = ((caller, args, frame) => {
                    if(args.Count != 0) throw VTSException.ArgCountException("float", "toString", 0, (uint)args.Count, frame);
                    else return new VTSVariable() {
                        Class = VTSString,
                        Mutable = false,
                        Fields = new Dictionary<string, object>() {
                            ["value"] = caller.Fields["value"].ToString()
                        }
                    };
                })
            }
        };
        public static VTSClass VTSBool = new VTSClass() {
            Name = "bool", Actions = new Dictionary<string, VTSAction>(),
            Fields = new Dictionary<string, string>() { ["value"] = "bool" },
            Operators = new Dictionary<VTSOperator, VTSAction>() {
                [VTSOperator.AND] = new VTSAction() { Name = VTSOperator.AND.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => {
                        if(var1.Class == VTSBool && var2.Class == VTSBool) 
                            return (bool)var1.Fields["value"] && (bool)var2.Fields["value"] ? True() : False(); 
                            /*return new VTSVariable() {
                                Class = VTSBool,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = (bool)var1.Fields["value"] && (bool)var2.Fields["value"]
                                }
                            }; */
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.AND, frame);
                    }
                },
                [VTSOperator.OR] = new VTSAction() { Name = VTSOperator.OR.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => {
                        if(var1.Class == VTSBool && var2.Class == VTSBool)
                            return (bool)var1.Fields["value"] || (bool)var2.Fields["value"] ? True() : False();
                            /*return new VTSVariable() {
                                Class = VTSBool,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = (bool)var1.Fields["value"] || (bool)var2.Fields["value"]
                                }
                            }; */
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.OR, frame);
                    }
                },
                [VTSOperator.EQUALS] = new VTSAction() { Name = VTSOperator.EQUALS.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => {
                        if(var1.Class == VTSBool && var2.Class == VTSBool)
                            return (bool)var1.Fields["value"] == (bool)var2.Fields["value"] ? True() : False();
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.EQUALS, frame);
                    }
                }
            },
            Internals = new Dictionary<string, Func<VTSVariable, List<VTSVariable>, StackFrame, VTSVariable>>() {
                ["flip"] = ((caller, args, frame) => {
                    if(args.Count != 0) throw VTSException.ArgCountException("bool", "flip", 0, (uint)args.Count, frame);
                    else return new VTSVariable() {
                        Class = VTSBool,
                        Mutable = false,
                        Fields = new Dictionary<string, object>() {
                            ["value"] = !(bool)caller.Fields["value"]
                        }
                    };
                }),
                ["toString"] = ((caller, args, frame) => {
                    if(args.Count != 0) throw VTSException.ArgCountException("int", "toString", 0, (uint)args.Count, frame);
                    else return new VTSVariable() {
                        Class = VTSString,
                        Mutable = false,
                        Fields = new Dictionary<string, object>() {
                            ["value"] = ((bool)caller.Fields["value"]).ToString()
                        }
                    };
                })
            }
        };
        #endregion

        #region builtin constants
        public static VTSVariable Void = new VTSVariable() {
            Class = VoidClass, Mutable = false, Fields = new Dictionary<string, object>(), Name = "void"
        };

        public static VTSVariable Core = new VTSVariable() {
            Class = CoreClass, Mutable = false, Fields = new Dictionary<string, object>(), Name = "core"
        };

        public static VTSVariable False() => new VTSVariable() {
            Class = VTSBool, Mutable = false, Fields = new Dictionary<string, object>() { ["value"] = false },
            Name = "false"
        };

        public static VTSVariable True() => new VTSVariable() {
            Class = VTSBool, Mutable = false, Fields = new Dictionary<string, object>() { ["value"] = true },
            Name = "true"
        };
        #endregion

        #region Containers
        public static Dictionary<string, VTSVariable> BuiltinVariables = new Dictionary<string, VTSVariable>() {
            ["void"] = Void, ["core"] = Core//, ["false"] = CoreStructures.False, ["true"] = CoreStructures.True
        };

        public static Dictionary<string, VTSClass> BuiltinClasses = new Dictionary<string, VTSClass>() { 
            ["Core"] = CoreClass, ["Voidtype"] = VoidClass, 
            ["int"] = VTSInt, ["string"] = VTSString, ["float"] = VTSFloat, ["bool"] = VTSBool,
            ["list"] = ListClass
        };
        #endregion
    }
}
