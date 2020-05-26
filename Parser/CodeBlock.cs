using System;
using System.Collections.Generic;
using Jay.VTS;
using System.Linq;
using Jay.Xtend;

namespace Jay.VTS.Parser
{
    public class CodeBlock
    {
        public int Lineno { get; set; }
        public string File { get; set; }
        public string Line { get; set; }
        public List<CodeBlock> Contents { get; set; }
        public CodeBlock Parent { get; set; }
        public bool IsLine { get; set; }
        public string Type { get; set; }
        public LineElement Split { get; set; }

        public static explicit operator string(CodeBlock target) => target.ToString(0);

        public string ToString(uint depth){
            List<string> lines = new List<string>();
            if(depth == 0) { lines.Add($" --- Beginning of Code Block --- \n  [{File}]\n"); }
            string res = "";
            res += "[" + Lineno.ToString().PadLeft(4) + "]\t";
            for(int i = 0; i < depth; i++) { res += "  "; }
            res += Line + " <--> { " + Type + " } " +
                (Split != null ? Split.ToOneliner() : "[ null ]");
            lines.Add(res);
            if(Contents != null) {
                Contents.ForEach(line => lines.Add(line.ToString(depth + 1)));
            }
            return String.Join("\n", lines) + (depth == 0 ? "\n --- End of Code Block --- " : "");
        }

        public string ToParentString(uint depth) {
            List<string> lines = new List<string>();
            if(depth == 0) { lines.Add($" --- Beginning of Code Block --- \n [{File}]\n"); }
            string res = "";
            res += "[" + Lineno.ToString().PadLeft(4) + "]\t";
            for(int i = 0; i < depth; i++) { res += "  "; }
            res += Line + " --> Parent = " + (Parent == null ? "(null)" : $"'{Parent.Line}'");
            lines.Add(res);
            if(Contents != null) {
                Contents.ForEach(line => lines.Add(line.ToParentString(depth + 1)));
            }
            return String.Join("\n", lines) + (depth == 0 ? "\n --- End of Code Block --- " : "");
        }

        public List<LineElement> Slice(int index) => Split.Inner.Slice(index).ToList();
    }
}