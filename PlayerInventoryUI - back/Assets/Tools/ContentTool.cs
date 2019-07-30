using UnityEditor;
using UnityEngine;
using System.IO;

namespace SDG.Unturned.Tools
{
    /// <summary>
    /// Simple tool to export all new assetbundles. 
    /// </summary>
	public class ContentTool : EditorWindow 
	{
		[MenuItem("Window/Unturned/Content Tool")]
		public static void ShowWindow() 
		{
			GetWindow(typeof(ContentTool));
		}

		protected Vector2 ScrollPosition;

		protected virtual bool IsContentBundle(string AssetBundleName)
		{
			return AssetBundleName.EndsWith(".content");
		}

		protected virtual string GetContentBundleExportPath(string AssetBundleName)
		{
			return EditorPrefs.GetString(AssetBundleName + "_ExportPath");
		}

		protected virtual void SetContentBundleExportPath(string AssetBundleName, string Path)
		{
			EditorPrefs.SetString(AssetBundleName + "_ExportPath", Path);
		}

		protected virtual void BuildContentBundle(string AssetBundleName)
		{
			string OutputPath = GetContentBundleExportPath(AssetBundleName);
			if(string.IsNullOrEmpty(OutputPath))
			{
				Debug.LogWarning("Output path unset for: " + AssetBundleName);
				return;
			}

			EditorAssetBundleHelper.Build(AssetBundleName, OutputPath, false);
		}

		protected virtual void OnGUI_ContentBundles()
		{
			string[] AssetBundles = AssetDatabase.GetAllAssetBundleNames();
			foreach(string AssetBundleName in AssetBundles)
			{
				if(!IsContentBundle(AssetBundleName))
					continue;

				GUILayout.BeginHorizontal();

				GUILayout.Label(AssetBundleName);

				string CurrentPath = GetContentBundleExportPath(AssetBundleName);
				bool HasPath = !string.IsNullOrEmpty(CurrentPath);
				
				if(GUILayout.Button(new GUIContent("...", CurrentPath)))
				{
					string NewPath = EditorUtility.OpenFolderPanel("Content Bundle", CurrentPath, "");
					SetContentBundleExportPath(AssetBundleName, NewPath);
				}

				bool WasEnabled = GUI.enabled;
				GUI.enabled = HasPath && WasEnabled;
				if(GUILayout.Button("Export"))
				{
					BuildContentBundle(AssetBundleName);
				}
				GUI.enabled = WasEnabled;

				GUILayout.EndHorizontal();
			}
		}

		protected virtual void OnGUI()
		{
			ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
			OnGUI_ContentBundles();
			GUILayout.EndScrollView();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Content Tool");
        }
	}
}
