using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jay.VTS
{
	public class Program
	{
		public static Program Instance = new Program();
		public List<string> LoadedFiles = new List<string>();
		public int ExitCode { set => Environment.ExitCode = value; }

		public void Start(string[] args)
		{
			try
			{
				Interpreter.Create(args.Length < 1 ? "--interactive" : args[0]);
				Interpreter.Instance.FirstPass().LoadVTSModules().LoadImports().SecondPass();
			}
			catch(VTSException vtse)
			{
				Console.Error.WriteLine(" === An error has occured === ");
				while(vtse != null)
				{
					Console.Error.WriteLine($"Error Type: {vtse.Type}");
					Console.Error.WriteLine($"Error Message: {vtse.Message}");
					Console.Error.WriteLine($" --- Stack Trace --- ");
					Console.Error.WriteLine(vtse.StackTrace);
					Console.Error.WriteLine($" --- End of Stack Trace --- \n");
					vtse = vtse.Cause;
				}
			}
		}

		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Instance.Start(args);
			/*Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());*/
		}
	}
}
