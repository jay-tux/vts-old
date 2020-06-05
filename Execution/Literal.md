# VTS - Jay.VTS.Execution.Literal
A class containing an algorithm for converting literal values to VTSVariables (in ``static VTSVariable Literal.Parse(string Literal)``), the pseudocode for the algorithm is below:
 - If Literal is ``true``: return the ``CoreStructures.True`` value.
 - If Literal is ``false``: return the ``CoreStructures.False`` value.
 - If Literal can be parsed as a float (into floatval):
   - If the Literal contains a floating point separator (``.``): return it as a VTSFloat.
   - Else: return it as a VTSInt.
 - Else: return Literal as a VTSString.
