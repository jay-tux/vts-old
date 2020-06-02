using Jay.VTS.Parser;
using Jay.VTS.Structures;
using Jay.Logging;
using System.Collections.Generic;

namespace Jay.VTS.Execution
{
    public class Literal
    {
        public static VTSVariable Parse(string Literal) {
            Logger.Log("Trying to parse literal " + Literal);
            //literal true; return ref to constant true
            if(Literal == "true") return CoreStructures.True();
            //literal false; return ref to constant false
            else if(Literal == "false") return CoreStructures.False();
            //maybe numeric literal: try parse as float
            else if(float.TryParse(Literal, out float floatval)) {
                //contains '.' -> is float
                if(Literal.Contains(".")) {
                    return new VTSVariable() {
                        Class = CoreStructures.VTSFloat, Mutable = false, Fields = new Dictionary<string, object>() {
                            ["value"] = floatval
                        }, Name = "_litFloat"
                    };
                }
                //is int
                else {
                    return new VTSVariable() {
                        Class = CoreStructures.VTSInt, Mutable = false, Fields = new Dictionary<string, object>() {
                            ["value"] = (int)floatval
                        }, Name = "_litInt"
                    };
                }
            }
            //string literal; return as ref
            else return new VTSVariable() {
                Class = CoreStructures.VTSString, Mutable = false, Fields = new Dictionary<string, object>() {
                    ["value"] = Literal.Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\r", "\r")
                }, Name = "_litString"
            };
        }
    }
}