using System;
using System.Collection.Generic;

namespace Jay.VTS
{
	public class VTSException : Exception
	{
		public string StackTrace;
		public string Type;
		public VTSException Cause;
		public VTSException(string Type, StackFrame Stack, string Message, VTSException Cause) 
			: base(Message)
		{
			this.StackTrace = GenTrace(Stack);
			this.Type = Type;
			this.Cause = Cause;
		}

		public VTSException(string Type, string LoadMod, string Message) : base(Message)
		{
			this.StackTrace = $"\t<preproc_{LoadMod}>";
			this.Type = Type;
			this.Cause = null;
		}

		private string GenTrace(StackFrame Stack)
		{
			List<string> frames = new List<string>();
			while(Stack.Parent != null)
			{
				frames.add($"\t{(string)Stack}: {(int)Stack}";
			}
			return frames.Join("\n");
		}
	}
}
/*
 *	INTERNAL ERROR TYPES
 * -> IOLoadError -> can't load source file (not found, no perms)
 */
