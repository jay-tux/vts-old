using System;
using System.Collections.Generic;
using Jay.VTS;
using Jay.VTS.Parser;

namespace Jay.VTS.Structures
{
    public class VTSAction
    {
        public string Name;
        public CodeBlock Instructions;

        public static explicit operator VTSAction(CodeBlock code) => 
            new VTSAction() { Name = code.Split.Inner[1].Content, Instructions = code };
        public override string ToString() => "VTSAction::" + Name;
    }
}