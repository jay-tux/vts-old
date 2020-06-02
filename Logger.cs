#if VERBOSE
using System;
#endif

namespace Jay.Logging
{

    public enum LogType { EXECUTION, DEBUG, MESSAGE, MODULE, PARSING, WARNING, STRUTURAL, NOIGNORE }
    public class Logger
    {
        #if VERBOSE
        public static bool Enabled = true;
        #endif

        /*public static void Log(string message){
            #if VERBOSE
            if(Enabled) {
                Console.WriteLine(message);
            }
            #endif
        }*/

        public static void Log(string message, LogType type) {
            #if VERBOSE
            switch(type)
            {
		#if EXECUTION
		case LogType.EXECUTION:
		    Console.WriteLine(message);
		    break;
		#endif
                #if DEBUG
                case LogType.DEBUG:
                    Console.WriteLine(message);
                    break;
                #endif

                #if MESSAGE
                case LogType.MESSAGE:
                    Console.WriteLine(message);
                    break;
                #endif

                #if MODULE
                case LogType.MODULE:
                    Console.WriteLine(message);
                    break;
                #endif

                #if PARSING
                case LogType.PARSING:
                    Console.WriteLine(message);
                    break;
                #endif

                #if WARNING
                case LogType.WARNING:
                    Console.WriteLine(message);
                    break;
                #endif

                #if STRUTURAL
                case LogType.STRUTURAL:
                    Console.WriteLine(message);
                    break;
                #endif
            }
            #endif
        }

        //public static void Log(object message) => Log(message.ToString());
        public static void Log(object message, LogType type) => Log(message.ToString(), type);
    }
}
