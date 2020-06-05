# VTS - Jay.VTS.Parser.LineElement and Jay.VTS.Parser.LineSplitter
A pair of classes for representing the elements within a single statement.

## LineElement Class
### Fields
 - ``public ElementType Type``;  
 Which type of element is this?
 - ``public string Content``;  
 The content of this LineElement, if it's not a block LineElement.
 - ``public List<LineElement> Inner``;  
 The content of this LineElement, if it's a block LineElement.
 - ``public LineElement Parent``;  
 The parent LineElement of this one.

### Methods
 - ``public string ToOneliner()``;  
 Converts this LineElement to a one-line string representation using the following algorithm:
   - If this is a block LineElement, returns ``[ <content> ]``, where ``<content>`` is replaced by the one-line string representation of the elements in its Inner field, separated by ``, ``.
   - If this is a non-block LineElement, returns ``{ <content>: <type> }``, where ``<content>`` is replaced by its Content field and ``<type>`` is replaced by its Type field's string representation.
 - ``public bool ContainsOperator()``;  
 **Method is obsolete**.
 - ``public string ToString(uint Offset)``;  
 Converts this LineElement to its string representation using the following algorithm:
   - If this is a block LineElement: returns the string representations of all its elements, separated by ``\n`` with offset ``Offset + 1``.
   - Else: returns ``<offset>[<type>]{<content>}``, where ``<offset>`` is replaced by ``Offset`` spaces (`` ``), ``<type>`` is replaced by this LineElement's Type field's string representation and ``<content>`` is replaced by this LineElement's content field.

### Operators
 - ``public LineElement this[int index]``;  
 This indexer returns the ``index``-th element from this LineElement's Inner collection.

## LineSplitter Algorithm
This algorithm works in two passes:
### First Pass: Character-Based Splitting
Iterates over each character and uses splitting sentinels to add the required LineElements to its root, the pseudocode implementation is given below:
 - Create the root LineElement
 - If the line starts with a ``#``:
   - Return the line as a preprocessor directive (``#vts`` or ``#load`` call).
 - Set the current string to ``""``.
 - Set the current pointer to the root element.
 - Set the inString and inComment flags to false.
 - Set the next type to ``ElementType.None``.
 - For each character in the CodeBlock's content, do:
   - If the inComment flag is true:
     - If the current character signals the end of the comment: flip the inComment flag and reset the current string.
     - Else: add the current character to the current string.
   - If the inString flag is true:
     - If the current character is a ``"``: flip the inString flag, add the current string as a literal and reset it.
     - Else: add the current character to the current string.
   - Else:
     - If the current character is a space or a comma (`` `` or ``,``):
       - Determine the type of the current string and, if its type is not Void, add it to the pointer's element list.
       - Reset the current string.
       - If the next type was not none: set it to none.
       - If the current character is a comma (``,``): add a Separator element.
     -  If the current character is a dot (``.``):
       - Determine the type of the current string and, if its type is not Void, add it to the pointer's element list.
       - Reset the current string.
       - If the next type was not none: set it to none.
       - Set the next type to ``ElementType.Member``.
     - If the current character is an opening bracket (``(``):
       - Determine the type of the current string and, if its type is not Void, add it to the pointer's element list.
       - Reset the current string.
       - If the next type was not none: set it to none.
       - Add a block LineElement to the pointer's element list and move the pointer to it.
     - If the current character is a closing bracket (``)``):
       - Determine the type of the current string and, if its type is not Void, add it to the pointer's element list.
       - Reset the current string.
       - If the next type was not none, set it to none.
       - Move the pointer to the pointer's parent.
     - If the current character is a string delimiter (``"``):
       - Set the inString flag to true.
       - Determine the type of the current string and, if its type is not Void, add it to the pointer's element list.
       - Reset the current string.
       - If the next type was not none, set it to none.
     - Else:
       - Add the current character to the current string.
       - If the current string is the beginning of a comment (``/*``):
         - Set the inComment flag to true.
 - Determine the type of the current string and, if its type is not Void, add it to the pointer's element list.
 - Reroot the dots in the Root element.

### Sub-Algorithm: Determining the Type of a LineElement
In the following pseudocode, all capitalized Types are options from the ``Jay.VTS.Enums.ElementType`` enum.
 - If the next type is not none: return the next type.
 - If the current string is empty: return Void.
 - If the current string is ``class``: return Class.
 - If the current string is ``action``: return Action.
 - If the current string is ``return``: return Return.
 - If the current string is ``field``: return Field.
 - If the current string is in the Control list (``if, else, while``): return Control.
 - If the current string is in the Operators list(``+, -, *, /, %, &, |, =, ==, >, <``): return Operator.
 - If the current string is a legal literal (parsable int, parsable float, true or false): return Literal.
 - Else: return Identifier.

### Second Pass: Rerooting the Dots
This algorithm determines the kind of Member a Member LineElement actually is: a member action call, a literal float, or a member field. The recursive pseudocode is given below:
 - If the given LineElement is not a block: return.
 - For each element x in the given LineElement's block:
   - If x's type is Member:
     - If the next element is a block: this is a Member action call.
     - If the previous element is a literal: join both as a float literal.
     - Else: this is a Member field.
   - If x's type is Block:
     - Reroot the dots on x.
