using System;
using System.Collections.Generic;
using Jay.VTS.Execution;

namespace Jay.VTS
{
	public class VTSException : Exception
	{
		public new string StackTrace;
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
			StackFrame curr = Stack;
			while(curr != null)
			{
				frames.Add($"\t{(string)curr}: {(int)curr}");
				curr = curr.Parent;
			}
			return string.Join(", ", frames);
		}
	}
}
/*
 *	INTERNAL ERROR TYPES
 * -> IOLoadError -> can't load source file (not found, no perms)
 * -> SyntaxError -> syntax error (in preproc) (newline in constant, ...)
 */