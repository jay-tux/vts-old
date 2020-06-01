using Jay.VTS.Structures;
using Jay.VTS.Execution;
using Jay.VTS.Parser;
using Jay.Logging;
using System.Linq;
using Jay.Xtend;
using System.Collections.Generic;

namespace Jay.VTS.Structures
{
    public class VTSVariable
    {
        public VTSClass Class;
        public bool Mutable;
        public bool IsTypeRef = false;
        public Dictionary<string, object> Fields;
        public string Name;
        public static VTSVariable UNDEFINED(string name) => new VTSVariable() {
            Class = null, Mutable = false, IsTypeRef = false, Name = name
        };

        public override string ToString() => "~>" + (Class == null ? "(typeless)" : Class.Name);

        /*public string ToString(StackFrame caller) => 
            (Class.Actions.ContainsKey("toString") || Class.Internals.ContainsKey("toString")) ? 
                Call("toString", caller, new List<VTSVariable>()).Fields["value"].ToString() : 
                ("~>" + Class.Name);*/

        public string ToString(StackFrame caller) {
            if(Class == null) return "~> (typeless)";
            if(Class.Actions == null) return "~> (" + Class.Name + ":actionless)";
            if(Class.Internals == null) return "~> (" + Class.Name + ":internal-less)";
            if(Class.Actions.ContainsKey("toString") || Class.Internals.ContainsKey("toString")) {
                VTSVariable res = Call("toString", caller, new List<VTSVariable>());
                if(res == null) return "(void)";
                else if(res.Fields == null) return "(fieldless)";
                else if(res.Fields["value"] == null) return "(void value)";
                else return res.Fields["value"].ToString();
            }
            else {
                return "~>" + Class.Name;
            }
        }

        public VTSVariable Call(LineElement action, StackFrame frame, VTSVariable other) 
        {
            if(Class.Operators.ContainsKey((VTSOperator)action)) {
                VTSAction toRun = Class.Operators[(VTSOperator)action];
                if(toRun.IsInternalCall) {
                    //internal call: pass variables and frame, return result
                    return toRun.InternalCall(this, other, frame);
                }
                else {
                    //non internal call: prepare action call
                    //prepare new stackframe
                    StackFrame sf = new StackFrame(toRun.Instructions, 0){
                        Parent = frame,
                        Variables = new Dictionary<string, VTSVariable>() {
                            ["this"] = this,
                            ["other"] = other
                        },
                        ParentClass = Class
                    };
                    VTSVariable result = null;
                    //hook event
                    sf.StackFrameReturns += (src, res) => {
                        switch(res.ExitCode) {
                            case FrameEventArgs.Exits.ReturnValue: 
                                //set return value
                                result = res.ReturnValue; 
                                break;
                            case FrameEventArgs.Exits.Return: 
                                //no return value
                                result = CoreStructures.Void;
                                break;
                            case FrameEventArgs.Exits.InternalException:
                            case FrameEventArgs.Exits.CodeException: 
                                //error; crash
                                frame.Crash(res);
                                break;
                        }
                    };
                    return result;
                }
            }
            else {
                throw new VTSException("NameError", frame, "Class " + Class.Name + 
                    " doesn't have an operator " + action, null);
            }
        }

        public VTSVariable Call(string action, StackFrame frame, List<VTSVariable> args) 
        {
            Logger.Log(" ======= Call  details: ======");
            Logger.Log("Action = " + action);
            Logger.Log("Args = " + string.Join(", ", args.Select(x => x.Class)));
            Logger.Log("Caller = " + Class);
            if(Fields != null) Fields.Keys.ForEach(x => Logger.Log("  [" + x + "] = [" + Fields[x] + "]"));
            Logger.Log(" ======= End of details ======");
            if(IsTypeRef) {
                //special case: static action aka constructor
                if(action == "new") {
                    Logger.Log("Trying to execute constructor...");
                    //execute constructor, return result
                    return Class.Create(frame, args);
                }
                else {
                    //throw exception: only constructor is static call
                    throw new VTSException("ActionError", frame, "Action <" + action + "> is only accessible from" +
                        " an instance, not from a class reference.", null);
                }
            }
            else if(Class.Actions.ContainsKey(action)) {
                Logger.Log("Trying to execute non-internal action...");
                if(Class.Actions[action].ArgNames.Count != args.Count) {
                    throw new VTSException("ArgumentError", frame, "Action <" + Class.Name + "." + action + 
                        "> expects " + Class.Actions[action].ArgNames.Count + " arguments, " + args.Count + " given.",
                        null);
                }
                StackFrame sf = new StackFrame(Class.Actions[action].Instructions, 0){ 
                    Parent = frame,
                    Variables = new Dictionary<string, VTSVariable>(){
                        ["this"] = this
                    },
                    ParentClass = Class
                };
                for(int i = 0; i < args.Count; i++) {
                    sf.Variables[Class.Actions[action].ArgNames[i]] = args[i];
                }
                VTSVariable result = null;
                sf.StackFrameReturns += (src, res) => {
                    switch(res.ExitCode) {
                        case FrameEventArgs.Exits.ReturnValue: 
                            Logger.Log("Exited with return value");
                            result = res.ReturnValue; 
                            break;
                        case FrameEventArgs.Exits.Return: 
                            Logger.Log("Exited without return value");
                            result = CoreStructures.Void;
                            break;
                        case FrameEventArgs.Exits.InternalException:
                        case FrameEventArgs.Exits.CodeException: 
                            Logger.Log("Crashed due to error");
                            frame.Crash(res);
                            break;
                    }
                };
                sf.Execute();
                return result;
            }
            else if(Class.Internals.ContainsKey(action)) {
                Logger.Log("Trying to execute internal action...");
                return Class.Internals[action](this, args, frame);
            }
            else {
                throw new VTSException("NameError", frame, "Class " + Class.Name + 
                    " doesn't have a member action " + action, null);
            }
        }
    }
}