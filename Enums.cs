using System;

namespace Jay.VTS.Enums
{
    [Flags]
    public enum ElementType { 
        Block       = 1 << 0,

        None        = 1 << 1,
        Void        = 1 << 2,
        Preproc     = 1 << 3,

        Class       = 1 << 4,
        Action      = 1 << 5,
        Control     = 1 << 6,
        Return      = 1 << 7,
        Member      = 1 << 8,
        Field       = 1 << 9,
        Separator   = 1 << 10,

        Identifier  = 1 << 11,
        Literal     = 1 << 12,
        
        Operator    = 1 << 13,

        Comment     = 1 << 14
    }
}