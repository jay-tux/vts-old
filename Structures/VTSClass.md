# VTS - Jay.VTS.Structures.VTSClass
A class representing a single class in VTS code. Below is an overview of its ``public`` fiels and methods.

## Fields
 - ``public string Name``;  
 This class's name.
 - ``public Dictionary<string, string> Fields``; **Type should be List<string>. The value field is obsolete**  
 Contains the names of all fields of this class.
 - ``public Dictionary<string, VTSAction> Actions``;  
 Contains the actions for this class (includes the constructor).
 - ``public Dictionary<VTSOperator, VTSAction> Operators``;  
 Contains all overloaded operators for this class.
 - ``public Dictionary<string, Func<VTSVariable, List<VTSVariable>, StackFrame, VTSVariable>> Internals``;  
 Contains all internal actions for this class. Internal actions are actions executed straight from C#.
 - ``public VTSVariable TypeRef``;  
 A dummy variable acting as a type reference for constructor calls.

## Constructors
 - ``public VTSClass()``;  
 Creates a new VTSClass (only instantiates a non-mutable, field-less Type Refence).

## Methods
 - ``public VTSVariable Create(StackFrame frame, List<VTSVariable> args)``;  
 Creates a new instance of this VTSClass, and calls the constructor on it with the given arguments.
 - ``public bool Contains(string structure)``;  
 Returns true if this VTSClass constains either a field, an operator or an action with the given name.
 - ``public void PrintActions()``; **obsolete, Interpreter has a better overview**  
 Prints an overview of all actions to stdout.
 - ``public override string ToString()``;  
 Returns a string of the form ``VTSClass::<name>``, where ``<name>`` is this VTSClass's name.

## Operators
 - ``public static explicit operator VTSClass(CodeBlock code)``;  
 Generates a VTSClass from the given CodeBlock (sets its name and initializes empty dictionaries for the fields, operators and internals).
