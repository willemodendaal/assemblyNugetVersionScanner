using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;

namespace Willem.AssemblyReferenceDebugger.Commands.Helpers
{
	internal class NugetInfoExtractor
	{
		/// <summary>
		/// Finds which nuget package the assembly is part of (scans the /packages/ folder).
		/// Returns the nuget package name, and version, or null if this was a direct dll reference.
		/// Returns multiple lines if this DLL was found in multiple nuget packages.
		/// </summary>
		/// <param name="asm"></param>
		/// <returns></returns>
		internal IEnumerable<string> GetNugetInfoStrings(FileInfo asm)
		{
			DirectoryInfo[] foldersThatThisAssemblyResidesIn = GetPackageFoldersThatThisAssemblyResidesIn(asm);

			foreach (var directoryInfo in foldersThatThisAssemblyResidesIn)
			{
				yield return $"{asm.Name} found in package => {directoryInfo.FullName}";
			}
		}

		private DirectoryInfo[] GetPackageFoldersThatThisAssemblyResidesIn(FileInfo asm)
		{
			var packagesFolder = FindPackagesFolderByScanningUpTheTree(asm.Directory);
			if (packagesFolder == null)
			{
				throw new Exception($"/Packages folder not found, when scanning up from: {asm.Directory.FullName}");
			}

			return FindFoldersThatContainThisAssembly(scanFrom: packagesFolder, asm: asm).ToArray();
		}

		private DirectoryInfo FindPackagesFolderByScanningUpTheTree(DirectoryInfo asmDirectory)
		{
			if (asmDirectory.Name.Equals("packages", StringComparison.CurrentCultureIgnoreCase))
			{
				return asmDirectory;
			}
			else if (asmDirectory.GetDirectories().Any(subDir => subDir.Name.Equals("packages", StringComparison.CurrentCultureIgnoreCase)))
			{
				//Not this folder, but another sub folder (a sibling).
				return asmDirectory.GetDirectories().Single(subDir => subDir.Name.Equals("packages", StringComparison.CurrentCultureIgnoreCase));
			}
			else
			{
				if (asmDirectory.Parent == null)
				{
					return null;
				}
				return FindPackagesFolderByScanningUpTheTree(asmDirectory.Parent);
			}
		}

		private IEnumerable<DirectoryInfo> FindFoldersThatContainThisAssembly(DirectoryInfo scanFrom, FileInfo asm)
		{
			foreach(var subPackageFolder in scanFrom.GetDirectories())
			{
				if (FolderOrSubFoldersContainsThisAssembly(subPackageFolder, asm))
				{
					yield return subPackageFolder;
				}
			}
		}

		private bool FolderOrSubFoldersContainsThisAssembly(DirectoryInfo folder, FileInfo asm)
		{
			var filesInThisFolder = folder.GetFiles();
			var matchingFileNameOrNull = filesInThisFolder.FirstOrDefault(f => f.Name == asm.Name);
			if (matchingFileNameOrNull != null 
			    && MatchingFileVersionIsTheSame(matchingFileNameOrNull, asm))
			{
				return true;
			}

			//File not found in this specific folder. Scan sub-folders.
			var subFolders = folder.GetDirectories();
			if (subFolders.Any(sub => FolderOrSubFoldersContainsThisAssembly(sub, asm)))
			{
				return true;
			}

			//No match in this folder, or sub folders.
			return false;
		}

		private bool MatchingFileVersionIsTheSame(FileInfo assemblyThatMightBeAMatch, FileInfo asm)
		{
			var otherAssemblyVersion = GetDllVersion(assemblyThatMightBeAMatch);
			var mainAssemblyVersion = GetDllVersion(asm);

			if (otherAssemblyVersion == null || mainAssemblyVersion == null)
			{
				//Probably a native file, not something with a version nr.
				return false;
			}
			else
			{
				return otherAssemblyVersion.Equals(mainAssemblyVersion);
			}
		}

		private string GetDllVersion(FileInfo asm)
		{
			if (VersionCache.ContainsKey(asm.FullName))
			{
				return VersionCache[asm.FullName];
			}

			var version = FileVersionInfo.GetVersionInfo(asm.FullName);
			VersionCache.Add(asm.FullName, version.FileVersion);
			return version.FileVersion;
		}

		private static readonly Dictionary<string, string> VersionCache = new Dictionary<string, string>();
	}
}