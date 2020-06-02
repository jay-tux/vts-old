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
        public static Expression Split(LineElement Target, out uint ArgCount, uint offset = 0) 
        {
            ArgCount = 1;
            Logger.Log("Converting non-operator call-chain to postfix: " + Target.ToOneliner(), LogType.PARSING);
            List<LineElement> infix = Target.Inner;
            Stack<LineElement> holder = new Stack<LineElement>();
            Expression result = new Expression()
            { IsBlock = true, Block = new List<Expression>() };
            bool pop = false;

            int index = 0;
            while(index < infix.Count) {
                LineElement current = infix[index];
                Logger.Log("Encountered " + current.ToOneliner(), LogType.DEBUG);

                if(current.Type == ElementType.Block) { 
                    Logger.Log("  -> Encountered Block.", LogType.DEBUG);
                    Logger.Log("     -> " + current.ToOneliner(), LogType.DEBUG);
                    Expression split = Split(current, out uint argcnt, offset + 1);
                    result.Block.AddRange(split.Block);
                    if(pop.CheckFlip()) { 
                        result.Block.Add(new Expression() { 
                            IsCall = true, Content = holder.Pop(), ArgCount = argcnt
                        }); 
                    }
                    //Add block as series of args
                    //Add previous caller as "operator"
                    Logger.Log("  -> Finished parsing Block.", LogType.DEBUG);
                }
                else if(current.Type == ElementType.Void) {
                    Logger.Log("  -> Is Void. Ignoring.", LogType.DEBUG);
                    //Ignore voids.
                }
                else if(current.Type == ElementType.Member) {
                    Logger.Log("  -> Is Call.", LogType.DEBUG);
                    holder.Push(current);
                    pop = true;
                    //Is call, prepare for block
                }
                else if(current.Type == ElementType.Separator) {
                    Logger.Log("  -> Is separator", LogType.DEBUG);
                    ArgCount++;
                    //separator, skip
                }
                else {
                    Logger.Log("  -> Is probably object ref.", LogType.DEBUG);
                    result.Block.Add(new Expression() { 
                        IsCall = false, IsBlock = false, Content = current
                    });
                    //Add as obj reference
                }
                index++;
            }
            if(result.Block.Count == 0) ArgCount = 0;
            Logger.Log("Result: " + result.ToString(), LogType.PARSING);
            return result;
        }

        public static Expression ToPostFix(LineElement exp)
        {
            Logger.Log(" -> Converting to Postfix: " + exp.ToOneliner(), LogType.PARSING);
            List<LineElement> infix = exp.Inner;
            Stack<LineElement> holder = new Stack<LineElement>();
            Expression result = new Expression() 
            { IsBlock = true, Block = new List<Expression>() };

            int index = 0;
            while(index < infix.Count) {
                Logger.Log("Encountered " + infix[index].ToOneliner(), LogType.DEBUG);
                if(infix[index].Type != ElementType.Block && infix[index].Type != ElementType.Operator
                    && infix[index].Type != ElementType.Separator) {
                    result.Block.Add(new Expression() {
                        IsBlock = false, Content = infix[index]
                    });
                    /*result.Elements.Add(new OperatorExpression.OpExpElem()
                    { IsOperator = false, Representation = infix[index].Content });*/
                    Logger.Log("Added Non-operator.", LogType.DEBUG);
                }
                else if(infix[index].Type == ElementType.Block) {
                    Logger.Log("Encoutered Range", LogType.DEBUG);
                    //Expression parsed = ToPostFix(infix[index]);
                    //result.Block.AddRange(parsed.Block);
                    result.Block.Add(new Expression() { IsBlock = true, Block = ToPostFix(infix[index]).Block });
                    Logger.Log("Added Range", LogType.DEBUG);
                }
                else if(infix[index].Type == ElementType.Separator) {
                    Logger.Log("Encountered Comma; popping whole stack.", LogType.DEBUG);
                    while(holder.Count > 0) {
                        result.Block.Add(new Expression() { 
                            IsBlock = false, Content = holder.Pop()
                        });
                    }
                    result.Block.Add(new Expression() { IsBlock = false, Content = infix[index] });
                }
                else if(infix[index].Type == ElementType.Operator) {
                    VTSOperator curr = (VTSOperator)infix[index];
                    while(holder.Count > 0 && (VTSOperator)holder.Peek() >= curr) 
                    { 
                        result.Block.Add(new Expression() {
                            IsBlock = false, Content = holder.Pop()
                        });
                        /*result.Elements.Add(new OperatorExpression.OpExpElem() 
                        { IsOperator = true, Representation = holder.Pop().Content }); */
                        Logger.Log("Added Operator to Postfix.", LogType.DEBUG);
                    }
                    holder.Push(infix[index]);
                }
                index++;
            }
            while(holder.Count > 0)
                result.Block.Add(new Expression() { IsBlock = false, Content = holder.Pop() });
                //result.Elements.Add(new OperatorExpression.OpExpElem() { IsOperator = true, Representation = holder.Pop().Content });
            Logger.Log("Postfix result: " + result, LogType.PARSING);
            return result;
        }
    }
}