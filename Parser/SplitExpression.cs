using System;
using Jay.VTS.Enums;
using System.Linq;
using Jay.Logging;
using Jay.Xtend;
using Jay.VTS.Structures;
using System.Collections.Generic;

namespace Jay.VTS.Parser
{
    public class SplitExpression
    {
        public static Expression Split(LineElement Target, uint offset = 0) 
        {
            Logger.Log(OffString(offset) + "Converting non-operator call-chain to postfix: " + Target.ToOneliner());
            List<LineElement> infix = Target.Inner;
            Stack<LineElement> holder = new Stack<LineElement>();
            Expression result = new Expression()
            { IsBlock = true, Block = new List<Expression>() };
            bool pop = false;

            int index = 0;
            while(index < infix.Count) {
                LineElement current = infix[index];
                Logger.Log(OffString(offset) + "Encountered " + current.ToOneliner());

                if(current.Type == ElementType.Block) { 
                    Logger.Log(OffString(offset) + "  -> Encountered Block.");
                    Logger.Log(OffString(offset) + "     -> " + current.ToOneliner());
                    Expression split = Split(current, offset + 1);
                    result.Block.AddRange(split.Block);
                    if(pop.CheckFlip()) { 
                        result.Block.Add(new Expression() { 
                            IsCall = true, Content = holder.Pop().Content, ArgCount = (uint)split.Count 
                        }); 
                    }
                    //Add block as series of args
                    //Add previous caller as "operator"
                    Logger.Log(OffString(offset) + "  -> Finished parsing Block.");
                }
                else if(current.Type == ElementType.Void) {
                    Logger.Log(OffString(offset) + "  -> Is Void. Ignoring.");
                    //Ignore voids.
                }
                else if(current.Type == ElementType.Member) {
                    Logger.Log(OffString(offset) + "  -> Is Call.");
                    holder.Push(current);
                    pop = true;
                    //Is call, prepare for block
                }
                else if(current.Type == ElementType.Operator) {
                    Logger.Log(OffString(offset) + "   -> Is operator???");
                }
                else if(current.Type == ElementType.Separator) {
                    Logger.Log(OffString(offset) + "  -> Is separator");
                    //separator, skip
                }
                else {
                    Logger.Log(OffString(offset) + "  -> Is probably object ref.");
                    result.Block.Add(new Expression() { 
                        IsCall = false, IsBlock = false, Content = current.Content
                    });
                    //Add as obj reference
                }
                index++;
            }
            Logger.Log(OffString(offset) + "Result: " + result.ToString());
            //ToPostFix(Target);
            return result;
        }

        private static string OffString(uint count) {
            string res = "";
            for(int i = 0; i < 2 * count; i++) { res += " "; }
            return res;
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