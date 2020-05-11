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
						CodeBlock val = new Interpreter(string.Join("/", fPath)).FirstPass().LoadVTSModules().LoadImports().Root;
						val.Contents.ForEach(rt => Root.Contents.Insert(0, rt));
					}
				}
			});
		}
	}
}
