using System;

public class StackFrame
{
    public StackFrame Parent;

    public static explicit operator string(StackFrame val) => "";
    public static explicit operator int(StackFrame val) => 0;
}