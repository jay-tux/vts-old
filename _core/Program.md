# VTS - Jay.VTS.Program
The Program class is (mostly) the entry point for the VTS interpreter. Here is an overview of all ``public`` fields and methods.  
## Fields
 - ``public static Program Instance``;  
 A static entry for the other classes.
 - ``public List<string> LoadedFiles``;  
 A list containing all loaded local files, to prevent double inclusion.
 - ``public int ExitCode``;  
 A quick setter for the runtime's exit code.

## Methods
 - ``public void Start(string[] args)``;  
 The actual entry point which creates the Interpreter, starts all phases (first code pass, loading modules, loading local files, execution (second pass)) and catches code-based exceptions. Also logs how long the execution took on the ``NOIGNORE`` log-level.
 - ``[STAThread] static void Main(string[] args)``;  
 The application entry; calls the instance's Start method.
