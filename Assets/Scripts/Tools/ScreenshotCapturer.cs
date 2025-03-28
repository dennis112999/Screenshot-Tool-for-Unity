using System.IO;
using System;

using UnityEngine;

namespace Dennis.Tools.ScreenshotTool
{
    /// <summary>
    /// Screenshot Capturer Tool
    /// </summary>
    public static class ScreenshotCapturer
    {
        /// <summary>
        /// Captures a screenshot using the specified settings
        /// </summary>
        /// <param name="settings">Screenshot Settings</param>
        /// <returns></returns>
        public static string Capture(ScreenshotSettings settings)
        {
            if (settings.camera == null)
            {
#if UNITY_EDITOR
                Debug.LogError("ScreenshotCapturer: Camera is null.");
#endif
                return null;
            }

            // Calculate the final resolution based on width, height, and scale
            int finalWidth = settings.width * settings.scale;
            int finalHeight = settings.height * settings.scale;

            // Create a temporary RenderTexture and Texture2D for capture
            RenderTexture renderTexture = new RenderTexture(finalWidth, finalHeight, 24);
            Texture2D screenshotTexture = new Texture2D(finalWidth, finalHeight, TextureFormat.ARGB32, false);

            try
            {
                // Render the scene from the specified camera into the RenderTexture
                settings.camera.targetTexture = renderTexture;
                settings.camera.Render();

                // Read the rendered image into a Texture2D
                RenderTexture.active = renderTexture;
                screenshotTexture.ReadPixels(new Rect(0, 0, finalWidth, finalHeight), 0, 0);
                screenshotTexture.Apply();

                // Encode the texture into the selected image format (PNG, JPG, EXR)
                byte[] imageData = EncodeScreenshot(screenshotTexture, settings.format, out string extension);

                // Generate a unique file path and write the image to disk
                string filePath = GenerateScreenshotName(settings.savePath, settings.width, settings.height, extension);
                File.WriteAllBytes(filePath, imageData);

                return filePath;
            }
            finally
            {
                // Cleanup and restore camera/render target
                settings.camera.targetTexture = null;
                RenderTexture.active = null;
                UnityEngine.Object.DestroyImmediate(renderTexture);
            }
        }

        /// <summary>
        /// Encodes a Texture2D into a byte array using the specified screenshot format
        /// </summary>
        /// <param name="texture">The texture to encode</param>
        /// <param name="format">The desired screenshot format (PNG, JPG, EXR)</param>
        /// <param name="extension">The output file extension, including the leading dot (e.g., ".png")</param>
        /// <returns>A byte array containing the encoded image data</returns>
        private static byte[] EncodeScreenshot(Texture2D texture, ScreenshotFormat format, out string extension)
        {
            // default fallback
            extension = ".png";

            switch (format)
            {
                case ScreenshotFormat.PNG:
                    extension = ".png";
                    return texture.EncodeToPNG();

                case ScreenshotFormat.JPG:
                    extension = ".jpg";
                    return texture.EncodeToJPG();

                case ScreenshotFormat.EXR:
                    extension = ".exr";
                    return texture.EncodeToEXR();

                default:
#if UNITY_EDITOR
                    Debug.LogWarning("Unsupported format. Defaulting to PNG.");
#endif
                    return texture.EncodeToPNG();
            }
        }

        private static string GenerateScreenshotName(string savePath, int width, int height, string extension)
        {
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            return Path.Combine(savePath, $"screenshot_{width}x{height}_{timeStamp}{extension}");
        }
    }
}
