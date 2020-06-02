using System;
using System.Linq;
using System.Collections.Generic;
using Jay.VTS;
using Jay.VTS.Enums;
using Jay.Logging;
using Jay.VTS.Structures;

namespace Jay.VTS.Parser
{
    public class LineSplitter 
    {
        public CodeBlock Target;
        public (string, int) Location;
        public static List<string> Control = new List<string>(){
            "if", "else", /*"elseif",*/ "while", //"for"
        };
        /*public static List<string> Operator = new List<string>(){
            "=", "==", /*"+=", "*=", "-=", "/=", "%=", "*", "-", "+", "*", "/", "%", /*"!=", "~", 
            "AND", "OR", "NOT"
        };*/
        public static List<string> MultChar = new List<string>(){
            "\\n", "\\t", "\\r"
        };

        public LineSplitter(CodeBlock Target, (string, int) Location) {
            this.Target = Target;
            this.Location = Location;
        }

        public LineElement SplitTarget() {
            LineElement root = new LineElement(){
                Type = ElementType.Block,
                Inner = new List<LineElement>(),
                Content = ""
            };
            if(Target.Line.StartsWith("#")) {
                return new LineElement() {
                    Type = ElementType.Preproc,
                    Inner = null,
                    Content = ""
                };
            }

            string currStr = "";
            LineElement pointer = root;
            bool inString = false;
            bool finished = false;
            bool inComment = false;
            ElementType next = ElementType.None;
            Target.Line.ToCharArray().ToList().ForEach(x => {
                if(!finished) {
                    if(inComment) {
                        if(x == '/' && currStr.EndsWith("*")) {
                            inComment = false;
                            pointer.Inner.Add(new LineElement() {
                                Type = ElementType.Comment,
                                Content = currStr,
                                Inner = null,
                                Parent = pointer
                            });
                            currStr = "";
                        }
                        else {
                            currStr += x;
                        }
                    }
                    else if(inString) {
                        if(x == '"') { 
                            inString = false;
                            pointer.Inner.Add(new LineElement() {
                                Type = ElementType.Literal,
                                Content = currStr,
                                Inner = null,
                                Parent = pointer
                            });
                            currStr = "";
                        }
                        else {
                            currStr += x;
                        }
                    }
                    else {
                        if(x == ' ' || x == ',') {
                            
                            LineElement tmp = (new LineElement(){
                                Content = currStr,
                                Inner = null,
                                Parent = pointer,
                                Type = next == ElementType.None ? DetermineType(currStr) : next
                            });
                            //if(tmp.Type == ElementType.Operator) { /*infix -> prefix*/ }
                            if(tmp.Type != ElementType.Void) { pointer.Inner.Add(tmp); }
                            currStr = "";
                            if(next != ElementType.None) { next = ElementType.None; }

                            if(x == ',') { 
                                pointer.Inner.Add(new LineElement(){
                                    Content = ",",
                                    Inner = null,
                                    Parent = pointer,
                                    Type = ElementType.Separator
                                });
                            }
                        }
                        else if(x == '.') {
                            LineElement tmp = (new LineElement(){
                                Content = currStr,
                                Inner = null,
                                Parent = pointer,
                                Type = next == ElementType.None ? DetermineType(currStr) : next
                            });
                            if(tmp.Type != ElementType.Void) { pointer.Inner.Add(tmp); }
                            next = ElementType.Member;
                            currStr = "";
                        }
                        else if(x == '(') {
                            LineElement tmp = (new LineElement(){
                                Content = currStr,
                                Inner = null,
                                Parent = pointer,
                                Type = next == ElementType.None ? DetermineType(currStr) : next
                            });
                            if(tmp.Type != ElementType.Void) { pointer.Inner.Add(tmp); }
                            if(next != ElementType.None) { next = ElementType.None; }
                            currStr = "";
                            LineElement newInner = new LineElement() {
                                Type = ElementType.Block,
                                Content = null,
                                Inner = new List<LineElement>(),
                                Parent = pointer
                            };
                            pointer.Inner.Add(newInner);
                            pointer = newInner;
                        }
                        else if(x == ')'){
                            LineElement tmp = (new LineElement(){
                                Content = currStr,
                                Inner = null,
                                Parent = pointer,
                                Type = next == ElementType.None ? DetermineType(currStr) : next
                            });
                            pointer.Inner.Add(tmp);
                            currStr = "";
                            if(next != ElementType.None) { next = ElementType.None; }
                            if(pointer.Parent != null) {
                                pointer = pointer.Parent;
                                if(next != ElementType.None) {
                                    throw new VTSException("SyntaxError", "firstPass::line", 
                                        $"Unexpected ')' in <{Location.Item1}> on line <{Location.Item2}> , expected <field> or <action>");
                                }
                            }
                            else {
                                throw new VTSException("SyntaxError", "firstPass::line", 
                                    $"Unexpected ')' in <{Location.Item1}> on line <{Location.Item2}>, expected <keyword> or <identifier>");
                            }
                        }
                        else if(x == '"') {
                            inString = true;
                            LineElement tmp = (new LineElement(){
                                Content = currStr,
                                Inner = null,
                                Parent = pointer,
                                Type = next == ElementType.None ? DetermineType(currStr) : next
                            });
                            if(tmp.Type != ElementType.Void) { pointer.Inner.Add(tmp); }
                            if(next != ElementType.None) { next = ElementType.None; }
                            currStr = "";
                        }
                        else {
                            currStr += x;
                            if(currStr == "/*") {
                                if(next != ElementType.None) {
                                    throw new VTSException("SyntaxError", "firstPass::line", 
                                        $"Unexpected <comment> in <{Location.Item1}> on line <{Location.Item2}>, expected <field> or <action>");
                                }
                                inComment = true;
                            }
                        }
                    }
                }
            });

            LineElement last = (new LineElement(){
                Content = currStr,
                Inner = null,
                Parent = pointer,
                Type = next == ElementType.None ? DetermineType(currStr) : next
            });
            if(last.Type != ElementType.Void) { pointer.Inner.Add(last); } 
            currStr = "";

            if(root != pointer) { 
                throw new VTSException("SyntaxError", "fistPass::line", 
                    $"Unexpected ';' in <{Location.Item1}> on line <{Location.Item2}>, expected ')'");
            }
            //current result: [ { lst: Identifier }, { =: Operator }, { list: Identifier }, { new: Member }, [ { : Void } ] ]
            Logger.Log(" ========== Starting rerooting dots on: ========== ");
            Logger.Log(root.ToOneliner());
            Logger.Log(" ==========    Rerooting operation:     ========== ");
            RerootDots(root);
            Logger.Log(" ==========       Rerooted result:      ========== ");
            Logger.Log(root.ToOneliner());
            return root;
            //return RerootDots(root);
            //return root;
        }

