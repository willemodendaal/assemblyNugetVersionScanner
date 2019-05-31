using System;

namespace Willem.AssemblyReferenceDebugger.UserInputGatherers
{
	/// <summary>
	/// Create sub-class to prompt for user input, with validation.
	/// </summary>
	class GetUserInput
	{
		private readonly OutputWriter _outputWriter;
		private readonly string _prompt;
		private readonly Func<string, bool> _isValid;
		private readonly string _invalidHint;

		public GetUserInput(OutputWriter outputWriter, string prompt, Func<string, bool> isValid, string invalidHint)
		{
			_outputWriter = outputWriter ?? throw new ArgumentNullException(nameof(outputWriter));
			_prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));
			_isValid = isValid ?? throw new ArgumentNullException(nameof(isValid));
			_invalidHint = invalidHint ?? throw new ArgumentNullException(nameof(invalidHint));
		}

		public virtual string GetInput()
		{
			string userInput = null;
			while (userInput == null)
			{
				_outputWriter.WriteLine($"\n{_prompt}");
				userInput = Console.ReadLine();
				_outputWriter.WriteLineToFileOnly(userInput);
				if (_isValid(userInput))
				{
					return userInput;
				}
				Console.ForegroundColor = ConsoleColor.Red;
				_outputWriter.WriteLine($"\nInvalid value. {_invalidHint}");
				Console.ResetColor();
				_outputWriter.WriteLine($"Please try again...");
			}
			return null;
		}
	}
}
