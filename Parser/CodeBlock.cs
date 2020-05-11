using System;
using System.Collections.Generic;
using Jay.VTS;

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
            res += Line + " <--> " + (Split != null ? Split.ToOneliner() : "[ null ]");
            lines.Add(res);
            if(Contents != null) {
                Contents.ForEach(line => lines.Add(line.ToString(depth + 1)));
            }
            return String.Join("\n", lines) + (depth == 0 ? "\n --- End of Code Block --- " : "");
        }
    }
}