        private void RerootDots(LineElement root) {
            if(root.Type != ElementType.Block) return;
            for(int ind = 0; ind < root.Inner.Count; ind++) {
                if(root[ind].Type == ElementType.Member) {
                    if(ind < root.Inner.Count - 1 && root[ind + 1].Type == ElementType.Block) { 
                        Logger.Log("Is actual member [" + root[ind].ToOneliner() + "]");
                        /*do nothing*/ 
                    } 
                    else if(ind > 0 && root.Inner[ind - 1].Type == ElementType.Literal) {
                        Logger.Log("Is literal [" + root[ind].ToOneliner() + "]");
                        root.Inner[ind - 1].Content += "." + root.Inner[ind].Content;
                        root.Inner[ind].Type = ElementType.None;
                    }
                    else {
                        Logger.Log("Is field [" + root[ind].ToOneliner() + "]");
                        root.Inner[ind].Type = ElementType.Field;
                    }
                }
                else if(root[ind].Type == ElementType.Block) {
                    Logger.Log("Working recursive on block [" + root[ind].ToOneliner() + "]");
                    RerootDots(root[ind]);
                }
            }
        }

        private ElementType DetermineType(string tmp) {
            if(tmp == "") return ElementType.Void;
            if(tmp == "class") return ElementType.Class;
            if(tmp == "action") return ElementType.Action;
            if(tmp == "return") return ElementType.Return;
            if(tmp == "field") return ElementType.Field;
            if(Control.Contains(tmp)) return ElementType.Control;
            if(VTSOperator._ops.Contains(tmp)) return ElementType.Operator;
            if(int.TryParse(tmp, out int a) || float.TryParse(tmp, out float b) || 
                tmp == "true" || tmp == "false" /*|| tmp.Length == 1 || MultChar.Contains(tmp)*/) return ElementType.Literal;
            return ElementType.Identifier;
        }
    }
}