using System.IO;
using UnityEngine;
using UnityEditor;

namespace SDG.Unturned.Tools
{
	static class EditorAssetBundleHelper
	{
		/// <summary>
		/// Build an asset bundle by name.
		/// </summary>
		/// <param name="AssetBundleName">Name of an asset bundle registered in the editor.</param>
		/// <param name="OutputPath">Absolute path to directory to contain built asset bundle.</param>
		/// <param name="Multiplatform">Should mac and linux variants of asset bundle be built as well?</param>
		public static void Build(string AssetBundleName, string OutputPath, bool Multiplatform)
		{
			string[] AssetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(AssetBundleName);
			if(AssetPaths.Length < 1)
			{
				Debug.LogWarning("No assets in: " + AssetBundleName);
				return;
			}

			AssetBundleBuild[] Builds = new AssetBundleBuild[1];
			Builds[0].assetBundleName = AssetBundleName;
			Builds[0].assetNames = AssetPaths;

			// Saves some perf by disabling these unused loading options.
			BuildAssetBundleOptions Options = BuildAssetBundleOptions.DisableLoadAssetByFileName | BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;

			if(Multiplatform)
			{
				string OutputFile = OutputPath + '/' + AssetBundleName;
				string OutputManifest = OutputFile + ".manifest";

				string LinuxBundleName = MasterBundleHelper.getLinuxAssetBundleName(AssetBundleName);
				string LinuxOutputFile = OutputPath + '/' + LinuxBundleName;
				string LinuxManifest = LinuxOutputFile + ".manifest";

				string MacBundleName = MasterBundleHelper.getMacAssetBundleName(AssetBundleName);
				string MacOutputFile = OutputPath + '/' + MacBundleName;
				string MacManifest = MacOutputFile + ".manifest";

				// Delete existing files
				if(File.Exists(LinuxOutputFile))
					File.Delete(LinuxOutputFile);
				if(File.Exists(LinuxManifest))
					File.Delete(LinuxManifest);
				if(File.Exists(MacOutputFile))
					File.Delete(MacOutputFile);
				if(File.Exists(MacManifest))
					File.Delete(MacManifest);

				// Linux, then rename
				BuildPipeline.BuildAssetBundles(OutputPath, Builds, Options, BuildTarget.StandaloneLinuxUniversal);
				File.Move(OutputFile, LinuxOutputFile);
				File.Move(OutputManifest, LinuxManifest);
				
				// Mac, then rename
				BuildPipeline.BuildAssetBundles(OutputPath, Builds, Options, BuildTarget.StandaloneOSX);
				File.Move(OutputFile, MacOutputFile);
				File.Move(OutputManifest, MacManifest);
			}

			// Windows... finally done!
			BuildPipeline.BuildAssetBundles(OutputPath, Builds, Options, BuildTarget.StandaloneWindows64);

			// Unity (sometimes?) creates an empty bundle with the same name as the folder, so we delete it...
			string OutputDirName = Path.GetFileName(OutputPath);
			string EmptyBundlePath = OutputPath + "/" + OutputDirName;
			if(File.Exists(EmptyBundlePath))
			{
				File.Delete(EmptyBundlePath);
			}
			string EmptyManifestPath = EmptyBundlePath + ".manifest";
			if(File.Exists(EmptyManifestPath))
			{
				File.Delete(EmptyManifestPath);
			}
		}
	}
}
