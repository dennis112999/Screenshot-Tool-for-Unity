using UnityEditor;
using UnityEngine;

namespace Dennis.Tools.ScreenshotTool
{
    /// <summary>
    /// Renders a live preview of a camera into an editor GUI box
    /// </summary>
    public class CameraPreviewRenderer
    {
        private RenderTexture _previewTexture;
        private const float c_maxPreviewWidth = 300f;
        private const float c_maxPreviewHeight = 200f;

        /// <summary>
        /// Draws a live preview of the specified camera, scaled to fit the defined maximum dimensions
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="resWidth"></param>
        /// <param name="resHeight"></param>
        /// <param name="scale"></param>
        public void Draw(Camera camera, int resWidth, int resHeight, int scale)
        {
            if (camera == null)
            {
                EditorGUILayout.HelpBox("Camera is not assigned. A valid camera must be selected to render the preview.",
                        MessageType.Warning);
                return;
            }

            int scaledWidth = resWidth * scale;
            int scaledHeight = resHeight * scale;

            // Calculate preview dimensions while preserving aspect ratio
            float aspect = (float)scaledWidth / scaledHeight;
            float previewWidth = c_maxPreviewWidth;
            float previewHeight = previewWidth / aspect;

            // Clamp preview height if it exceeds max height
            if (previewHeight > c_maxPreviewHeight)
            {
                previewHeight = c_maxPreviewHeight;
                previewWidth = previewHeight * aspect;
            }

            // Recreate RenderTexture if size has changed
            if (_previewTexture == null ||
                _previewTexture.width != (int)previewWidth ||
                _previewTexture.height != (int)previewHeight)
            {
                if (_previewTexture != null)
                    _previewTexture.Release();

                _previewTexture = new RenderTexture((int)previewWidth, (int)previewHeight, 16);
            }

            // Render the camera output to the preview texture
            camera.targetTexture = _previewTexture;
            camera.Render();
            camera.targetTexture = null;

            // Draw the preview box
            DrawPreviewBox(_previewTexture, previewWidth, previewHeight);
        }

        /// <summary>
        /// Draw Preview Box
        /// </summary>
        private void DrawPreviewBox(Texture texture, float width, float height)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box(texture, GUILayout.Width(width), GUILayout.Height(height));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public void Dispose()
        {
            if (_previewTexture == null) return;

            _previewTexture.Release();
            _previewTexture = null;
        }
    }
}
