using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Willem.AssemblyReferenceDebugger.Commands.Helpers;
using Willem.AssemblyReferenceDebugger.UserInputGatherers;

namespace Willem.AssemblyReferenceDebugger.Commands
{
	class ListDllInfoCommand: ICommand
	{
		public void Execute(OutputWriter output)
		{
			var validBinPath = new GetUserBinPathInput(output).GetInput();
			InspectAllAssemblies();

			//Local functions...
			void InspectAllAssemblies()
			{
				var assemblies = Directory.EnumerateFiles(validBinPath, "*.*", SearchOption.AllDirectories)
					.Where(file =>  PrefixesToScan.Any(pfx => file.ToLower().EndsWith(pfx)))
					.ToArray();

				foreach (var asm in assemblies.OrderBy(a => a))
				{
					InspectAssembly(new FileInfo(asm));	
				}

				if (assemblies.Length == 0)
				{
					output.WriteLine("\nNo assemblies found :(");
				}
			}

			void InspectAssembly(FileInfo asm)
			{
				var assemblySummary = new StringBuilder();
				assemblySummary.AppendLine($"{asm.Name}:\tDLL Version:[{GetDllVersion(asm)}]\tNuget:[{GetNugetInfo(asm)}]");

				output.WriteLine(assemblySummary.ToString());
			}

			string GetDllVersion(FileInfo asm)
			{
				var version = FileVersionInfo.GetVersionInfo(asm.FullName);
				return version.FileVersion;
			}

			string GetNugetInfo(FileInfo asm)
			{
				var matchingNugetPackages = new NugetInfoExtractor()
					.GetNugetInfoStrings(asm)
					.ToArray();

				if (matchingNugetPackages.Length == 1)
				{
					return matchingNugetPackages[0];
				}
				else if (matchingNugetPackages.Length == 0)
				{
					return "(Not a nuget package)";
				}
				else
				{
					//Multiple package matches. Return with newlines.
					return "\r\n - " + string.Join("\r\n - ", matchingNugetPackages);
				}
			}
		}

		private readonly string[] PrefixesToScan = new[] { ".dll", ".exe" };
	}
}
