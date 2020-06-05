# VTS - Jay.VTS.Structures.VTSAction
A class representing a single action within a VTSClass. Below is an overview of all ``public`` fields and methods.

## Fields
 - ``public string Name``;  
 The action's name in code.
 - ``public CodeBlock Instructions``;  
 The instructions associated with this VTSAction.
 - ``public StackFrame Parent``;  
 The StackFrame calling this VTSAction.
 - ``public event EventHandler<FrameEventArgs> ActionReturns``;  
 Passthrough event.
 - ``public VTSVariable Result``;  
 The return value of this VTSAction.
 - ``public List<string> ArgNames``;  
 The names of the arguments as defined in the action header.
 - ``public bool IsInternalCall``;  
 Is this a call to internal C# code? (only in the case of builtin operators).
 - ``public Func<VTSVariable, VTSVariable, StackFrame, VTSVariable> InternalCall``;  
 The internal call.

## Methods
 - ``public void Execute(StackFrame parent)`` **obsolete; use ``VTSAction.Execute(StackFrame parent, VTSVariable caller)`` instead**.
 - ``public void Execute(StackFrame parent, VTSVariable caller)``;  
 Creates a new StackFrame in which this VTSAction is executed and executes that StackFrame.
 - ``public override string ToString``;  
 Converts this VTSAction to its string representation, in the case of an internal call, this is ``VTSAction::<name> [__internal call__]``, otherwise it's ``VTSAction::<name> [<argcount> args]`` (where name is the value in the Name field and argcount is the size of the ArgName list).

## Operators
 - ``public static explicit operator VTSAction(CodeBlock code)``;  
 Converts a CodeBlock into a VTSAction (sets the name, arguments, instructions).
