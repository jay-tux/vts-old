using System;
using Jay.VTS;
using Jay.VTS.Enums;
using System.Collections.Generic;

namespace Jay.VTS.Parser
{
    public class LineElement
    {
        public ElementType Type;
        public string Content;
        public List<LineElement> Inner;
        public LineElement Parent;

        public LineElement this[int index] => Inner[index];

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
}