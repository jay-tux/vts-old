# VTS - Jay.VTS.ImportModule
The ImportModule contains some methods for loading modules and including local files. Here is an overview of all ``public`` methods.

## Methods
 - ``public ImportModule()``; **obsolete, methods should become static**.
 - ``public void LoadSource(CodeBlock Parse, CodeBlock Root)``;  
 Searches ``Parse`` for all ``CodeBlock``s of the form ``#load <file>;`` and loads those into memory using the Interpreter's static ``CreateLow`` method. Modifies ``Root`` to reflect these changes.
 - ``public void LoadModules(CodeBlock Parse)``;  
 Searches ``Parse`` for all ``CodeBlock``s of the form ``#vts <module>;`` and activates those modules in the ``Interpreter``. Logs failure, success and obsolete calls to stdout wit the ``MODULE`` log type.
