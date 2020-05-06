using System;
using System.Linq;
using System.Collections.Generic;

namespace Jay.VTS
{
    public class LineSplitter 
    {
        public CodeBlock Target;
        public (string, int) Location;
        public static List<string> Control = new List<string>(){
            "if", "else", "elseif", "while", "do", "for", "foreach", "unless"
        };
        public static List<string> Operator = new List<string>(){
            "=", "==", "+=", "*=", "-=", "/=", "%=", "*", "-", "+", "*", "/", "%", "!=", "~"
        };
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

            string currStr = "";
            LineElement pointer = root;
            bool inString = false;
            bool finished = false;
            ElementType next = ElementType.None;
            Target.Line.ToCharArray().ToList().ForEach(x => {
                if(!finished) {
                    if(inString) {
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
                            if(tmp.Type != ElementType.Void) { pointer.Inner.Add(tmp); }
                            currStr = "";
                            if(next != ElementType.None) { next = ElementType.None; }
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
                            if(currStr == "//") {
                                if(next != ElementType.None) {
                                    throw new VTSException("SyntaxError", "firstPass::line", 
                                        $"Unexpected <comment> in <{Location.Item1}> on line <{Location.Item2}>, expected <field> or <action>");
                                }
                                pointer.Inner.Add(new LineElement(){
                                    Type = ElementType.Comment,
                                    Content = "",
                                    Inner = null,
                                    Parent = pointer
                                });
                                finished = true;
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
            return root;
        }

        private ElementType DetermineType(string tmp) {
            if(tmp == "") return ElementType.Void;
            if(tmp == "class") return ElementType.Class;
            if(tmp == "action") return ElementType.Action;
            if(tmp == "return") return ElementType.Return;
            if(Control.Contains(tmp)) return ElementType.Control;
            if(Operator.Contains(tmp)) return ElementType.Operator;
            if(int.TryParse(tmp, out int a) || float.TryParse(tmp, out float b) || 
                tmp == "true" || tmp == "false" || tmp.Length == 1 || MultChar.Contains(tmp)) return ElementType.Literal;
            return ElementType.Identifier;
        }
    }

    public class LineElement
    {
        public ElementType Type;
        public string Content;
        public List<LineElement> Inner;
        public LineElement Parent;

        public string ToOneliner() {
            if(Type == ElementType.Block) {
                string res = "[ ";
                List<string> data = new List<string>();
                Inner.ForEach(x => data.Add(x.ToOneliner()));
                res += string.Join(", ", data);
                res += " ]";
                return res;
            }
            else {
                return "{ " + Content + ": " + Type + " }";
            }
        }

        public string ToString(uint Offset) {
            if(Type == ElementType.Block) {
                List<string> res = new List<string>();
                /*string tmp = "";
                for(int i = 0; i < Offset; i++) { tmp += "\t"; }
                res.Add(tmp + $"[{Type}] {Inner.Count}");*/
                Inner.ForEach( x => res.Add(x.ToString(Offset + 1)));
                return string.Join("\n", res);
            }
            else {
                string res = "";
                for(int i = 0; i < Offset; i++) { res += "  "; }
                return res + $"[{Type}]{Content}";
            }
        }
    }

    public enum ElementType { 
        Block = -0x10,

        None = 0x00,
        Void,

        Class = 0x10,
        Action,
        Control,
        Return,
        Member,

        Identifier = 0x20,
        Literal,
        
        Operator = 0x30,
        
        Comment = 0x40
     }
}