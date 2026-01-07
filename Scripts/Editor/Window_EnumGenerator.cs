using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NamPhuThuy.Common
{
#if UNITY_EDITOR
    public class Window_EnumGenerator : EditorWindow
    {
        #region Private Fields
        private Vector2 _scrollPos;
        private GUIStyle _centeredLabelStyle;

        private string enumName = "NewEnum";
        private string outputFileName = "NewEnum.cs";
        private string sourceFolderPath = "Assets/";
        private string outputFolderPath = "Assets/Enums/";
        #endregion

        #region Menu Item
        [MenuItem("NamPhuThuy/Common/Enum Generator")]
        public static void ShowWindow()
        {
            Window_EnumGenerator window = GetWindow<Window_EnumGenerator>("Enum Generator");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }
        #endregion

        #region Unity Callbacks
        private void OnGUI()
        {
            InitializeStyles();

            float padding = 20f;
            Rect areaRect = new Rect(padding, padding, position.width - 2 * padding, position.height - 2 * padding);

            GUILayout.BeginArea(areaRect);
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            DrawHeader();
            GUILayout.Space(10);
            DrawContent();
            GUILayout.Space(10);
            DrawButtons();

            EditorGUILayout.EndScrollView();
            
            GUILayout.EndArea();
        }
        #endregion

        #region Private Methods
        private void InitializeStyles()
        {
            if (_centeredLabelStyle == null)
            {
                _centeredLabelStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 16
                };
            }
        }

        private void DrawHeader()
        {
            GUILayout.Label("Enum Generator", _centeredLabelStyle);
            EditorGUILayout.HelpBox("This tool creates a C# enum from the filenames in a specified folder.", MessageType.Info);
        }

        private void DrawContent()
        {
            enumName = EditorGUILayout.TextField("Enum Name", enumName);
            outputFileName = EditorGUILayout.TextField("Output File Name", outputFileName);

            // Source Folder Path
            EditorGUILayout.BeginHorizontal();
            sourceFolderPath = EditorGUILayout.TextField("Source Folder Path", sourceFolderPath);
            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                string path = EditorUtility.OpenFolderPanel("Select Source Folder", Application.dataPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    if (path.StartsWith(Application.dataPath))
                    {
                        sourceFolderPath = "Assets" + path.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Invalid Path", "Please select a folder within the project's Assets directory.", "OK");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            // Output Folder Path
            EditorGUILayout.BeginHorizontal();
            outputFolderPath = EditorGUILayout.TextField("Output Folder Path", outputFolderPath);
            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                string path = EditorUtility.OpenFolderPanel("Select Output Folder", Application.dataPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    if (path.StartsWith(Application.dataPath))
                    {
                        outputFolderPath = "Assets" + path.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Invalid Path", "Please select a folder within the project's Assets directory.", "OK");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawButtons()
        {
            if (GUILayout.Button("Generate Enum", GUILayout.Height(30)))
            {
                GenerateEnum();
            }
        }

        private void GenerateEnum()
        {
            if (string.IsNullOrWhiteSpace(enumName) || string.IsNullOrWhiteSpace(outputFileName) || string.IsNullOrWhiteSpace(sourceFolderPath))
            {
                EditorUtility.DisplayDialog("Error", "All fields must be filled.", "OK");
                return;
            }

            if (!Directory.Exists(sourceFolderPath))
            {
                EditorUtility.DisplayDialog("Error", $"Source folder not found: {sourceFolderPath}", "OK");
                return;
            }

            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            string[] files = Directory.GetFiles(sourceFolderPath);
            if (files.Length == 0)
            {
                EditorUtility.DisplayDialog("Warning", "No files found in the source folder.", "OK");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// This file is auto-generated by the EnumGenerator tool.");
            sb.AppendLine("// Do not modify this file manually.");
            sb.AppendLine();
            sb.AppendLine($"public enum {Sanitize(enumName)}");
            sb.AppendLine("{");
            
            sb.AppendLine("    NONE = 0,");

            int enumValue = 1;
            foreach (string file in files)
            {
                if (Path.GetExtension(file) == ".meta") continue;

                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                string sanitizedName = Sanitize(fileNameWithoutExtension);
                sb.AppendLine($"    {sanitizedName} = {enumValue},");
                enumValue++;
            }

            sb.AppendLine("}");

            string finalPath = Path.Combine(outputFolderPath, outputFileName);
            File.WriteAllText(finalPath, sb.ToString());

            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Success", $"Enum '{enumName}' was successfully generated and saved to '{finalPath}'.", "OK");
        }

        private string Sanitize(string input)
        {
            string sanitized = Regex.Replace(input, @"[^a-zA-Z0-9_]", "");

            if (string.IsNullOrEmpty(sanitized))
            {
                return "_";
            }
            
            if (char.IsDigit(sanitized[0]))
            {
                sanitized = "_" + sanitized;
            }

            return sanitized;
        }
        #endregion
    }
#endif
}