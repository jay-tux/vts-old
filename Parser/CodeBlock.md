# VTS - Jay.VTS.Parser.CodeBlock and Jay.VTS.Parser.CodeSplitter
A pair of classes for representing code blocks (statements and actual blocks).

## CodeBlock Class
### Fields
 - ``public int Lineno``;  
 The line on which this CodeBlock is found (for stack trace purposes).
 - ``public string File``;  
 The file in which this CodeBlock is found (for stack trace purposes).
 - ``public string Line``;  
 The contents of this CodeBlock (if this is a line CodeBlock).
 - ``public List<CodeBlock> Contents``;  
 The contents of this CodeBlock (if this is not a line CodeBlock).
 - ``public CodeBlock Parent``;  
 The parent of this CodeBlock.
 - ``public bool IsLine``;  
 Is this a line CodeBlock?
 - ``public string Type``;  
 The type of this CodeBlock.
 - ``public LineElement Split``;  
 The split version of this CodeBlock.

### Methods
 - ``public string ToString(uint depth)``;  
 Converts this CodeBlock to its string representation using a depth:
   - If the depth is 0 (replace ``<filename>`` with the File field, ``<lineno>`` with the padded (4) line number, ``<type>`` with the Type field, ``<split>`` with the one-line representation of the Split field (or ``[ null ]`` if that field is null), and ``<contents>`` by the contents' string representation (on depth + 1), delimited by ``\n``):
   ```
    --- Beginning of Code Block ---
    [<filename>]
    [<lineno>] <--> { <Type> } <split>
    <contents>
    --- End of Code Block ---
    ```
   - Else (same placeholders as in the other case, ``<depth>`` an amount of spaces equal to the depth parameter):
    ```
    [<lineno>]<depth> <--> { <Type> } <split>
    <contents>
    ```
 - ``public string ToParentString(uint depth)``;  
 A similar function to the ``ToString(uint depth)`` function, but replace the ``<--> { <Type> } <split>`` with ``<line> --> <parent>``, where ``<line>`` is this CodeBlock's line and ``<parent>`` is this CodeBlock's parent's line, if any, or ``(null)`` otherwise.
 - ``public List<LineElement> Slice(int index)``;  
 Gets all elements from this CodeBlock's Split LineElement starting from index ``index``.

### Operators
 - ``public static explicit operator string(CodeBlock target)``;  
 Converts ``target`` to a string using the ``target.ToString(0)`` method.

## CodeSplitter Algorithm
This algorithm works in one pass to convert a string (file's contents) to a CodeBlock; its pseudocode implementation is given below:

 - Create the root CodeBlock, and set both the current and currParent pointer to it.
 - Set the current depth to 0, the current method and containing class to null.
 - Set the current line number to 1, and the inString, inComment and asterisk flags to false.
 - Initialize the current string to ``""`` (the empty string).
 - For each character in the VTS code, do:
   - If the inComment flag is true:
     - If the current character is a a newline: increment the line number.
     - If the current character is an asterisk (``*``), set the asterisk flag to true.
     - If the current character is a slash (``/``) and the asterisk flag is true, set the inComment flag to false and reset the current string.
     - Else: Set the asterisk flag to false.
   - If the inString flag is true:
     - If the current character is a ``"``, set the inString flag to false.
     - Append the current character to the current string.
   - Else: do something depending on the character:
     - ``*``: if the current line ends with a slash ``/``, set the inComment flag to true, and reset the current string; otherwise add the ``*`` to the current line.
     - ``"``: set the inString flag to true and append ``"`` to the current string.
     - ``\n``: increment the line number and append `` `` to the current string.
     - ``{``: add the current line as a CodeBlock.
         - If it signaled a class: set the containing variable to this class.
         - If it signaled an action: set the current method to this action.
         - Else: increase the current depth.
         - Move the pointer to newest CodeBlock's block list and the currParent pointer to the newest CodeBlock
         - Reset the current line.
     - ``}``:
         - If the depth is 0:
           - If the current method is not null:
             - If the current CodeBlock signaled an operator: add it as operator.
             - Else: add it as a method.
             - Set the current method to null.
           - If the containing class is not null: add the current CodeBlock as a class.
         - Else: decrease the depth.
         - Move the current pointer to its parent.
         - Move the currParent pointer to its parent.
         - Reset the current line.
     - ``;``: Add the current line as CodeBlock to the pointer and reset it.
     - Anything else: Add the character to the current line.
