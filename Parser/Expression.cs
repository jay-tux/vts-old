using System;
using System.Linq;
using System.Collections.Generic;

namespace Jay.VTS.Parser
{
    public class Expression
    {
        public string Content;
        public bool IsBlock;
        public List<Expression> Block;
        public bool IsCall;
        public uint ArgCount;

        public override string ToString() => 
            IsBlock ? ("[ " + string.Join(", ", Block.Select(x => x.ToString())) + "] ") : 
            IsCall ? ("{ Call:" + Content + "; " + ArgCount + " args }" ) : ("{ " + Content + " }");
    }

    public class OperatorExpression : Expression
    {
        public List<OpExpElem> Elements;

        public OperatorExpression() { IsBlock = false; Block = null; }

        public override string ToString() => "[ " + string.Join(" ", Elements.Select(x => x.Representation)) + " ]";

        public class OpExpElem
        {
            public bool IsOperator;
            public string Representation;
        }
    }
}