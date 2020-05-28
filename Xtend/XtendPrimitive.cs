using System.Linq;

namespace Jay.Xtend
{
    public static class XtendPrimitive
    {
        public static bool CheckFlip(this bool target) {
            if(target) {
                target = false;
                return true;
            }
            return false;
        }
    }
}