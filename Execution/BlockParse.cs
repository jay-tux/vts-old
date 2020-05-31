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
        public static Expression ParseSingleBlock(StackFrame frame, CodeBlock block) {
            try {
                Logger.Log("Parsing block: ");
                Logger.Log(block.ToString(1));
                Expression parse = SplitExpression.ToPostFix(block.Split);
                parse = SplitExpression.Split((LineElement)parse, out uint _);
                Logger.Log("==== RESULT ====");
                Logger.Log(parse);
                return parse;
            }
            catch(VTSException vtse) {
                frame.Crash(new FrameEventArgs() {
                    ExitCode = FrameEventArgs.Exits.CodeException,
                    Error = vtse
                });
                return null;
            }
            catch(Exception e) {
                frame.Crash(new FrameEventArgs(){
                    ExitCode = FrameEventArgs.Exits.InternalException,
                    InternalError = e.Message
                });
                return null;
            }
       }
    }
}