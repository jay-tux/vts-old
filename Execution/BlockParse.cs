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
        public static Expression ParseSingleBlock(StackFrame frame, LineElement block) {
            try {
                Logger.Log("Parsing block: ", LogType.PARSING);
                Logger.Log(block.ToOneliner(), LogType.PARSING);
                Expression parse = SplitExpression.ToPostFix(block);
                parse = SplitExpression.Split((LineElement)parse, out uint _);
                Logger.Log("==== RESULT ====", LogType.PARSING);
                Logger.Log(parse, LogType.PARSING);
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
                    InternalError = e.Message,
                    Frame = frame
                });
                return null;
            }
        }
        public static Expression ParseSingleBlock(StackFrame frame, CodeBlock block) 
            => ParseSingleBlock(frame, block.Split);
    }
}