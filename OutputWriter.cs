using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Willem.AssemblyReferenceDebugger
{
	/// <summary>
	/// Writes to console, and appends to specified file.
	/// </summary>
	class OutputWriter
	{
		private readonly string _fileName;

		public OutputWriter(string fileName)
		{
			_fileName = fileName;
		}

		public void WriteLine(params string[] lines)
		{
			foreach (var line in lines)
			{
				Console.WriteLine(line);
			}
			File.AppendAllLines(_fileName, lines);
		}

		public void WriteLineToFileOnly(params string[] userInput)
		{
			File.AppendAllLines(_fileName, userInput);
		}
	}
}
