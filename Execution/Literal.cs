using Jay.VTS.Parser;
using Jay.VTS.Structures;
using Jay.Logging;

namespace Jay.VTS.Execution
{
    public class Literal
    {
        public static VTSVariable Parse(string Literal) {
            //unparsable, return null
            //parsable, return VTSVariable
            Logger.Log("Trying to parse literal " + Literal);
            return new VTSVariable();
        }
    }
}