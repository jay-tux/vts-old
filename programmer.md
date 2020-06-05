# VTS Programmer Guide
## General Program Structure
A VTS program contains of two parts: the entry point, and the classes. A class can contain actions and (upcoming) fields. Object-oriented design (aka inheritance and interfaces) is not yet available, but might come.
### Entry point
The entry point defines where the Interpreter should start parsing your code. Ideally, each file should either contain a class, or the entry point.  

The entry point is the only action allowed outside of a class, and can't be defined within a class.  
You define the entry point as following (the trailing ``{`` can be placed on another line, if you want to, but don't forget the space between ``entry`` and ``()``; the entry point is the only structure which is so specific in it's definition rules):  
```vts
entry () {
  /* code */
}
```
A VTS program is done running when it reaches the closing ``}`` of its entry point.  
The entry itself can't contain classes or other actions, only statements to run, and each statement ends in a ``;``.  

The ``/examples/`` contains a few examples which illustrate all parts of the VTS language.
### Comments
To mark parts of your script which don't contain code, use the following structure:
```vts
/*
Comments
*/
```
Comments and the ``/*`` and ``*/`` marks can be indented, and can be within or outside of any possible structure. Anything between the ``/*`` and the ``*/`` is not parsed.
### Loading Modules
To include builtin VTS modules into your program (like the *IO* and *HTTP* modules), use the following code:
```vts
#vts <module-name>;
```
Where you replace ``module-name`` with the name of the module you wish to import. Don't forget the ``#`` at the start of the line, the ``<`` and ``>`` surrounding the module name and the ``;``.  
Module documentation is also available, see below.
### Including Files
In order to keep your VTS projects structured, you can split code among multiple files, but each class should be within exactly one file (you can't split them over multiple files).  
To include other files into the VTS runtime, use the following directive:
```vts
#load <file-path>;
```
Where you replace ``file-path`` with the path relative to the file in which you place the command. Again, don't forget the ``#`` at the start of the directive, the ``<`` and ``>`` surrounding the file path and the terminating ``;``.
## Creating Classes
Classes are important to keep your project's code structured, and are an important part of any VTS code. A class can't have the same name as another class in your project, and can't have the same name as an internal class.  
Classes are defined as following:
```vts
class class-name
{
  fields
  actions
}
```
Class names should only contain alphanumeric characters (a-z, A-Z, 0-9).  
Instances of classes are created using their constructor.
### Constructors
A class constructor is one of the class's actions, and always has the name ``new``:
```vts
class MyClass
{
  action new() {
    /* do some preliminary work, idk... */
  }
}
```
The constructor can also have arguments, if necessary:
```vts
class MySecondClass
{
  action new(arg1, arg2) {
    /* do something useful with arg1 and arg2... */
  }
}
```

To create a variable of a class type, use the following code (in an action, or in the entry):
```vts
varname = MyClass.new();
```
Or, if the class's constructor requires arguments:
```vts
secondvar = MySecondClass.new(6.32, "a string");
```
Creating a variable is the only time you can use the ``classname.actionname(args)`` construction; otherwise, you'll always use the ``varname.actionname(args)`` construction (in other words, VTS doesn't support ``static`` actions).
### Actions
Defining an action in a VTS class is very similar to creating a constructor for that class:
```vts
class ClassWithActions
{
  action new() { /* do nothing, absolutely nothing */ }

  action firstAction() {
    /* do something */
  }

  action secondAction(arg1) {
    /* do something with arg1 */
  }
}
```
As you can see, the ``firstAction`` action doesn't have any arguments, but the second one has (one: ``arg1``).  

As mentioned before, calling a (non-constructor) action requires the use of a variable:
```vts
myVar = ClassWithActions.new(); /* create var using class name */
myVar.firstAction(); /* call parameterless action */
myVar.secondAction(5.23); /* call action with parameter */
```

