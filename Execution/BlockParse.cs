using System.Collections.Generic;
using Jay.VTS.Structures;
using Jay.VTS.Execution;
using Jay.VTS.Parser;
using Jay.VTS.Enums;
using Jay.Xtend;
using Jay.Logging;
using Jay.VTS;
using System;

namespace Jay.VTS.Execution
{
    public class BlockParse
    {
        public static void ParseSingleBlock(StackFrame frame, CodeBlock block) {
            try {
                Logger.Log("Parsing block: ");
                Logger.Log(block.ToString(1));
                #if VERBOSE
                //Logger.Enabled = false;
                #endif
                Expression parse = SplitExpression.ToPostFix(block.Split);
                parse = SplitExpression.Split((LineElement)parse, out uint _);
                #if VERBOSE
                //Logger.Enabled = true;
                #endif
                Logger.Log("==== RESULT ====");
                Logger.Log(parse);
            }
            catch(VTSException vtse) {
                throw new VTSException("RuntimeError", frame, "Something went wrong while parsing an expression.", vtse);
            }
            catch(Exception) {}
       }
    }
}