using System;
using Jay.VTS.Enums;
using System.Linq;
using System.Collections.Generic;
using Jay.VTS.Structures;

namespace Jay.VTS.Parser
{
    public class Expression
    {
        public bool IsBlock;
        public LineElement Content;
        public List<Expression> Block;
        public bool IsCall;
        public uint ArgCount;
        public uint Count {
            get => (uint)Block.Select(x => x.IsCall ? -ArgCount : 1).Sum();
        }

        public override string ToString() => 
            IsBlock ? ("[ " + string.Join(", ", Block.Select(x => x.ToString())) + " ] ") : 
            IsCall ? ("{ Call:" + (Content == null ? "null" : Content.ToOneliner()) + "; " + ArgCount + " args }" ) : 
            (Content != null && Content.Type == ElementType.Operator) ? 
                ("{ Oper:" + ((VTSOperator)Content).ToString() + " }") :
                ("{ " + (Content == null ? "null" : Content.ToOneliner()) + " }");
        
        public static explicit operator LineElement(Expression target)
        {
            if(target.IsBlock) {
                LineElement result = new LineElement() { 
                    Type = ElementType.Block, 
                    Inner = new List<LineElement>(), 
                    Content = "" 
                };
                foreach(Expression e in target.Block) {
                    result.Inner.Add((LineElement)e);
                }
                return result;
            }
            else {
                return target.Content;
            }
        }
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