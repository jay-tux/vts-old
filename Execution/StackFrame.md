# VTS - Jay.VTS.Execution.StackFrame
A class representing the work done inside a single execution stack frame. Below is an overview of the ``public`` fields and methods.

## Fields
 - ``public StackFrame Parent``;  
 This StackFrame's parent (or caller frame).
 - ``public CodeBlock Root``;  
 The container of this StackFrame's code.
 - ``public CodeBlock Pointer``;  
 A pointer to the current CodeBlock to execute.
 - ``public event EventHandler<FrameEventArgs> StackFrameReturns``;  
 An event which is fired when a StackFrame is done executing (either by errors, return statements or end-of-code).
 - ``public Dictionary<string, VTSVariable>``;  
 Contains all variables in scope with their identifiers.
 - ``public VTSClass ParentClass``;  
 The class from which this StackFrame is started. Used when working with member actions.
 - ``public bool IsCopyFrame``;  
 Is this a copy frame? (A copy frame is a StackFrame which inherits its parent's variables)

## Constructor
 - ``public StackFrame(CodeBlock Root, int EntryIndex)``;  
 Creates a new StackFrame from the given code. Execution will start at the ``EntryIndex``-th CodeBlock in ``Root``'s block list.

## Methods
 - ``public static void Overview()``; **obsolete method, use ``Interpreter.Instance.PrintAll()`` instead.**
 - ``public static StackFrame FindEntry(CodeBlock master)``;  
 Searches master for the program's entry point recursively and returns the StackFrame associated with the entry point.
 - ``public void Crash(FrameEventArgs eventArgs)``;  
 Crashes this CodeBlock. Can be used to return as well.
 - ``public void AddHandler(EventHandler<FrameEventArgs> handler)``; **obsolete method, use ``StackFrame.StackFrameReturns += handler`` instead.**
 - ``public bool HasVar(string name)``;  
 Returns ``true`` if this StackFrame holds a variable with the given name, if this StackFrame is a copy frame and ``Parent.HasVar(name)`` returns ``true``, or if ``name`` refers to one of the core variables.
 - ``public VTSVariable GetVariable(string name)``;  
 Attempts to get the VTSVariable associated with ``name`` (using the same techniques as ``HasVar``).
 - ``public void SetVariable(string name, VTSVariable newValue)``;  
 Attempts to set the VTSVariable associated with ``name`` (using the same techniques as ``HasVar``).
 - ``public void Execute()``; *see Execution Algorithm*.
 - ``public void PrintScope()``;  
 Prints all variables in scope using the ``STRUTURAL`` and ``WARNING`` log types.

## Operators
 - ``public static explicit operator string(StackFrame val)``;  
 Returns the file in which the code in ``val`` is found (for stack trace purposes).
 - ``public static explicit operator int(StackFrame val)``;  
 Returns the number of the line to which ``val.Pointer`` points (for stack trace purposes).

## Execution Algorithm
The execution algorithm consists of three pieces: the main loop (``public void Execute()``), the control calls (``private void ControlCall(CodeBlock block)``) and the expression evaluation (``private void RunExpression(Expression e)``). Each of these will be described in pseudocode below.

### The Main Loop
This algorithm uses the ``private int Index`` and ``private bool Finished`` fields.
 - Initialize the default FrameEventArgs.
 - While Finished is false and Index < Root's element count, do:
   - Update Pointer.
   - If Pointer is nonempty (amount of elements in Pointer.Split.Inner > 0):
     - If the first element in Pointer is of type Control: do a control call.
     - If the first element in Pointer is of type Return: update the FrameEventArgs (the return value is the result of the expression evaluation) and set Finished to ``true``.
     - Else: run the expression in Pointer.
   - Increase Index.
 - Crash this StackFrame with the (updated) FrameEventArgs.
If a VTSException is thrown in the main loop, Crash the frame with an appropriately constructed FrameEventArgs (CodeException).
If another Exception is throw in the main loop, Crash the frame with an appropriately constructed FrameEventArgs (InternalException).

### The Control Call
The CodeBlock passed to this method will always have the following form:
```
(if or while) (<expression>) {
  <statements>
}
```
Thus, CodeBlock will feature a structure like this (in its Split field):
```
(if or while) [ <expression> ]
```
And the Block will have a children list, which can be executed.  
Below is the pseudocode for executing a control call:
 - Run the expression in ``block.Split.Inner[1]``, by converting it to an expression (the result is stored in ``private VTSVariable tempValue``), and then calling the *Run Expression* part of the algorithm.
 - If the ``tempValue`` is of type ``CoreStructures.VTSBool`` and it evaluates to true:
   - Create a copy frame from this StackFrame and execute it; if it finishes with a non-EOF code, Crash this StackFrame as well.
   - Run the copy.
   - If this StackFrame didn't yet return and the type of control call is while, decrease Index.

### The Expression Evaluation
This evaluates a single (postfix) Expression e using a stack, pseudocode is given below:
 - If e is null: return
 - If e is not a block: error
 - For each expression part sub in e, do:
   - If sub's Type is Return or None: skip
   - If sub's Type is an Identifier: push the dereferenced value onto the stack (this will be either a VTSVariable, a Class Reference, or an undefined value).
   - If sub's Type is a Literal: push the parsed literal onto the stack.
   - If sub's Type is an operator:
     - Pop the operands.
     - If the operator is an assignment: assign the second operand's value to the first one.
     - If it's another operator: attempt to execute the operator and push the result.
   - If sub's Type is Member:
     - Pop the required amount of arguments.
     - Pop one extra argument as caller.
     - Attempt to call the action referenced by sub.
     - Push the result.
   - If sub's Type is Field:
     - Pop the reference to its holder.
     - Push the variable referenced by ``holder.sub``.
 - If there is one value left on the stack: assign it to the ``private VTSVariable tempValue`` field.
