namespace Jay.VTS.Enums
{
    public enum ElementType { 
        Block = -0x10,

        None = 0x00,
        Void,

        Class = 0x10,
        Action,
        Control,
        Return,
        Member,

        Identifier = 0x20,
        Literal,
        
        Operator = 0x30,
        
        Comment = 0x40
    }
}