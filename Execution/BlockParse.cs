using Jay.VTS.Structures;
using Jay.VTS.Execution;
using Jay.VTS.Parser;
using Jay.VTS.Enums;
using Jay.Logging;
using System;

namespace Jay.VTS.Execution
{
    public class BlockParse
    {
        public static void ParseSingleBlock(StackFrame frame, CodeBlock block) {
            Logger.Log("Parsing block: ");
            Logger.Log(block.ToString(1));

            if(block.Split.Inner.Count == 0) { return; }

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
                    string varname = block.Split[2].Content;
                    if(Interpreter.Instance.ContainsClass(varname)) {
                        throw new VTSException("NameError", frame, "Identifier is already defined as class.", null);
                    }
                    else if(frame.ParentClass != null && frame.ParentClass.Actions.ContainsKey(varname)) {
                        throw new VTSException("NameError", frame, "Identifier is already defined as class-action.", null);
                    }
                    else if(CoreStructures.BuiltinVariables.ContainsKey(varname)) {
                        throw new VTSException("NameError", frame, "Identifier is already defined as global constant.", null);
                    }
                    else {
                        //evaluate result
                        VTSVariable result = new VTSVariable();

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
            }
        }
    }
}