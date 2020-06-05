# VTS - Jay.VTS.Structures.VTSOperator
A class with exactly 11 instances to represent the operators existing in the VTS language. Below is an overview:

## Fields
 - ``public string Name``;  
 Read-only field containing the operator's name.
 - ``public string ActionName``;
 Read-only field containing the operator's action name (in VTS code).
 - ``public string Operator``;  
 Read-only field containing the operator sign.

## Static Fields
 - ``public static string[] _lgl``; **Should become private**  
 All legal operator action names.
 - ``public static string[] _ops``; **Should become private**  
 All legal operator signs.

## Methods
 - ``public static bool IsOperator(string name)``;  
 IS the name a legal action name?
 - ``public int CompareTo(Object obj)``;  
 Comparator for use in ``VTSOperator > VTSOperator``. Compares using the precedence of the operators.
 - ``public override string ToString()``;  
 Returns a string representation of the VTSOperator of the form ``<operator>[<name>]``, where ``<operator>`` is the operator sign and ``<name>`` is the operator's name.

## Operators
 - ``public static explicit operator VTSOperator(string name)``;  
 Attempts to map the name onto an existing operator with that name.
 - ``public static explicit operator VTSOperator(LineElement elem)``;  
 Attempts to map the operator sign in the LineElement onto an existing operator with that name.
 - The VTSOperator class overloads the ``>``, ``<``, ``>=`` and ``<=`` operators, comparing their precedence.

## Instances
Please bear in mind that the assignment operator (``=``) can't be overloaded.
| Static Field Name | Operator Sign | Action Name | Precedence |
|-------------------|---------------|-------------|------------|
| ADD               | +             | ``_add``    | 4          |
| SUBTRACT          | -             | ``_sub``    | 4          |
| MULTIPLY          | *             | ``_mul``    | 5          |
| DIVIDE            | /             | ``_div``    | 5          |
| MODULUS           | %             | ``_mod``    | 3          |
| EQUALS            | ==            | ``_eql``    | 2          |
| LARGER            | >             | ``_lrg``    | 2          |
| SMALLER           | <             | ``_sml``    | 2          |
| ASSIGN            | =             | ``_asg``    | 0          |
| OR                | \|            | ``_dis``    | 1          |
| AND               | &             | ``_con``    | 1          |
