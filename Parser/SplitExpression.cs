using System;
using Jay.VTS.Enums;
using System.Linq;
using Jay.Logging;
using Jay.VTS.Structures;
using System.Collections.Generic;

namespace Jay.VTS.Parser
{
    public class SplitExpression
    {
        public static Expression Split(LineElement Target) 
        {
            Logger.Log("Trying to split " + Target.ToOneliner());
            ToPostFix(Target);
            return new Expression();
        }

        public static OperatorExpression ToPostFix(LineElement exp)
        {
            Logger.Log(" -> Converting to Postfix: " + exp.ToOneliner());
            List<LineElement> infix = exp.Inner;
            Stack<LineElement> holder = new Stack<LineElement>();
            OperatorExpression result = new OperatorExpression() 
            { Elements = new List<OperatorExpression.OpExpElem>() };

            int index = 0;
            while(index < infix.Count) {
                Logger.Log("Encountered " + infix[index].ToOneliner());
                if(infix[index].Type != ElementType.Block && infix[index].Type != ElementType.Operator) {
                    result.Elements.Add(new OperatorExpression.OpExpElem()
                    { IsOperator = false, Representation = infix[index].Content });
                    Logger.Log("Added Non-operator.");
                }
                else if(infix[index].Type == ElementType.Block) {
                    Logger.Log("Encoutered Range");
                    OperatorExpression parsed = ToPostFix(infix[index]);
                    result.Elements.AddRange(parsed.Elements);
                    Logger.Log("Added Range");
                }
                else if(infix[index].Type == ElementType.Operator) {
                    VTSOperator curr = (VTSOperator)infix[index];
                    while(holder.Count > 0 && (VTSOperator)holder.Peek() >= curr) 
                    { 
                        result.Elements.Add(new OperatorExpression.OpExpElem() 
                        { IsOperator = true, Representation = holder.Pop().Content }); 
                        Logger.Log("Added Operator to Postfix.");
                    }
                    holder.Push(infix[index]);
                }
                index++;
            }
            while(holder.Count > 0)
                result.Elements.Add(new OperatorExpression.OpExpElem() { IsOperator = true, Representation = holder.Pop().Content });
            Logger.Log("Postfix result: " + result);
            return result;
        }
    }
}