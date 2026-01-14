using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NamPhuThuy.Common
{
#if UNITY_EDITOR
    public class Window_ScriptsModifier : EditorWindow
    {
        #region Private Fields
    
        
       

        private Vector2 _scrollPos;
        private GUIStyle _centeredLabelStyle;
        private GUIStyle _centeredButtonStyle;
        #endregion

        #region Menu Item
        [MenuItem("NamPhuThuy/Common/Window - Scripts Modifier")]
        public static void ShowWindow()
        {
            Window_ScriptsModifier window = GetWindow<Window_ScriptsModifier>("Scripts Modifier");
            window.minSize = new Vector2(400, 400);
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
            DrawNamespaceSection();
            GUILayout.Space(20);
            DrawArchivingSection();

            EditorGUILayout.EndScrollView();
            
            GUILayout.EndArea();
        }
        #endregion

        #region Initialize

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
            GUILayout.Label("Scripts Modification Tool", _centeredLabelStyle);
            EditorGUILayout.HelpBox(
                "This tool can perform file modifications that CANNOT be undone with Ctrl+Z. " +
                "Please use version control (like Git) to revert changes if needed.",
                MessageType.Warning);
        }

        private void DrawNamespaceSection()
        {
            GUILayout.Label("Namespace Refactoring", EditorStyles.boldLabel);
            _targetFolder = (DefaultAsset)EditorGUILayout.ObjectField("Target Folder", _targetFolder, typeof(DefaultAsset), false);
            _oldNamespace = EditorGUILayout.TextField("Old Namespace", _oldNamespace);
            _newNamespace = EditorGUILayout.TextField("New Namespace", _newNamespace);
            
            if (GUILayout.Button("Change Namespaces", _centeredButtonStyle, GUILayout.Height(30)))
            {
                ChangeNamespaces();
            }
        }

        private void DrawArchivingSection()
        {
            GUILayout.Label("Script Archiving", EditorStyles.boldLabel);
            _archiveFolder = (DefaultAsset)EditorGUILayout.ObjectField("Target Folder", _archiveFolder, typeof(DefaultAsset), false);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Comment Out", _centeredButtonStyle, GUILayout.Height(30)))
            {
                CommentOutScripts();
            }
            if (GUILayout.Button("Un-comment", _centeredButtonStyle, GUILayout.Height(30)))
            {
                UncommentScripts();
            }
            GUILayout.EndHorizontal();
        }
        #endregion
        

        #region Namespace Changer
        
        private DefaultAsset _targetFolder;
        private string _oldNamespace = "Old.Namespace";
        private string _newNamespace = "New.Namespace";
        

        private void ChangeNamespaces()
        {
            if (_targetFolder == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a target folder for namespace changing.", "OK");
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
                    $"Are you sure you want to replace namespace '{_oldNamespace}' with '{_newNamespace}' in all scripts inside '{path}'?",
                    "Yes, Change Namespaces", "Cancel"))
            {
                ProcessFolderForNamespaceChange(path);
            }
        }

     

        private void ProcessFolderForNamespaceChange(string path)
        {
            string[] scriptFiles = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            int filesChanged = 0;
            string pattern = @"(^\s*namespace\s+)" + Regex.Escape(_oldNamespace) + @"(?=\s*[\r\n{])";
            string replacement = @"$1" + _newNamespace;

            foreach (string filePath in scriptFiles)
            {
                string content = File.ReadAllText(filePath);
                if (Regex.IsMatch(content, pattern, RegexOptions.Multiline))
                {
                    string newContent = Regex.Replace(content, pattern, replacement, RegexOptions.Multiline);
                    File.WriteAllText(filePath, newContent);
                    filesChanged++;
                }
            }
    
            if (filesChanged > 0)
            {
                EditorUtility.DisplayDialog("Success", $"Successfully changed the namespace in {filesChanged} files.", "OK");
                AssetDatabase.Refresh();
            }
            else
            {
                EditorUtility.DisplayDialog("No Changes", $"No scripts with the exact namespace '{_oldNamespace}' were found.", "OK");
            }
        }

        
        #endregion

        #region Comment Scripts
        
        private DefaultAsset _archiveFolder;
        private const string COMMENT_PLACEHOLDER = "#1#";

        private void CommentOutScripts()
        {
            if (_archiveFolder == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a folder to comment out.", "OK");
                return;
            }
            string path = AssetDatabase.GetAssetPath(_archiveFolder);
            if (EditorUtility.DisplayDialog("Confirm Comment Out Scripts",
                    $"Are you sure you want to comment out all .cs files inside '{path}' using multi-line comments?",
                    "Yes, Comment Out", "Cancel"))
            {
                ProcessFolderForCommenting(path, true);
            }
        }

        private void UncommentScripts()
        {
            if (_archiveFolder == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a folder to un-comment.", "OK");
                return;
            }
            string path = AssetDatabase.GetAssetPath(_archiveFolder);
            if (EditorUtility.DisplayDialog("Confirm Un-comment Scripts",
                    $"Are you sure you want to remove multi-line comments from all .cs files inside '{path}'?",
                    "Yes, Un-comment", "Cancel"))
            {
                ProcessFolderForCommenting(path, false);
            }
        }
        
        private void ProcessFolderForCommenting(string path, bool shouldComment)
        {
            string[] scriptFiles = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            int filesChanged = 0;

            foreach (string filePath in scriptFiles)
            {
                string content = File.ReadAllText(filePath);
                string newContent = content;

                if (shouldComment)
                {
                    // Avoid double-commenting
                    if (!content.StartsWith("/*") && !content.EndsWith("*/"))
                    {
                        // Replace any inner "*/" to prevent syntax errors
                        string escapedContent = content.Replace("*/", COMMENT_PLACEHOLDER);
                        newContent = "/*\n" + escapedContent + "\n*/";
                        filesChanged++;
                    }
                }
                else // Un-comment
                {
                    // Check if the file is actually commented out in this way
                    if (content.StartsWith("/*") && content.EndsWith("*/"))
                    {
                        // Remove the outer '/*' and '*/'
                        string strippedContent = content.Substring(3, content.Length - 6).Trim();
                        // Restore the original "*/" from the placeholder
                        newContent = strippedContent.Replace(COMMENT_PLACEHOLDER, "*/");
                        filesChanged++;
                    }
                }
                
                if (newContent != content)
                {
                    File.WriteAllText(filePath, newContent);
                }
            }

            string action = shouldComment ? "commented out" : "un-commented";
            if (filesChanged > 0)
            {
                EditorUtility.DisplayDialog("Success", $"Successfully {action} {filesChanged} script(s).", "OK");
                AssetDatabase.Refresh();
            }
            else
            {
                EditorUtility.DisplayDialog("No Changes", "No scripts needed to be modified.", "OK");
            }
        }

        #endregion
    }
#endif
}