An action can, if needed, return a value. This is done using the ``return`` keyword:
```vts
class Adding
{
  action new() {}
  action add(var1, var2) {
    return var1 + var2;
  }
}
```
### Fields
_Fields functionality is not yet 100% complete!_
A class can contain actions as well as data. This data is presented in the form of fields. A field is defined in its class before it's used:
```vts
class WithFields
{
  Field f1;
  Field f2;
}
```
These fields are initialized to ``void`` (see below) when a class instance is created, but they can be changed by the constructor and other methods. To access a varaible's field, use the ``varname.fieldname`` construction:
```vts
fieldVar = WithFields.new();
fieldVar.f1 = "New value for f1!!!"; /* set the value of the f1 field */
copy = fieldVar.f2; /** get the value of the f2 field */
```
## Flow of Control
In the life of a program, decisions need to be made: this is done using the ``if`` and ``while `` structures:
```vts
entry () {
  a = 0;
  lst = list.new();

  while(a < 10) {
    if(a > 5) {
      core.stdout("Added "); core.stdout(a); core.stdout("\n");
    }
    lst.add(a);
    a = a + 1;
  }
}
```
In the example above, a list is created, and using the wile loop, is filled with the numbers 0-9. In addition, all numbers in the interval ]5, 10[ are printed using the ``core.stdout`` action.  

An ``if`` loop executes the statements in its body (between the ``{`` and the ``}``) if and only if the expression between ``(`` and ``)`` is true.
*An ``else`` construction is going to be implemented, but is not finished yet.*
A ``while`` loop executes the statements in its body (between the ``{`` and the ``}``) as long as the expression between ``(`` and ``)`` is true.
## Builtin Overviews
VTS contains a few datatypes (classes) ready to use for you, so you don't have to reinvent the wheel. More classes are available in the builtin modules, which you can import.
### Boolean (``bool``)
A value representing ``true`` or ``false``. They can be created only using their respective literal values:
```vts
var1 = true;
var2 = false;
```
#### Fields
The Boolean type does not have any fields.
#### Actions
The Boolean type has two actions:
 - ``flip()``; which turns ``true`` into ``false`` and vice versa (using the return value).
 - ``toString()``; which returns a string representation of the bool.

#### Operators
The Boolean type has three operators:
 - ``==``, which tests for equality (returns ``true`` if both Booleans have the same value),
 - ``&``, which returns ``true`` if both Booleans are ``true``,
 - ``|``, which returns ``true`` if either Boolean is ``true``

### Integer (``int``)
A value representing any integral number (a 32-byte integer). They can be created only using a literal value:
```vts
var1 = 168;
var2 = -138;
```
#### Fields
The Integer type does not have any fields.
#### Actions
The Integer type has two actions:
 - ``toFloat()``; which turns an Integer into a Float value (using the return value).
 - ``toString()``; which return a string representation of the int.

#### Operators
The Integer type has eight operators, most of which have a trivial meaning:
 - ``+``, ``-``, ``*``, ``/``, and ``%``, for, respectively, addition, subtractions, multiplication, division and modulus. These operations return Integers.
 - ``==``, ``<``, ``>``, for respectively equality, smaller-than, and larger-than. These operations return Booleans.

### Float (``float``)
A value representing any real number. They can be created only using a literal value (using the ``.`` as the floating points).
```vts
var1 = 1.059;
var2 = -168.19849;
```
#### Fields
The Float type does not have any fields.
#### Actions
The Float type has two actions:
 - ``toInt()``; which turns a Float into an Integer value (using the return value; dropping the decimal part).
 - ``toString()``; which returns a string representation of the float.

#### Operators
The Float type has seven operators, most of which have a trivial meaning:
 - ``+``, ``-``, ``*``, and ``/``, for, respectively, addition, subtraction, multiplication, and division. These operations return Floats.
 - ``==``, ``<``, and ``>``, for, respectively, equality, smaller-than, and greather-than. These operations return Booleans.

### String (``string``)
A value representing a piece of text (a string of characters). They can only be created using a literal value (using the ``"`` as delimiters).
```vts
var1 = "short string";
var2 = "very long string containing a newline (\n) and a tab (\t)";
```
A string literal can't contain a newline (as in, in the code). To add a newline to your string, use ``\n``.
#### Fields
The String type does not have any fields.
#### Actions
The String type has two actions:
 - ``getLength()``; which returns an Integer representing the length of the string.
 - ``toString()``; which returns the String itself.

#### Operators
The String type has two operators:
 - ``+``; which appends two Strings and returns the result. Neither string is modified.
 - ``==``; which compares two Strings and returns ``true`` if they're equal (containing the same characters).

### List (``list``)
The List type represents a sequence of variables. Variables in the list can have any type, and a list can contain multiple types of variables at the same time.
Lists are created using their constructor:
```vts
lst = list.new();
```
#### Fields
The List type has one field:
 - ``count``; a (positive) Integer value containing the amount of elements in the list.

#### Actions
The List type has five actions (not counting the constructor):
 - ``add(element)``; adds ``element`` to the end of the list.
 - ``get(index)``; returns the element at index ``index``. Indices are zero-based. If the index is outside the list's range (0..count - 1), an error is thrown.
 - ``set(index, element)``; updates the element at index ``index``. If the index is outside the list's range, an error is thrown.
 - ``remove(index)``; removes the element at index ``index``. If the index is outside the list's range, an error is thrown. Returns the element which was removed.
 - ``toString()``; returns a String representation of the list in the following form: ``[list`<count>]``, where ``<count>`` is replaced by the amount of elements in the list.

#### Operators
The List type does not have any operators.
### Core (``Core``)
The Core type gives access to certain builtin static functionality. They can't be created. A global constant is given under the name ``core``.
#### Fields
The Core type does not have any fields.
#### Actions
The Core type has six actions:
 - ``stdout(var)``; prints var's string representation to the stdout stream.
 - ``stderr(var)``; prints var's string representation to the stderr stream.
 - ``toString()``; return the string ``"core"`` (the string representation of the variable).
 - ``vardump()``; prints all variables in the current scope to stdout.
 - ``typedump()``; prints all types in memory to stdout (with their actions and operators).
 - ``wait()``; pauses the execution until the end-user pushes any key.

#### Operators
The Core type does not have any operators.
### Void (``Voidtype``)
The Void type represents nothing (an empty variable). Void objects can't be created. A variable can be set to Void by using the ``void`` constant.
#### Fields
The Void type does not have any fields.
#### Actions
The Void type does not have any actions.
#### Operators
The Void type does not have any operators.
## Module Documentation
Each builtin module has its own documentation page:
 - [IO Module](../guide/Modules/IO.md) (for file input and output).
 - [HTTP Module](../guide/Modules/HTTP.md) (for web services; **under construction**).

## Advanced Topics
### Operator Overloading
In VTS, a lot of operators can be overloaded for use on your own classes:
 - The mathematical operators: ``+``, ``-``, ``*``, ``/``, and ``%``,  
 - The comparison operators: ``<``, ``>``, and ``==``,  
 - The Boolean operators: ``&``, and ``|``.  

There is only one operator you can't overload, which is the assignment ``=`` operator, which has precedence 0 (the lowest precedence).  
Overloading an operator requires writing an action with a specific name for your class. The name of this action depends on the operator you want to overload. Overloaded operators always take two arguments.  
Operators with higher precedence are executer earlier.
#### Mathematical Operators
| Operator Name | Operator Sign | Action Name | Precedence |
|---------------|---------------|-------------|------------|
| Add           | +             | ``_add()``  | 4          |
| Subtract      | -             | ``_sub()``  | 4          |
| Multiply      | *             | ``_mul()``  | 5          |
| Divide        | /             | ``_div()``  | 5          |
| Modulus       | %             | ``_mod()``  | 3          |

#### Comparison Operators
| Operator Name | Operator Sign | Action Name | Precedence |
|---------------|---------------|-------------|------------|
| Equality      | ==            | ``_eql()``  | 2          |
| Smaller-than  | <             | ``_sml()``  | 2          |
| Larger than   | >             | ``_lrg()``  | 2          |

#### Boolean Operators
| Operator Name | Operator Sign | Action Name | Precedence |
|---------------|---------------|-------------|------------|
| Or            | &             | ``_dis()``  | 1          |
| And           | \|            | ``_con()``  | 1          |
