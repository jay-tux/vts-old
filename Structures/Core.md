# VTS - Jay.VTS.CoreStructures
The CoreStructures class is a static class which is used as container for all builtin types and variable. Below is an overview of said types and variables, as well as their containers.  

Each of the classes (except core and void) also have a value field, which contains the actual value in the variable.

## Types
### CoreClass (used as Core)
**Fields**
None
**Internals**

| Name     | Description                                                |
|----------|------------------------------------------------------------|
| stdout   | prints its argument to stdout.                             |
| stderr   | prints its argument to stderr.                             |
| toString | returns ``core``.                                          |
| vardump  | prints all variables in the current scope to stdout *(1)*. |
| typedump | prints all defined types to stdout *(2)*.                  |
| wait     | waits for the end-user to push any key.                    |

*(1): this dump consists of all variables in the current StackFrame, all builtin constants, and (if the StackFrame is a copy frame) all of the StackFrame Parent's variables. After the variables, the current stack trace is displayed.*
*(2): all VTSClasses in this dump are displayed with their fields, operators and actions.*
**Operators**
None

### VoidClass (used as Void)
**Fields**
None
**Internals**
None
**Operators**
None

### ListClass (used as list)
**Fields**

| Name    | Description                         |
|---------|-------------------------------------|
| count   | The amount of elements in the list. |

**Internals**

| Name     | Description                                                   |
|----------|---------------------------------------------------------------|
| toString | Returns ``[list`<cnt>]`` (``<cnt>`` is the count field).      |
| new      | Constructor. Initializes the list.                            |
| add      | Adds its argument to the list and increases the count field.  |
| get      | Gets the list value at the index referred to by its argument. |
| set      | (index, val) -> replaces the element at index by val.         |
| remove   | (index) -> removes the index-th argument and decreases count. |

**Operators**
None

### VTSInt (used as int)
**Fields**
None

**Internals**

| Name        | Description                             |
|-------------|-----------------------------------------|
| toString    | Returns the string value of this int.   |
| toFloat     | Converts this int to a float value.     |

**Operators**
All algebraic (``+``, ``-``, ``*``, ``/``) operators are defined for the int type, as well as the modulo (``%``) and the comparison (``==``, ``<``, ``>``) operators.

### VTSString (used as string)
**Fields**
None

**Internals**

| Name       | Description                                                  |
|------------|--------------------------------------------------------------|
| toString   | Returns this variable.                                       |
| getLength  | Returns an int value representing the length of this string. |

**Operators**
The equality (``==``) and addition (``+``) operator are defined. The addition (``+``) operator is used to append strings.

### VTSFloat (used as float)
**Fields**
None

**Internals**

| Name        | Description                              |
|-------------|------------------------------------------|
| toString    | Returns the string value of this float.  |
| toFloat     | Converts this float to an int value.     |

**Operators**
All algebraic (``+``, ``-``, ``*``, ``/``) operators are defined for the float type, as well as the comparison (``==``, ``>``, ``<``) operators.

### VTSBool (used as bool)
**Fields**
None

**Internals**

| Name        | Description                             |
|-------------|-----------------------------------------|
| toString    | Returns the string value of this bool.  |
| flip        | Returns the opposite of this bool.      |

**Operators**
All logical (``==``, ``&``, ``|``) operators are defined for the bool type.

## Variables
### Void (used as void)
The only instance of the Void type, used to indicate empty variables.

### Core (used as core)
The only instance of the Core type, used to access its methods.

### False (used as false)
**This is not a field, but a method**  
A quick constructor for a bool variable with the ``false`` value.

### True (used as true)
**This is not a field, but a method**  
A quick constructor for a bool variable with the ``true`` value.

## Containers
### BuiltinVariables
Contains the (two) builtin constants:
```cs
public static Dictionary<string, VTSVariable> BuiltinVariables = new Dictionary<string, VTSVariable>() {
  ["void"] = Void, ["core"] = Core
};
```
### BuiltinClasses
Contains the builtin types:
```cs
public static Dictionary<string, VTSClass> BuiltinClasses = new Dictionary<string, VTSClass>() {
  ["Core"] = CoreClass, ["Voidtype"] = VoidClass,
  ["int"] = VTSInt, ["string"] = VTSString, ["float"] = VTSFloat, ["bool"] = VTSBool,
  ["list"] = ListClass
};
```
