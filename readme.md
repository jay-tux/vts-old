# VTS  
###### Visual (Multi-)threaded Scripting; an exercise in programming languages.

## Compiling & Running
As VTS is written in Mono/C#, you'll require some software to be able to build and run VTS. This also depends on the OS you're working on.
### UNIX-based systems
You'll need the ``dotnet-runtime`` and ``mono`` packages for compilation and running. Most distributions have them in their package managers.
### Windows
Usually, you should have the .NET framework preinstalled. For compilation, I recomment using Visual Studio.
### Actual Compilation with Mono
To compile the VTS interpreter with mono, run:  
```
mcs -r:System.Data -r:System.Drawing -r:System.Windows.Forms \
  -out:<filepath> -main:Jay.VTS.Program *.cs */*.cs
````  
from the root directory of the source code. Don't forget to replace ``<filepath>`` with a path to where you'd like to find the executable. (For example, ``bin/vts.exe``).  

The repository also contains a small (Bash) script which does exactly that. To run the compiler, run ``./compile -c`` from the root directory. When run, the script creates an executable in ``bin/vts-parse.exe``.  
You can also compile a debug version with ``./compile -c -d`` (resulting in ``bin/vts-debug.exe``; the debug version may not always be as stable as the normal version) or a verbose logging version with ``./compile -c -v`` (resulting in ``bin/vts-verbose.exe``).
### Running VTS with Mono
Running VTS requires a quick command:
```
mono <executable> <vts-script file>
```
This command runs the interpreter on the given script file. In order to maximize debug and verbose output, I recommend running the following:
```
mono --debug <executable> <vts-script file>
```
When not given a file to run, the interpreter will try to run with the ``--interactive`` mode, with hasn't been implemented yet.
## Quick Feature Overview
VTS has a lot of features already; some of them will be listed here. For a detailed overview, see [Programmer's Guide](../guide/programmer.md).  
 - Builtin string, float, int and list types
 - Dynamic typing system
 - Local file inclusion
 - Custom classes and member methods (called actions)
 - Builtin operator overloading support
 - File IO module
 - Variable and type overviews (dumps)

## Development Overview
Because VTS contains a lot of classes and files, I have opted to move them into their own namespaces and directories. For a detailed overview of which class (and which method) does what, see [Developer's Guide](../guide/dev.md).
