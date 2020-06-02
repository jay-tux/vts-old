using System;
using System.IO;
using Jay.VTS.Parser;

namespace Jay.VTS
{
	public class ImportModule
	{
		public ImportModule() {}

		public void LoadInternal() {}

		public void LoadSource(CodeBlock Parse, CodeBlock Root) {
			string[] fPath = Parse.File.Split('/');
			Parse.Contents.ForEach(block => {
				if(block.Line.TrimStart().StartsWith("#load <")) {
					string loadFile = block.Line.TrimStart().Split('<')[1].Split('>')[0];
					fPath[fPath.Length - 1] = loadFile;
					if(!Program.Instance.LoadedFiles.Contains(string.Join("/", fPath))) {
						CodeBlock val = Interpreter.CreateLow(string.Join("/", fPath)).FirstPass().LoadVTSModules().LoadImports().Root;
						val.Contents.ForEach(rt => Root.Contents.Insert(0, rt));
					}
				}
			});
		}

		public void LoadModules(CodeBlock Parse) {
			Parse.Contents.ForEach(block => {
				if(block.Line.TrimStart().StartsWith("#vts <")) {
					string mod = block.Line.TrimStart().Split('<')[1].Split('>')[0];
					if(!Interpreter.Instance.VTSModules.ContainsKey(mod)) {
						if(!Interpreter.Instance.VTSModuleStatus[mod]) {
							Interpreter.Instance.VTSModuleStatus[mod] = true;
							Interpreter.Instance.VTSModules[mod]();
						}
					}
					else {
						throw new VTSException("ModuleNotFoundError", "loadVTSMod", 
							"Built-in module <" + mod + "> doesn't exist.");
					}
				}
			});
		}
	}
}
