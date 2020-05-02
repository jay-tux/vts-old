using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jay.VTS
{
	public class Interpreter
	{
		private List<string> _imported;
		private string _native;
		public string Native { get => _native; private set => _native = value; };
		public int ImportedLength { get => _imported.Length; }

		public Interpreter(string file)
		{
			if(file == "--interactive")
			{
				//start interactive session
			}
			else
			{
				try
				{
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
					throw VTSException("IOLoadError", "loadsource",
							$"Issue trying to read VTS-source file '{file}'.");
				}
			}
		}

		public string this[int line] { get => _imported[line]; private set => _imported[line] = value; }

		public Interpreter LoadVTSModules() => {}
		public Interpreter LoadImports() => {}
		public Interpreter FirstPass() => {}
		public Interpreter SecondPass() => {}

		public void Add(string Import) => _imported.add(Import);
	}
}
