# VTS - Jay.VTS.Structures.VTSVariable
A class representing a single instance of a variable. Below is an overview of all ``public`` fields and methods.

## Fields
 - ``public VTSClass Class``;  
 The VTSClass of which this VTSVariable is an instance.
 - ``public bool Mutable``;  
 Are the fields of this VTSVariable mutable?
 - ``public bool IsTypeRef``;  
 Is this a Type Reference?
 - ``public string Name``;  
 The name of this VTSVariable.

## Methods
 - ``public static VTSVariable UNDEFINED(string name)``;  
 Creates a new undefined VTSVariable with the given name.
 - ``public override string ToString()``;  
 Returns the basic string representation of this VTSVariable: ``~><type>``, where ``<type>`` is its Class's name or ``(typeless)`` it it's undefined.
 - ``public string ToString(StackFrame caller)``; **to be modified**  
 Returns the string representation of this VTSVariable (replace ``<type>`` with its class's name):
   - If it doesn't have a class: ``~> (typeless)``.
   - If its class doesn't have any actions: ``~> (<type>:actionless)``.
   - If its class doesn't have any internal actions: ``~> (<type>:internal-less)``
   - If its class is ``CoreStructures.VTSString``: its ``value`` field.
   - If its class contains a ``toString`` action or internal: the return value of its ``toString`` action.
   - Else: ``~> <type>``.
 - ``public VTSVariable Call(LineElement action, StackFrame frame, VTSVariable other)``;  
 Attempts to call an operator on this VTSVariable; the operator is given in the LineElement.
 - ``public VTSVariable Call(string action, StackFrame frame, List<VTSVariable> args)``;  
 Attempts to call an action on this VTSVariable.
