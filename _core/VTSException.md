# VTS - Jay.VTS.VTSException
The VTSException class is mostly to represent errors in the VTS code to be parsed. Here is an overview of all ``public`` fields and methods.
## Fields
 - ``public new string StackTrace``;  
 Hides the inherited ``StackTrace`` variable to use the generated VTS-code stack trace.  
 - ``public string Type``;  
 The type of error which has occured.
 - ``public VTSException Cause``;  
 The inner exception which has cause this one. Usually ``null``.

## Methods
 - ``public VTSException(string Type, StackFrame Stack, string Message,  VTSException Cause)``;  
 Default in-code constructor. Creates a new VTSException with the given Type, stack trace, message and Cause.
 - ``public VTSException(string Type, string LoadMod, string Message)``;  
  VTSException Constructor for when the given VTS code is unparsable (in which case the stack trace consists of the module and line where the parser failed).
 - ``public static VTSException OperatorException(VTSVariable v1, VTSVariable v2, VTSOperator op, StackFrame frame)``;  
  Generates a VTSException saying that the given operator does not exist for the given types of variables.
 - ``public static VTSException ArgCountException(string Class, string Action, uint req, uint given, StackFrame frame)``;  
  Generates a VTSException saying that the amount of arguments passed to the given class's action is not correct.  
 - ``public static VTSException NullRef(string name, StackFrame frame)``;  
  Generates a VTSException saying that the given name does not exist in the current context.  
 - ``public static VTSException TypeException(VTSClass expected, VTSClass actual, StackFrame frame)``;  
  Generates a VTSException saying that the variable given is of the wrong type.
