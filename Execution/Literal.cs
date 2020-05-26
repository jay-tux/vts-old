using Jay.VTS.Parser;
using Jay.VTS.Structures;
using Jay.Logging;

namespace Jay.VTS.Execution
{
    public class Literal
    {
        public static VTSVariable Parse(string Literal) {
            Logger.Log("Trying to parse literal " + Literal);
            return new VTSVariable();
        }
    }
}