# VTS - Jay.VTS.Parser.Expression and Jay.VTS.Parser.SplitExpression
A pair of classes for representing postfix expressions.
## Expression Class
### Fields
 - ``public bool IsBlock``;  
 Is this a block-type expression? (An expression which contains other expressions).
 - ``public LineElement Content``;  
 The content of this (non-block) Expression.
 - ``public List<Expression> Block``;  
 The content of this (block) Expression.
 - ``public bool IsCall``;  
 Is this Expression field an action call?
 - ``public uint ArgCount``;  
 The amount of arguments this (call) Expression requires.
 - ``public uint Count``;  
 If this is a block Expression, determines how much arguments are required, depending on the block's contents.

### Methods
 - ``public override string ToString``;  
 Converts this Expression to its string representation. The representation depends on the kind of Expression this is:
   - if this is a block Expression, return ``[ <contents> ]``, where contents are the string representations of this Expression's contents, separated by ``, ``.
   - if this is a call Expression, depends on the values in the ``Content`` field:
     - if ``Content`` is ``null``, returns ``{ Call:null }``
     - otherwise, returns ``{ Call:<content>; <argcount> args }``, where ``<content>`` is this Expression's Content field converted to a one-line string and ``<argcount>`` is this Expression's ArgCount field.
   - if this Expression's content is not null, and this Expression represents an operator call, returns ``{ Oper:<operator> }``, where ``<operator>`` is the string representation of this Expression's operator.
   - if this Expression's conten is not null, returns ``{ <content> }``, where ``<content>`` is the one-line string representation of this Expression's content.
   - else, returns ``{ null }``.

### Operators
 - ``public static explicit operator LineElement(Expression target)``;  
 Attempts to convert this Expression to a LineElement. If this is a block Expression, returns a block LineElement which is filled recursively. Otherwise, returns this Expression's content.

## OperatorExpression Class
**This class is obsolete and should be removed**.

## SplitExpression Algorithm
This algorithm works in two passes:
### First Pass: Operator Pass
Uses a stack to convert all operators in the LineElement to a postfix expression, but doesn't modify all other parts, below is the pseudocode implementation:
 - Create an empty operator stack
 - Create an empty postfix result Expression
 - For each element x in the block LineElement, do:
   - If x is a block (between brackets): parse recursively and append result as block;
   - If x is a separator: pop all operators on the stack;
   - If x is an operator: pop all operators with higher precedence, then push this operator;
   - Else: add to postfix list
 - Pop all resulting operators to the Expression

### Second Pass: Call Pass
Uses a stack to convert all calls to postfix (each call is an n-ary operator), below is the pseudocode implementation:
 - Set the current Argument count to 1
 - Create an empty call stack
 - Create an empty postfix result Expression
 - Set the pop flag to false
 - For each element x in the block Expression, do:
   - If x is a block (between brackets): parse recursively, saving the argument count, and append the result as a block
     - If the pop flag is true, flip it to false and pop one call from the stack.
   - If x is nothing (Void): skip x
   - If x is a Member call: push onto the stack, set pop flag to true
   - If x is a Separator: increment the Argument count
   - Else: x is an identifier, add x to the result.
 - If there are no elements in the Expression's block, set the Argument count to 0.
