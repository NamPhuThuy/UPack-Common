using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace NamPhuThuy.Common
{
#if UNITY_EDITOR
    public class Window_NamespaceChanger : EditorWindow
    {
        #region Private Fields
        private DefaultAsset _targetFolder;
        private string _oldNamespace = "Old.Namespace";
        private string _newNamespace = "New.Namespace";
        private Vector2 _scrollPos;
        private GUIStyle _centeredLabelStyle;
        private GUIStyle _centeredButtonStyle;
        #endregion

        #region Menu Item
        [MenuItem("NamPhuThuy/Common/Window - Namespace Changer")]
        public static void ShowWindow()
        {
            Window_NamespaceChanger window = GetWindow<Window_NamespaceChanger>("Namespace Changer");
            window.minSize = new Vector2(400, 250);
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
            if (_centeredButtonStyle == null)
            {
                _centeredButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 14,
                    fontStyle = FontStyle.Bold
                };
            }
        }

        private void DrawHeader()
        {
            GUILayout.Label("Namespace Refactoring Tool", _centeredLabelStyle);
            EditorGUILayout.HelpBox(
                "This tool will find and replace namespaces in all C# scripts within the target folder and its subfolders.\n" +
                "IMPORTANT: This action modifies files directly and CANNOT be undone with Ctrl+Z. Please use version control (like Git) to revert changes if needed.",
                MessageType.Warning);
        }

        private void DrawContent()
        {
            _targetFolder = (DefaultAsset)EditorGUILayout.ObjectField("Target Folder", _targetFolder, typeof(DefaultAsset), false);
            _oldNamespace = EditorGUILayout.TextField("Old Namespace", _oldNamespace);
            _newNamespace = EditorGUILayout.TextField("New Namespace", _newNamespace);
        }

        private void DrawButtons()
        {
            if (GUILayout.Button("Change Namespaces", _centeredButtonStyle, GUILayout.Height(30)))
            {
                ChangeNamespaces();
            }
        }

        private void ChangeNamespaces()
        {
            if (_targetFolder == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a target folder.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(_oldNamespace))
            {
                EditorUtility.DisplayDialog("Error", "Old Namespace cannot be empty for this operation.", "OK");
                return;
            }

            string path = AssetDatabase.GetAssetPath(_targetFolder);
            if (!Directory.Exists(path))
            {
                EditorUtility.DisplayDialog("Error", "The selected folder does not exist.", "OK");
                return;
            }

            if (EditorUtility.DisplayDialog("Confirm Namespace Change",
                    $"Are you sure you want to replace namespace '{_oldNamespace}' with '{_newNamespace}' in all scripts inside '{path}'?\n\nThis action cannot be undone via the editor.",
                    "Yes, Change Namespaces", "Cancel"))
            {
                ProcessFolder(path);
            }
        }

        private void ProcessFolder(string path)
        {
            // Find all .cs files in the folder and subfolders
            string[] scriptFiles = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            int filesChanged = 0;

            string oldNamespaceDeclaration = $"namespace {_oldNamespace}";
            string newNamespaceDeclaration = $"namespace {_newNamespace}";

            // Filter out editor scripts to be safe
            var filteredScripts = scriptFiles.Where(p => !p.Replace("\\", "/").Contains("/Editor/")).ToArray();

            foreach (string filePath in filteredScripts)
            {
                string content = File.ReadAllText(filePath);

                // Check if the file contains the old namespace
                if (content.Contains(oldNamespaceDeclaration))
                {
                    string newContent = content.Replace(oldNamespaceDeclaration, newNamespaceDeclaration);
                    File.WriteAllText(filePath, newContent);
                    filesChanged++;
                    Debug.Log($"Changed namespace in: {filePath}");
                }
            }
    
            if (filesChanged > 0)
            {
                EditorUtility.DisplayDialog("Success",
                    $"Successfully changed the namespace in {filesChanged} files. Re-compiling assets now.", "OK");
                AssetDatabase.Refresh();
            }
            else
            {
                EditorUtility.DisplayDialog("No Changes",
                    $"No scripts with the namespace '{_oldNamespace}' were found in the target folder.", "OK");
            }
        }
        #endregion
    }
#endif
}