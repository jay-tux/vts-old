# VTS - Jay.VTS.Interpreter
The Interpreter class does a lot of the preliminary work. Here is an overview of all ``public`` fields and methods.
## Fields
 - ``public Dictionary<string, Action> VTSModules``;  
 This ``Dictionary`` holds the information on how which module is injected into the interpreter. The key (``string``) represents the name of the module, and the value (``System.Action``) represents how the injection goes.  
 - ``public Dictionary<string, bool> VTSModuleStatus``;  
 This ``Dictionary`` holds the information on each module's status: a value set to ``true`` implies the module is loaded.
 - ``public CodeBlock Pass``;  
 Holds the result after the first code pass.
 - ``public CodeBlock Root``;  
 Holds the root ``CodeBlock``. This ``CodeBlock`` contains all code in the current context.
 - ``public string Filename``;  
 The name of the source file this Interpreter instance is responsible for parsing.
 - ``public Dictionary<string, VTSClass> Classes``;  
 Holds all classes available in the current context. Each of those classes holds their own operators, actions and fields.  
 - ``public List<VTSVariable> Variables``; **obsolete**.
 - ``public static Interpreter Instance``;  
 A static entry for the other classes.

## Methods
 - ``public static void Create(string file)``;  
 Creates the main Interpreter instance. If it's already set, writes an error message to stderr.
 - ``public static Interpreter CreateLow(string file)``;  
 Creates a secondary Interpreter instance for parsing other source files.
 - ``public Interpreter LoadVTSModules()``;  
 Calls the ``ImportModule`` to import all builtin modules required. Chainable method.
 - ``public bool ContainsClass(string ClassName)``;  
 Returns ``true`` if the current context has a class with the given name.
 - ``public void PrintClasses()``;  
 Prints an overview of all class names in the current context.
 - ``public void AddClass(VTSClass ClassCode)``;  
 Adds a class to the current context, or overwrites it if it already exists.
 - ``public void PrintAll()``;  
 If the ``VERBOSE`` flag is set, prints a full overview (think ``core.typedump()``) to stdout using the ``STRUTURAL`` log type.
 - ``public Interpreter LoadImports()``;  
 Includes all local files necessary using the ``ImportModule``. Chainable method.
 - ``public Interpreter FirstPass()``;  
 Splits all code into ``CodeBlocks`` (performs the first pass). Chainable method.
 - ``public Interpreter SecondPass()``;  
 Executes all loaded code (performs the second pass). Also set the current direcotry and logs a full code overview using the ``PARSING`` log type. Finally handles the exit of the root ``StackFrame``. Chainable method.
