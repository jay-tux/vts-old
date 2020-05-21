using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Jay.VTS;
using Jay.VTS.Enums;
using Jay.VTS.Structures;

namespace Jay.VTS.Parser
{
    public class CodeSplitter
    {
        public string OGCode { get; }
        public string File { get; }

        public CodeSplitter(string code, string file)
        {
            OGCode = string.Join('\n', code.Split('\n').ToList().Where(x => !x.Trim().StartsWith("//")));
            File = file;
        }

        public CodeBlock SplitCode()
        {
            VTSClass containing = null;
            CodeBlock method = null;
            int depth = 0;
            CodeBlock data = new CodeBlock()
            {
                IsLine = false,
                File = File,
                Lineno = 0,
                Parent = null,
                Contents = new List<CodeBlock>(),
                Split = null,
                Type = "file",
                Line = null
            };
            CodeBlock current = data;
            int lineno = 1;
            bool inString = false;
            string currLine = "";
            OGCode.ToCharArray().ToList().ForEach(chr => {
                if(inString) 
                {
                    if(chr == '\n') { throw new VTSException("Syntax Error", "firstPass::code", 
                        $"Newline in constant in <{this.File}>, on line <{lineno}>"); }
                    if(chr == '"') { inString = false; }
                    currLine += chr;
                }
                else 
                {
                    switch(chr)
                    {
                        case '"':
                            inString = true;
                            currLine += '"';
                        break; 

                        case '\n':
                            lineno++;
                            currLine += ' ';
                        break;

                        case '{':
                            CodeBlock inner = new CodeBlock() {
                                Line = Regex.Replace(currLine.TrimStart(), @"\s+", " "),
                                Lineno = lineno,
                                IsLine = false,
                                File = this.File,
                                Parent = current,
                                Contents = new List<CodeBlock>(),
                            };
                            inner.Split = new LineSplitter(inner, (File, lineno)).SplitTarget();
                            if(inner.Split.Inner[0].Type == ElementType.Class) {
                                Console.WriteLine("Found class " + inner.Split.Inner[1].Content);
                                if(containing != null) {
                                    throw new VTSException("SyntaxError", "firstPass::code",
                                        "In <" + this.File + ">, on line <" + lineno + 
                                        ">: Class within class is not allowed.");
                                }
                                if(method != null) {
                                    throw new VTSException("SyntaxError", "firstPass::code",
                                        "In <" + this.File + ">, on line <" + lineno +
                                        ">: Class within action is not allowed.");
                                }
                                if(Interpreter.Instance.ContainsClass(inner.Split.Inner[1].Content)) {
                                    throw new VTSException("SyntaxError", "firstPass::code", 
                                        "In <" + this.File + "> on line <" + lineno + ">: Class <" + 
                                        inner.Split.Inner[1].Content + "> is already defined.");
                                }
                                else {
                                    //Interpreter.Instance.AddClass(inner);
                                    containing = (VTSClass)inner;
                                    //Interpreter.Instance.PrintClasses();
                                }
                            }
                            else if(inner.Split.Inner[0].Type == ElementType.Action) {
                                Console.WriteLine("Found action " + inner.Split.Inner[1].Content + " in " + containing);
                                if(containing == null){
                                    throw new VTSException("SyntaxError", "firstPass::code",
                                        "Code-structure <action> " + inner.Split.Inner[1].Content + 
                                        ", as defined in <" + this.File + "> on line <" + lineno +
                                        ">, can only exist within a <class> or as an <entry>.");
                                }
                                if(method != null) {
                                    throw new VTSException("SyntaxError", "firstPass::code",
                                        "In <" + this.File + ">, on line <" + lineno + 
                                        " >: Action within action is not allowed.");
                                }
                                else if(containing.Contains(inner.Split.Inner[1].Content)) {
                                    throw new VTSException("SyntaxError", "firstPass::code",
                                        "In <" + this.File + ">, on line <" + lineno + ">: Action or Field <" +
                                        inner.Split.Inner[1].Content + "> is already defined in class <" +
                                        containing.Name + ">.");
                                }
                                else {
                                    method = inner;
                                    //containing.Actions[inner.Split.Inner[1].Content] = (VTSAction)inner;
                                    //containing.PrintActions();
                                }
                            }
                            else 
                            {
                                depth++;
                            }
                            inner.Type = inner.Line.Split(' ')[0];
                            if(inner.Type.Length < 4) { inner.Type += "  "; }
                            current.Contents.Add(inner);
                            current = inner;
                            currLine = "";
                        break;

                        case '}':
                            if(current.Parent == null) { throw new VTSException("SyntaxError", "firstPass::code", 
                                "Unexpected '}' in <" + File + "> on line <" + lineno + ">"); }
                                if(depth == 0) {
                                    if(method != null) {
                                        if(VTSOperator.IsOperator(method.Split.Inner[1].Content)) {
                                            containing.Operators[(VTSOperator)method.Split.Inner[1].Content] = (VTSAction)method;
                                        }
                                        else {
                                            containing.Actions[method.Split.Inner[1].Content] = (VTSAction)method;
                                        }
                                        method = null;
                                    }
                                    else if(containing != null) {
                                        Interpreter.Instance.AddClass(containing);
                                        containing = null;
                                    }
                                }
                                else {
                                    depth--;
                                }
                            current = current.Parent;
                            currLine = "";
                        break;

                        case ';':
                            CodeBlock vl = (new CodeBlock(){
                                IsLine = true,
                                File = this.File,
                                Line = Regex.Replace(currLine.TrimStart(), @"\s+", " "),
                                Contents = null,
                                Lineno = lineno,
                                Type = "code"
                            });
                            vl.Split = new LineSplitter(vl, (File, lineno)).SplitTarget();
                            if(vl.Split.Inner != null && vl.Split.Inner[0].Type == ElementType.Field) {
                                if(containing != null) {
                                    if(method == null) {
                                        if(!containing.Contains(vl.Split.Inner[1].Content)) {
                                            containing.Fields[vl.Split.Inner[1].Content] = "_void";
                                        }
                                        else {
                                            throw new VTSException("SyntaxError", "firstPass::code",
                                                "In <" + this.File + ">, on line <" + lineno + ">: Action or Field <" +
                                                vl.Split.Inner[1].Content + "> is already defined in class <" +
                                                containing.Name + ">.");
                                        }
                                    }
                                    else {
                                        throw new VTSException("SyntaxError", "firstPass::code",
                                            "In <" + this.File + ">, on line <" + lineno + 
                                            ">: Field within action is not allowed.");
                                    }
                                }
                                else {
                                    throw new VTSException("SyntaxError", "firstPass::code",
                                        "In <" + this.File + ">, on line <" + lineno + 
                                        ">: Field outside of class is not allowed.");
                                }
                            }
                            current.Contents.Add(vl);
                            currLine = "";
                        break;

                        default:
                            currLine += chr;
                        break;
                    }
                }
            });
            return data;
        }
    }
}