using System.IO;
using System.Text.RegularExpressions;

namespace Willem.AssemblyReferenceDebugger.UserInputGatherers
{
	class GetUserBinPathInput: GetUserInput
	{
		public GetUserBinPathInput(OutputWriter output) 
			: base(
				output,
				"Please enter the path to the /bin folder:", 
				isValid:(pathToFolder)=> Regex.IsMatch(pathToFolder, "[\\:]") && Directory.Exists(pathToFolder),
				invalidHint: "Input must be an *existing* folder, like c:\\subfolder\\bin\\")
		{
		}
	}
}
