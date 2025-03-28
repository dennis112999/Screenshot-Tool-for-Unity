using UnityEngine;

namespace Dennis.Tools.ScreenshotTool
{
    public enum ScreenshotFormat
    {
        PNG,
        JPG,
        EXR
    }

    /// <summary>
    /// Represents all configurable settings used for capturing a screenshot,
    /// including resolution, camera, file format, and output behavior.
    /// </summary>
    public class ScreenshotSettings
    {
        /// <summary>
        /// The base width of the screenshot, before applying scale
        /// </summary>
        public int width = Screen.width;

        /// <summary>
        /// The base height of the screenshot, before applying scale
        /// </summary>
        public int height = Screen.height;

        /// <summary>
        /// The scale multiplier applied to width and height
        /// Higher values produce larger images
        /// </summary>
        public int scale = 1;

        /// <summary>
        /// The camera used to render the screenshot
        /// </summary>
        public Camera camera;

        /// <summary>
        /// The full folder path where screenshots will be saved
        /// </summary>
        public string savePath = "";

        /// <summary>
        /// The file format used to save the screenshot (e.g., PNG, JPG, EXR)
        /// </summary>
        public ScreenshotFormat format = ScreenshotFormat.PNG;

        /// <summary>
        /// If true, the screenshot file will be opened automatically after capture
        /// </summary>
        public bool OpenAfterShot = false;

        /// <summary>
        /// The final output width after applying scale
        /// </summary>
        public int FinalWidth => width * scale;

        /// <summary>
        /// The final output height after applying scale
        /// </summary>
        public int FinalHeight => height * scale;
    }

}
