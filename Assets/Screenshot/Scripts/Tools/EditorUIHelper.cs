using UnityEditor;
using UnityEngine;

namespace Dennis.Tools.ScreenshotTool
{
    /// <summary>
    /// A utility class that provides helper methods for drawing custom UI elements in the Unity Editor
    /// </summary>
    public static class EditorUIHelper
    {
        /// <summary>
        /// Draws an integer slider with a tooltip and a trailing label showing the current value
        /// </summary>
        /// <param name="label">The label displayed next to the slider</param>
        /// <param name="tooltip">The tooltip shown when hovering over the label</param>
        /// <param name="value">The current value of the slider</param>
        /// <param name="min">The minimum value allowed</param>
        /// <param name="max">The maximum value allowed</param>
        /// <returns>The updated integer value selected by the slider</returns>
        public static int DrawIntSliderWithLabel(string label, string tooltip, int value, int min, int max)
        {
            EditorGUILayout.BeginHorizontal();
            value = EditorGUILayout.IntSlider(new GUIContent(label, tooltip), value, min, max);
            EditorGUILayout.LabelField($"x{value}", GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            return value;
        }
    }
}