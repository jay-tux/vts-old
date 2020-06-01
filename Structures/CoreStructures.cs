using System;
using Jay.VTS.Execution;
using Jay.VTS.Structures;
using System.Collections.Generic;

namespace Jay.VTS.Structures
{
    public static class CoreStructures
    {
        #region builtin classes
        #region Core Class
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
                })
            }
        };
        #endregion
        #region Void Class
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
        #endregion
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
                [VTSOperator.MODULUS] = new VTSAction() { Name = VTSOperator.MODULUS.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => { 
                        if(var1.Class == VTSFloat && var2.Class == VTSFloat)  
                            return new VTSVariable() {
                                Class = VTSFloat,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = (float)var1.Fields["value"] % (float)var2.Fields["value"]
                                }
                            };
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.MODULUS, frame);
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
                            return new VTSVariable() {
                                Class = VTSBool,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = (bool)var1.Fields["value"] && (bool)var2.Fields["value"]
                                }
                            }; 
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.AND, frame);
                    }
                },
                [VTSOperator.OR] = new VTSAction() { Name = VTSOperator.OR.Name, IsInternalCall = true,
                    InternalCall = (var1, var2, frame) => {
                        if(var1.Class == VTSBool && var2.Class == VTSBool)  
                            return new VTSVariable() {
                                Class = VTSBool,
                                Mutable = false,
                                Fields = new Dictionary<string, object>() {
                                    ["value"] = (bool)var1.Fields["value"] || (bool)var2.Fields["value"]
                                }
                            }; 
                        else throw VTSException.OperatorException(var1, var2, VTSOperator.OR, frame);
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

        public static VTSVariable False = new VTSVariable() {
            Class = VTSBool, Mutable = false, Fields = new Dictionary<string, object>() { ["value"] = false },
            Name = "false"
        };

        public static VTSVariable True = new VTSVariable() {
            Class = VTSBool, Mutable = false, Fields = new Dictionary<string, object>() { ["value"] = true },
            Name = "true"
        };
        #endregion

        #region Containers
        public static Dictionary<string, VTSVariable> BuiltinVariables = new Dictionary<string, VTSVariable>() {
            ["void"] = Void, ["core"] = Core, ["false"] = CoreStructures.False, ["true"] = CoreStructures.True
        };

        public static Dictionary<string, VTSClass> BuiltinClasses = new Dictionary<string, VTSClass>() { 
            ["Core"] = CoreClass, ["Voidtype"] = VoidClass, 
            ["int"] = VTSInt, ["string"] = VTSString, ["float"] = VTSFloat, ["bool"] = VTSBool
        };
        #endregion
    }
}
