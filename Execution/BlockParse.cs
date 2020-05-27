using System.Collections.Generic;
using Jay.VTS.Structures;
using Jay.VTS.Execution;
using Jay.VTS.Parser;
using Jay.VTS.Enums;
using Jay.Xtend;
using Jay.Logging;
using System;

namespace Jay.VTS.Execution
{
    public class BlockParse
    {
        public static void ParseSingleBlock(StackFrame frame, CodeBlock block) {
            Logger.Log("Parsing block: ");
            Logger.Log(block.ToString(1));
            Expression parse = SplitExpression.Split(block.Split);

            /*if(block.Split.Inner.Count == 0) { return; }

            if(block.Split[0].Type == ElementType.Identifier) {
                if(block.Split.Inner.Count == 1) {
                    throw new VTSException("SyntaxError", frame, "<identifier>; is not a valid statement.", null);
                }
                else if(block.Split[1].Content == "=") {
                    //          ASSIGNMENT FORM <identifier> = <value/expression>
                    if(block.Split.Inner.Count == 2) {
                        throw new VTSException("SyntaxError", frame, "Unexpected ';', expected <value> or <identifier>", null);
                    }
                    //ASSIGNMENT
                        //check if class exists with name -> error
                        //check if action exists with name in class -> error
                        //check if global variable exists with name -> error
                        //check if variable exists with name in frame -> re-assign
                        //check if field exists with name in class -> re-assign
                        //else -> assign in frame
                    string varname = block.Split[0].Content;
                    if(Interpreter.Instance.ContainsClass(varname)) {
                        throw new VTSException("NameError", frame, $"Identifier <{varname}> is already defined as class.", null);
                    }
                    else if(frame.ParentClass != null && frame.ParentClass.Actions.ContainsKey(varname)) {
                        throw new VTSException("NameError", frame, $"Identifier <{varname}> is already defined as class-action.", null);
                    }
                    else if(CoreStructures.BuiltinVariables.ContainsKey(varname)) {
                        throw new VTSException("NameError", frame, $"Identifier <{varname}> is already defined as global constant.", null);
                    }
                    else {
                        //evaluate result
                        VTSVariable result = ParseExpression(frame, block, 2);

                        //(re-)assign variable
                        if(frame.ParentClass != null && frame.ParentClass.Fields.ContainsKey(varname)) {
                            //(re-)assign in class (this field in frame's vars)
                            frame.Variables["this"].Fields[varname] = result;
                        }
                        else {
                            //(re-)assign in frame
                            frame.Variables[varname] = result;
                        }
                    }
                }
            }*/
        }

        public static VTSVariable ParseExpression(StackFrame frame, CodeBlock block, int index) {
            Expression split = SplitExpression.Split(new LineElement() {
                Type = ElementType.Block,
                Content = "",
                Inner = (List<LineElement>)block.Slice(index),
                Parent = null
            });
            return CoreStructures.Void;
            /*Logger.Log("Trying to parse expression <" + block.Split[index].ToString(0) + ">");
            LineElement expr = new LineElement() {
                Type = ElementType.Block,
                Content = "",
                Inner = (List<LineElement>)block.Slice(index),
                Parent = null
            };
            if(expr.Inner.Count == 1) {
                switch(expr[0].Type) {
                    case ElementType.Identifier:
                        Logger.Log("Expression is Identifier");
                        if(frame.Variables.ContainsKey(expr[0].Content)) {
                            return frame.Variables[expr[0].Content];
                        }
                        else if(CoreStructures.BuiltinVariables.ContainsKey(expr[0].Content)) {
                            return CoreStructures.BuiltinVariables[expr[0].Content];
                        }
                        else {
                            throw new VTSException("NameError", frame, $"Identifier <{expr[0].Content}> unknown.", null);
                        }

                    case ElementType.Literal:
                        Logger.Log("Expression is Literal");
                        return Literal.Parse(expr[0].Content);

                    default:
                        Logger.Log("Expression is one-element type.");
                        break;
                }
            }
            else {
                Logger.Log("Expression is " + expr.Inner.Count + "-element type.");
            }
            return new VTSVariable();*/
        }
    }
}