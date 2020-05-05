using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Jay.VTS
{
    public class CodeSplitter
    {
        public string OGCode { get; }
        public string File { get; }

        public CodeSplitter(string code, string file)
        {
            OGCode = code;
            File = file;
        }

        public CodeBlock SplitCode()
        {
            CodeBlock data = new CodeBlock()
            {
                IsLine = false,
                File = File,
                Lineno = 0,
                Parent = null,
                Contents = new List<CodeBlock>(),
                Type = "file"
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
                            inner.Type = inner.Line.Split(' ')[0];
                            if(inner.Type.Length < 4) { inner.Type += "  "; }
                            current.Contents.Add(inner);
                            current = inner;
                            currLine = "";
                        break;

                        case '}':
                            if(current.Parent == null) { throw new VTSException("SyntaxError", "firstPass::code", 
                                "Unexpected '}' in <" + File + "> on line <" + lineno + ">"); }
                            current = current.Parent;
                            currLine = "";
                        break;

                        case ';':
                            current.Contents.Add(new CodeBlock(){
                                IsLine = true,
                                File = this.File,
                                Line = Regex.Replace(currLine.TrimStart(), @"\s+", " "),
                                Contents = null,
                                Lineno = lineno,
                                Type = "code"
                            });
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