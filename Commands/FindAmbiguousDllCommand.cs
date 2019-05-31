using System;

namespace Willem.AssemblyReferenceDebugger.Commands
{
	class FindAmbiguousDllCommand: ICommand
	{
		public void Execute(OutputWriter outputWriter)
		{
			outputWriter.WriteLine(nameof(FindAmbiguousDllCommand));
			outputWriter.WriteLine("Done!");
		}
	}
}
