using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Willem.AssemblyReferenceDebugger
{
	class Program
	{
		static string _outputFileName = $"assemblyReferenceDebugger-{DateTime.Now:yyyy-M-d_hh-mm-ss}.log";

		static void Main(string[] args)
		{
			RenderTitle();
			Features? chosenFeature = null;

			while(chosenFeature != Features.Exit)
			{
				RenderMenu();
				chosenFeature = GetUserMainMenuChoice();

				if (chosenFeature == null)
				{
					RenderIncorrectChoiceText();
				}
				else if (chosenFeature == Features.Exit)
				{
					//Do nothing. 'while' will exit.
				}
				else
				{
					new FeatureExecutor().ExecuteFeature(chosenFeature.Value, _outputFileName);
				}
			}
		}

		private static void RenderIncorrectChoiceText()
		{
			Console.WriteLine("\nIncorrect menu option. Please try again.\n");
		}

		private static Features? GetUserMainMenuChoice()
		{
			Console.Write("Choose a menu item:");
			var chosenText = Console.ReadLine();

			if (int.TryParse(chosenText, out int chosenNr))
			{
				var chosenEnumValue = (Features)chosenNr;
				return chosenEnumValue;
			}
			
			return null;
		}

		private static void RenderMenu()
		{
			Console.WriteLine("\nMain menu:");
			Console.WriteLine($"{(int)Features.ListDllInfo} - List info about DLLs (versions, nuget package info, etc)");
			Console.WriteLine($"{(int)Features.FindAmbiguousClass} - Find ambiguous class in multiple dlls.");
			Console.WriteLine($"{(int)Features.Exit} - EXIT");
		}

		private static void RenderTitle()
		{
			Console.WriteLine("Assembly Reference Debugger");
			Console.WriteLine("- To help figure out what's going on in /bin!\n\n");
		}
	}
}
