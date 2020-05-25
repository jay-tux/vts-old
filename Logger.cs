#if VERBOSE
    #if !ONLYFILELOG
using System;
    #endif
    #if FILELOG
using System.IO;
    #endif
#endif

namespace Jay.Logging
{
    public class Logger
    {
        private static string File = "~/debug.log";

        public static void Log(string message){
            #if VERBOSE
                #if FILELOG
            File.AppendAllLines(message);
                #endif
                #if !ONLYFILELOG
            Console.WriteLine(message);
                #endif
            #endif
        }
    }
}