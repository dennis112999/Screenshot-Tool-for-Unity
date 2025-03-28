#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Dennis.Tools.ScreenshotTool
{
    [ExecuteInEditMode]
    public class ScreenshotToolWindow : EditorWindow
    {
        // Setting Screenshot Setting
        private ScreenshotSettings _screenSetting = new ScreenshotSettings();

        // Camera handling
        private CameraPreviewRenderer _cameraPreviewRenderer = new CameraPreviewRenderer();

        // Scroll view state (for Editor GUI)
        private Vector2 _scrollPosition;

        // GUI Style
        private GUIStyle _miniTitleStyle;
        private GUIStyle _titleStyle;

        [MenuItem("Dennis/Tools/Screenshot Tool Window")]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow<ScreenshotToolWindow>();
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.Show();
            editorWindow.titleContent = new GUIContent("Screenshot Tool Window");
        }

        private void OnEnable()
        {
            _screenSetting.savePath = SavePathManager.GetSavePath();
        }

        private void OnDisable()
        {
            _cameraPreviewRenderer.Dispose();
        }

        private void OnGUI()
        {
            InitGUIStyles();

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Space(10);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true));

            // Setting Title 
            GUILayout.Label("Screenshot Capture", _titleStyle);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            DrawCameraSettingsSection();
            GUILayout.Space(10);

            DrawOutputResolutionSection();
            GUILayout.Space(10);

            DrawCameraPreviewSection();
            GUILayout.Space(10);

            DrawSavePathSettings();
            GUILayout.Space(10);

            DrawScreenshotOptionsSection();

            GUILayout.EndScrollView();
            GUILayout.Space(10);
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Init GUI Styles
        /// </summary>
        private void InitGUIStyles()
        {
            if (_miniTitleStyle == null)
            {
                _miniTitleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 15,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft
                };
                _miniTitleStyle.normal.textColor = Color.white;
            }

            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 40,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }
        }

        /// <summary>
        /// Draws the UI section for configuring output resolution and scale settings
        /// </summary>
        private void DrawOutputResolutionSection()
        {
            EditorGUILayout.LabelField("Resolution Settings", _miniTitleStyle);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.BeginVertical("box");

            // Width Slider
            _screenSetting.width = EditorUIHelper.DrawIntSliderWithLabel(
                "Width", "Set the horizontal resolution of the output image.", _screenSetting.width, 100, 4096);

            // Height Slider
            _screenSetting.height = EditorUIHelper.DrawIntSliderWithLabel(
                "Height", "Set the vertical resolution of the output image.", _screenSetting.height, 100, 4096);

            // Scale Slider
            _screenSetting.scale = EditorUIHelper.DrawIntSliderWithLabel(
                "Scale", "Scale multiplies the resolution without reducing quality.", _screenSetting.scale, 1, 5);

            // Format Selection
            _screenSetting.format = (ScreenshotFormat)EditorGUILayout.EnumPopup(
                new GUIContent("Format", "Select the file format for the output image."),
                _screenSetting.format
            );

            // Info message box
            EditorGUILayout.HelpBox(
                $"The default screenshot mode is Crop.\n" +
                $"Set a suitable width and height for the output.\n" +
                $"The 'Scale' value increases the resolution without reducing image quality.\n\n" +
                $"Current output format: {_screenSetting.format}",
                MessageType.Info
            );

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Draws Camera Preview
        /// </summary>
        private void DrawCameraPreviewSection()
        {
            GUILayout.Label("Camera Preview", _miniTitleStyle);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.BeginVertical("box");

            // Camera Preview Renderer
            _cameraPreviewRenderer.Draw(_screenSetting.camera, _screenSetting.width, _screenSetting.height, _screenSetting.scale);
            EditorGUILayout.EndVertical();
        }

        private void DrawSavePathSettings()
        {
            // Section title
            EditorGUILayout.LabelField("Save Path", _miniTitleStyle);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.BeginVertical("box");

            // Save path field and browse button
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(_screenSetting.savePath);
            if (GUILayout.Button("Browse", GUILayout.MaxWidth(80)))
            {
                _screenSetting.savePath = SavePathManager.BrowseSavePath();
            }
            EditorGUILayout.EndHorizontal();

            // Info message
            EditorGUILayout.HelpBox("Choose the folder where screenshots will be saved.", MessageType.Info);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Draws the camera selection and transparent background _screenSetting UI
        /// </summary>
        private void DrawCameraSettingsSection()
        {
            // Section title
            EditorGUILayout.LabelField("Camera Settings", _miniTitleStyle);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.BeginVertical("box");

            // Camera field: allows the user to assign a Camera for screenshot capture
            _screenSetting.camera = EditorGUILayout.ObjectField(
                "Camera", 
                _screenSetting.camera, 
                typeof(Camera), 
                true
            ) as Camera;

            // Show a warning if no camera is assigned
            if (_screenSetting.camera == null)
            {
                EditorGUILayout.HelpBox("No camera selected.\n" +
                                        "Please assign a camera to use for screenshots.", 
                                        MessageType.Warning
                );
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawScreenshotOptionsSection()
        {
            EditorGUILayout.LabelField(
                    $"Output Resolution: {_screenSetting.width * _screenSetting.scale} x {_screenSetting.height * _screenSetting.scale} px",
                EditorStyles.boldLabel
            );

            // If no camera is assigned, show warning instead of capture button
            if (_screenSetting.camera == null)
            {
                EditorGUILayout.HelpBox("Please assign a camera before taking a screenshot.", MessageType.Warning);
            }
            else
            {
                if (GUILayout.Button("Take Screenshot", GUILayout.MinHeight(60)))
                {
                    if (string.IsNullOrEmpty(_screenSetting.savePath))
                    {
                        _screenSetting.savePath = EditorUtility.SaveFolderPanel(
                                "Path to Save Images", 
                                _screenSetting.savePath, 
                                Application.dataPath
                        );
                    }
                    CaptureScreenshot();
                }
            }

            EditorGUILayout.Space();
            DrawScreenshotToolsUI();
        }

        private void DrawScreenshotToolsUI()
        {
            // Section Title
            EditorGUILayout.LabelField("Saved Screenshot Tools", _miniTitleStyle);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.BeginVertical("box");

            // Toggle: Open file after capture
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Open Screenshot After Capture", GUILayout.Width(220));
            _screenSetting.OpenAfterShot = EditorGUILayout.Toggle(_screenSetting.OpenAfterShot);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            // Create Open Save Folder Button
            if (GUILayout.Button("Open Save Folder", GUILayout.Height(40)))
            {
                if (!string.IsNullOrEmpty(_screenSetting.savePath))
                {
                    Application.OpenURL("file://" + _screenSetting.savePath);
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Path", "The save folder path is empty or doesn't exist.", "OK");
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Executes the screenshot capture process
        /// </summary>
        public void CaptureScreenshot()
        {
            string fileName = ScreenshotCapturer.Capture(_screenSetting);

            if (!string.IsNullOrEmpty(fileName))
            {
                ShowNotification(new GUIContent("Screenshot taken!"));

                if (_screenSetting.OpenAfterShot)
                {
                    Application.OpenURL("file://" + fileName);
                }
            }
        }
    }
}

#endif