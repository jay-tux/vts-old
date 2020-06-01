using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Jay.Xtend;
using Jay.VTS.Enums;
using Jay.VTS.Parser;
using Jay.VTS.Structures;
using Jay.VTS.Execution;
using Jay.Logging;

namespace Jay.VTS
{
	public class Interpreter
	{
		private List<string> _imported;
		private string _native;
		public string Native { get => _native; private set => _native = value; }
		public int ImportedLength { get => _imported.Count; }
		public CodeBlock Pass;
		public CodeBlock Root;
		public string Filename { get; }
		//public List<VTSClass> Classes = new List<VTSClass>() { CoreStructures.CoreClass, CoreStructures.VoidClass };
		public Dictionary<string, VTSClass> Classes = new Dictionary<string, VTSClass>();
		public List<VTSVariable> Variables;
		public static Interpreter Instance;

		public static void Create(string file) {
			if(Instance == null) {
				Instance = new Interpreter(file) {
					Variables = new List<VTSVariable>()
				};
			}
			else {
				Console.Error.WriteLine(" --> Instance is already set.");
			}
		}

		public static Interpreter CreateLow(string file) => new Interpreter(file);

		private Interpreter(string file)
		{
			Classes = new Dictionary<string, VTSClass>();
			if(file == "--interactive")
			{
				//start interactive session
			}
			else
			{
				try
				{
					this.Filename = file;
					Program.Instance.LoadedFiles.Add(file);
					if(File.Exists(file))
					{
						_native = File.ReadAllText(file);
					}
					else
					{
						throw new VTSException("IOLoadError", "loadsource", 
								$"Can't find VTS-source file '{file}'.");
					}
				}
				catch(IOException ioe)
				{
					throw new VTSException("IOLoadError", "loadsource",
							$"Issue trying to read VTS-source file '{file}'.");
				}
			}
		}

		public string this[int line] { get => _imported[line]; private set => _imported[line] = value; }

		public Interpreter LoadVTSModules() {
			return this;
		}

		public bool ContainsClass(string ClassName) => this.Classes.ContainsKey(ClassName);
		public void PrintClasses() => Console.WriteLine(Classes.Count + " Classes in memory: " + string.Join(", ", Classes));
		public void AddClass(VTSClass ClassCode) => this.Classes[ClassCode.Name] = ClassCode;
		
		public void PrintAll() {
			Logger.Log(" ===== Current Memory Structures: =====");
			Classes.ForEach(cls => {
				Logger.Log(" -> " + cls.Key + " [" + cls.Value.Actions.Keys.Count +" actions]");
				cls.Value.Fields.Keys.ForEach(fld => Logger.Log("   -> Field::" + fld));
				cls.Value.Actions.Values.ForEach(act => Logger.Log("   -> Action::" + act));
				cls.Value.Operators.AsEnumerable().ForEach(x => Logger.Log("   -> Operator<" + x.Key + ">::" + x.Value));
			});
			CoreStructures.BuiltinClasses.ForEach(cls => {
				Logger.Log(" -> " + cls.Key + " [" + cls.Value.Actions.Keys.Count +" actions]");
				cls.Value.Fields.Keys.ForEach(fld => Logger.Log("   -> Field::" + fld));
				cls.Value.Actions.Values.ForEach(act => Logger.Log("   -> Action::" + act));
				cls.Value.Operators.AsEnumerable().ForEach(x => Logger.Log("   -> Operator<" + x.Key + ">::" + x.Value));
			});
			Logger.Log(" ===== End of Overview ===== \n\n");
		}

		public Interpreter LoadImports() {
			Root = new CodeBlock() {
				Parent = null,
				IsLine = false,
				Contents = new List<CodeBlock>() { Pass },
				Lineno = -1,
				Line = "",
				File = "<root of source>",
				Type = "root"
			};
			new ImportModule().LoadSource(Pass, Root);
			return this;
		}
		public Interpreter FirstPass() {
			Pass = new CodeSplitter(_native, Filename).SplitCode();
			return this;
		}
		public Interpreter SecondPass() {
			Root.Contents.ForEach(fil => Logger.Log((string)fil + "\n"));
			Logger.Log("");
			Root.Contents.ForEach(fil => Logger.Log(fil.ToParentString(0) + "\n"));
			//Console.WriteLine(" ------ ");
			StackFrame rootFrame = StackFrame.FindEntry(Root);
			if(rootFrame == null) {
				throw new VTSException("NoEntryError", "secondPass::entry", "VTS Entry point not set.");
			}
			rootFrame.StackFrameReturns += (src, args) => {
				switch(args.ExitCode) {
					case FrameEventArgs.Exits.Return:
					case FrameEventArgs.Exits.ReturnValue:
						Program.Instance.ExitCode = 0;
						break;

					case FrameEventArgs.Exits.CodeException:
						Program.Instance.ExitCode = (int)args.ExitCode;
						throw args.Error;

					case FrameEventArgs.Exits.InternalException:
						Console.Error.WriteLine(" ==== An internal error has occured. ==== \nPlease notify the developer of " + 
							"the following error:\n" + args.InternalError);
						Program.Instance.ExitCode = (int)args.ExitCode;
						break;
				}
			};
			rootFrame.Execute();
			//Console.WriteLine(" ------ ");
			return this;
		}

		public void Add(string Import) => _imported.Add(Import);
	}
}
