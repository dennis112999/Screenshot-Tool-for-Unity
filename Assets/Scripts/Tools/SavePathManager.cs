using UnityEditor;
using UnityEngine;

namespace Dennis.Tools.ScreenshotTool
{
    /// <summary>
    /// Manages the save path for screenshots using EditorPrefs
    /// </summary>
    public static class SavePathManager
    {
        private const string EditorPrefKey = "ScreenshotTool_SavePath";

        public static string GetSavePath()
        {
            return EditorPrefs.GetString(EditorPrefKey, Application.dataPath);
        }

        public static string BrowseSavePath()
        {
            try
            {
                string path = EditorUtility.SaveFolderPanel("Select Save Folder", GetSavePath(), Application.dataPath);
                if (!string.IsNullOrEmpty(path))
                {
                    EditorPrefs.SetString(EditorPrefKey, path);
                }
                return path;
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to select or save folder:\n{ex.Message}", "OK");
                return GetSavePath();
            }
        }
    }
}