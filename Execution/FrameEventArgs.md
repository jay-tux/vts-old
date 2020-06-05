# VTS - Jay.VTS.Execution.FrameEventArgs
A class representing how a StackFrame finished its execution. Below is an overview of its ``public`` fields and methods.

## Enums
 - ``public enum Exits``; **refactoring should move this to Jay.VTS.Enums**.  
 The type of exit the StackFrame had: ``EOF, ReturnValue, Return, InternalException, CodeException`` (representing, respectively, end-of-code, return with a value, return with Void, a C# exception occurred, a VTSException occurred).
## Fields
 - ``public Exits ExitCode``;  
 The exit type of the StackFrame.
 - ``public VTSException Error``;  
 The error which occurred in the StackFrame (in case of a ``Exits.CodeException`` return).
 - ``public string InternalError``;  
 The message of the C# error which occurred in the StackFrame (in case of a ``Exits.InternalException`` return).
 - ``public VTSVariable ReturnValue``;  
 The StackFrame's return value (in case of a ``Exits.ReturnValue`` return).
 - ``public StackFrame Frame``;  
 The StackFrame which returned.

## Constructors
 - ``public FrameEventArgs()`` **obsolete/empty constructor.**

## Methods
 - ``public FrameEventArgs SetExitCode(Exits Code)``; **obsolete, use object initializer instead**  
 Sets the exit type. Chainable method.
 - ``public FrameEventArgs SetError(VTSException Error)``; **obsolete, use object initializer instead**  
 Sets the VTSException which occurred in the StackFrame. Chainable method.
 - ``public FrameEventArgs SetInternal(string Internal)``; **obsolete, use object initializer instead**  
 Sets the internal exception message. Chainable method.
