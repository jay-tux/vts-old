# VTS Developer Guide
## Full Namespace Overview
As VTS contains a large number of classes, each class will be described in its own file, as will each namespace. This file acts as a central hub to access each of those files:
 - ``namespace Jay.VTS`` (main namespace for all VTS-related classes)
   - ``namespace Jay.VTS.Execution`` (contains execution-related classes, like ``StackFrame``, ...)
   - ``namespace Jay.VTS.Parser`` (contains parsing-related classes, like ``Expression``, ...)
   - ``namespace Jay.VTS.Structures`` (contains builtin VTS structures, like ``VTSClass``, ...)
   - ``namespace Jay.VTS.Enums`` (contains an enum, but refactoring should move some enums there)
 - ``namespace Jay.Xtend`` (contains a few extension methods for builtin C# classes)
 - ``namespace Jay.Logging`` (contains a simple preprocessor-driven logger)

## Full Class Overview by Namespace
### Jay.VTS namespace
The ``Jay.VTS`` namespace is mostly used to group the other namespaces, but contains a few classes of its own as well:
 - ``class Program``, in the file ``/_core/Program.cs``;
   contains VTS's entry point, starts running the interpreter, avoids multiple inclusion, and sets the exit code, see [Program](../guide/_core/Program.md).
 - ``class Interpreter``, in the file ``/_core/Interpreter.cs``;  
   contains methods to start running the VTS tasks (split the code, import the required modules, include the required files, ...), and holds a ``Dictionary`` of all (loaded/included) classes, see [Interpreter](../guide/_core/Interpreter.md).
 - ``class VTSException``, in the file ``/_core/VTSException.cs``;   
   contains a generic error type for when the VTS code has errors, see [VTSException](../guide/_core/VTSException.md).
 - ``class ImportModule``, in the file ``/_core/ImportModule.cs``;  
   imports the required builtin modules, and loads the requested local files, see [ImportModule](../guide/_core/ImportModule.md).

### Jay.VTS.Execution namespace
 - ``class StackFrame``, in the file ``/Execution/StackFrame.cs``;  
   contains the required methods for the stackframe-driven system, as well as expression parsing, method calling and variables within a frame, see [StackFrame](../guide/Execution/StackFrame.md).
 - ``class FrameEventArgs``, in the file ``/Execution/FrameEventArgs.cs``;  
   contains some data for when a ``StackFrame`` returns (status, return value, ...), see [FrameEventArgs](../guide/Execution/FrameEventArgs.md).
 - ``class BlockParse``, in the file ``/Execution/BlockParse.cs``;  
   actually an obsolete class, which turns CodeBlocks into Expressions (see their respective overviews). Code in here should be moved to the StackFrame class, see [BlockParse](../guide/Execution/BlockParse.md).
 - ``class Literal``, in the file ``/Execution/Literal.cs``;  
   handles literal parsing, see [Literal](../guide/Execution/Literal.md).

### Jay.VTS.Parser namespace
Classes in this namespace can be grouped by two: one which contains the data, another which transforms code into the given data type.
 - ``class CodeBlock``, ``class CodeSplitter``, in ``/Parser/CodeBlock.cs``, ``/Parser/CodeSplitter.cs``;  
  these classes transform (strings) of code into CodeBlocks (by splitting on the ``;``, ``/* */``, ``()`` and ``{}`` characters); they also filter away comments and includes, see [CodeBlock and CodeSplitter](../guide/Parser/CodeBlock.md).
 - ``class LineElement``, ``class LineSplitter``, in ``/Parser/LineElement.cs``, ``/Parser/LineSplitter.cs``;  
   these classes transform code blocks into their (expression) parse (by splitting on the ``.``, `` ``, ``,``, ``()`` characters), see [LineElement and LineSplitter](../guide/Parser/LineElement.md).
 - ``class Expression``, ``class SplitExpression``, in ``/Parser/Expression.cs``, ``/Parser/SplitExpression.cs``;  
   these classes transform line element blocks (see LineElement's documentation) into postfix expressions, see [Expression and SplitExpression](../guide/Parser/Expression.md).

### Jay.VTS.Structures namespace
Contains acutally two kinds of classes: the actual structures VTS contains (classes, actions, operators) and the builtin structures and modules (int, string, float, list, HTTP, IO).
 - Actual VTS Structures; in the files ``/Structures/VTSAction.cs``, ``/Structures/VTSClass.cs``, ``/Structures/VTSOperator.cs`` and ``/Structures/VTSVariable.cs``. For more info, see (respectively) [VTSAction](../guide/Structures/VTSAction.md), [VTSClass](../guide/Structures/VTSClass.md), [VTSOperator](../guide/Structures/VTSOperator.md), [VTSVariable](../guide/Structures/VTSVariable.md)
 - Builtin Structures; in the file ``/Structures/CoreStructures.cs``, see [CoreStructures](../guide/Structures/Core.md).
 - Modules; in all other files. The builtin structures can be seen as the Core module, which is auto-imported, see the documentation on the modules, via the [Modules](../guide/Structures/Modules.md) hub.

### Jay.VTS.Enums namespace
Contains currently only one enum, but refactoring should move some other enums there, see [Enums](../guide/Enums.md).

### Jay.Xtend namespace
Contains some extension methods for the builtin types.
 - ``static class XtendPrimitive``, in ``/Xtend/XtendPrimitive.cs``;  
 Adds some methods to the C# primitives, see [XtendPrimitive](../guide/Xtend/XtendPrimitive.md).
 - ``static class XtendIEnumerable``, in ``/Xtend/XtendIEnumerable.cs``;  
 Adds some methods to the C# ``IEnumerable`` type, see [XtendIEnumerable](../guide/Xtend/XtendIEnumerable.md).

### Jay.Logging namespace
 - ``public enum LogType``, in ``/Logger.cs``;  
 Defines the logging types; should be moved to ``/Enums.cs``, see [Logging](../guide/Logging.md).
 - ``public class Logger``, in ``/Logger.cs``;  
 The preprocessor-driven logger, see [Logging](../guide/Logging.md).
