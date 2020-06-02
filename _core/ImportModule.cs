using System;
using System.IO;
using Jay.VTS.Parser;
using Jay.Logging;

namespace Jay.VTS
{
	public class ImportModule
	{
		public ImportModule() {}

		public void LoadSource(CodeBlock Parse, CodeBlock Root) {
			string[] fPath = Parse.File.Split('/');
			Logger.Log(" -> [ImportModule]: Started loading Source files...");
			Parse.Contents.ForEach(block => {
				if(block.Line.TrimStart().StartsWith("#load <")) {
					string loadFile = block.Line.TrimStart().Split('<')[1].Split('>')[0];
					fPath[fPath.Length - 1] = loadFile;
					if(!Program.Instance.LoadedFiles.Contains(string.Join("/", fPath))) {
						Logger.Log("  -> [ImportModule]: Added " + fPath);
						CodeBlock val = Interpreter.CreateLow(string.Join("/", fPath)).FirstPass().LoadVTSModules().LoadImports().Root;
						val.Contents.ForEach(rt => Root.Contents.Insert(0, rt));
					}
				}
			});
		}

		public void LoadModules(CodeBlock Parse) {
			Logger.Log(" -> [ImportModule]: Started loading modules...");
			Parse.Contents.ForEach(block => {
				if(block.Line.TrimStart().StartsWith("#vts <")) {
					string mod = block.Line.TrimStart().Split('<')[1].Split('>')[0];
					if(Interpreter.Instance.VTSModules.ContainsKey(mod)) {
						if(!Interpreter.Instance.VTSModuleStatus[mod]) {
							Logger.Log("  -> [ImportModule]: Added " + mod);
							Interpreter.Instance.VTSModuleStatus[mod] = true;
							Interpreter.Instance.VTSModules[mod]();
						}
						else {
							Logger.Log("  -> [ImportModule]: " + mod + " already added");
						}
					}
					else {
						throw new VTSException("ModuleNotFoundError", "loadVTSMod", 
							"Built-in module <" + mod + "> doesn't exist.");
					}
				}
				else {
				}
			});
		}
	}
}
