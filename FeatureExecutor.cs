using System;
using Willem.AssemblyReferenceDebugger.Commands;

namespace Willem.AssemblyReferenceDebugger
{
	class FeatureExecutor
	{
		public void ExecuteFeature(Features chosenFeature, string outputFileName)
		{
			try
			{
				var outputWriter = new OutputWriter(outputFileName);
				switch (chosenFeature)
				{
					case Features.FindAmbiguousClass:
						new FindAmbiguousDllCommand().Execute(outputWriter);
						break;
					case Features.ListDllInfo:
						new ListDllInfoCommand().Execute(outputWriter);
						break;
					default:
						throw new Exception($"Unknown command {chosenFeature}. Unable to execute.");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"*** Error executing command: {e.Message}");
			}
		}
	}
}
