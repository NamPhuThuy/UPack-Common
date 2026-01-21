using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace NamPhuThuy.Common
{
    public class Window_AssetNameModifier : EditorWindow
    {
        #region Private Fields
        private Vector2 _scrollPos;
        private Vector2 _scrollPosAssetList;
        private GUIStyle _centeredButtonStyle;
        private GUIStyle _centeredLabelStyle;
        private GUIStyle _headerStyle;

        // Swap functionality
        private Object _assetA;
        private Object _assetB;

        // Batch rename functionality
        private DefaultAsset _targetFolder;
        private List<Object> _assetsInFolder = new List<Object>();
        private string _baseNamePrefix = "Asset";
        private string _suffixFormat = "_{0:00}"; // _{0:00} gives _01, _02, etc.
        private int _startIndex = 1;
        private bool _includeSubfolders = false;
        private string _filterByExtension = ""; // Empty = all types
        #endregion

        #region Menu Item
        [MenuItem("NamPhuThuy/Common/Window Asset Name Modifier")]
        public static void ShowWindow()
        {
            Window_AssetNameModifier window = GetWindow<Window_AssetNameModifier>("Asset Name Modifier");
            window.minSize = new Vector2(450, 700);
            window.Show();
        }
        #endregion

        #region Unity Callbacks
        private void OnEnable()
        {
            // Initialize when window opens
        }

        private void OnDisable()
        {
            // Cleanup when window closes
        }

        private void OnGUI()
        {
            InitializeStyles();

            float padding = 20f;
            Rect areaRect = new Rect(padding, padding, position.width - 2 * padding, position.height - 2 * padding);

            GUILayout.BeginArea(areaRect);
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            DrawHeader();
            GUILayout.Space(10);
            DrawSwapSection();
            GUILayout.Space(20);
            DrawBatchRenameSection();

            EditorGUILayout.EndScrollView();
            
            GUILayout.EndArea();
        }
        #endregion

        #region Initialization
        private void InitializeStyles()
        {
            if (_centeredButtonStyle == null)
            {
                _centeredButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 13,
                    fontStyle = FontStyle.Bold
                };
            }

            if (_centeredLabelStyle == null)
            {
                _centeredLabelStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 16
                };
            }

            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 14,
                    fontStyle = FontStyle.Bold
                };
            }
        }

        private void DrawHeader()
        {
            GUILayout.Label("Asset Name Modifier", _centeredLabelStyle);
            EditorGUILayout.HelpBox(
                "Tool for swapping asset names and batch renaming assets in folders.\n" +
                "Use Ctrl+Z to undo any rename operations.",
                MessageType.Info);
        }
        #endregion

        #region Swap Section
        private void DrawSwapSection()
        {
            GUILayout.Label("Swap Asset Names", _headerStyle);
            EditorGUILayout.HelpBox("Swap the file names of two selected assets.", MessageType.None);

            _assetA = EditorGUILayout.ObjectField("Asset A", _assetA, typeof(Object), false);
            _assetB = EditorGUILayout.ObjectField("Asset B", _assetB, typeof(Object), false);

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Use Selected Assets", GUILayout.Height(25)))
            {
                AssignSelectedAssets();
            }

            if (GUILayout.Button("Clear", GUILayout.Height(25)))
            {
                _assetA = null;
                _assetB = null;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            if (GUILayout.Button("SWAP NAMES", _centeredButtonStyle, GUILayout.Height(35)))
            {
                PerformSwapWithUndo();
            }
        }
        #endregion

        #region Batch Rename Section
        private void DrawBatchRenameSection()
        {
            GUILayout.Label("Batch Rename Assets", _headerStyle);
            EditorGUILayout.HelpBox("Rename multiple assets in a folder with incremental suffixes.", MessageType.None);

            // Folder selection
            _targetFolder = (DefaultAsset)EditorGUILayout.ObjectField(
                "Target Folder", 
                _targetFolder, 
                typeof(DefaultAsset), 
                false);

            EditorGUILayout.Space(5);

            // Rename settings
            _baseNamePrefix = EditorGUILayout.TextField("Base Name", _baseNamePrefix);
            _suffixFormat = EditorGUILayout.TextField("Suffix Format", _suffixFormat);
            EditorGUILayout.HelpBox(
                "Suffix Format examples:\n" +
                "• _{0:00} → _01, _02, _03...\n" +
                "• _{0:000} → _001, _002, _003...\n" +
                "• _{0} → _1, _2, _3...",
                MessageType.None);

            _startIndex = EditorGUILayout.IntField("Start Index", _startIndex);
            _includeSubfolders = EditorGUILayout.Toggle("Include Subfolders", _includeSubfolders);
            _filterByExtension = EditorGUILayout.TextField("Filter Extension (optional)", _filterByExtension);
            EditorGUILayout.HelpBox("Leave empty for all types, or enter like: .png, .prefab, .asset", MessageType.None);

            EditorGUILayout.Space(8);

            // Action buttons
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Load Selected Assets", GUILayout.Height(30)))
            {
                LoadAssetsFromFolder();
            }

            if (GUILayout.Button("Clear List", GUILayout.Height(30)))
            {
                _assetsInFolder.Clear();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // Asset list display
            if (_assetsInFolder.Count > 0)
            {
                GUILayout.Label($"Assets to Rename ({_assetsInFolder.Count})", EditorStyles.boldLabel);
                
                _scrollPosAssetList = EditorGUILayout.BeginScrollView(_scrollPosAssetList, GUILayout.Height(200));
                
                for (int i = 0; i < _assetsInFolder.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    string currentName = _assetsInFolder[i] != null ? _assetsInFolder[i].name : "NULL";
                    string extension = Path.GetExtension(AssetDatabase.GetAssetPath(_assetsInFolder[i]));
                    string newName = _baseNamePrefix + string.Format(_suffixFormat, _startIndex + i);
                    
                    EditorGUILayout.LabelField($"[{i}]", GUILayout.Width(30));
                    EditorGUILayout.LabelField(currentName, GUILayout.Width(120));
                    EditorGUILayout.LabelField("→", GUILayout.Width(20));
                    EditorGUILayout.LabelField(newName + extension, GUILayout.Width(150));
                    
                    if (GUILayout.Button("X", GUILayout.Width(25)))
                    {
                        RemoveAssetFromListWithUndo(i);
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
                
                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space(10);

                if (GUILayout.Button("APPLY BATCH RENAME", _centeredButtonStyle, GUILayout.Height(40)))
                {
                    PerformBatchRenameWithUndo();
                }
            }
        }
        #endregion

        #region Private Methods - Swap Functionality
        private void PerformSwapWithUndo()
        {
            if (_assetA == null || _assetB == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign both assets!", "OK");
                return;
            }

            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Swap Asset Names");
            int undoGroup = Undo.GetCurrentGroup();

            SwapAssetNames(_assetA, _assetB);

            Undo.CollapseUndoOperations(undoGroup);
        }

        private void SwapAssetNames(Object a, Object b)
        {
            string pathA = AssetDatabase.GetAssetPath(a);
            string pathB = AssetDatabase.GetAssetPath(b);

            if (string.IsNullOrEmpty(pathA) || string.IsNullOrEmpty(pathB))
            {
                EditorUtility.DisplayDialog("Error", "Invalid asset paths.", "OK");
                return;
            }

            if (pathA == pathB)
            {
                EditorUtility.DisplayDialog("Error", "You selected the same asset twice.", "OK");
                return;
            }

            string nameA = Path.GetFileNameWithoutExtension(pathA);
            string nameB = Path.GetFileNameWithoutExtension(pathB);
            string dirA = Path.GetDirectoryName(pathA);
            string extA = Path.GetExtension(pathA);

            string tempName = nameA + "_temp_swap_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            string tempPath = Path.Combine(dirA, tempName + extA);

            try
            {
                var err1 = AssetDatabase.RenameAsset(pathA, tempName);
                if (!string.IsNullOrEmpty(err1)) throw new System.Exception(err1);

                var err2 = AssetDatabase.RenameAsset(pathB, nameA);
                if (!string.IsNullOrEmpty(err2)) throw new System.Exception(err2);

                var err3 = AssetDatabase.RenameAsset(tempPath, nameB);
                if (!string.IsNullOrEmpty(err3)) throw new System.Exception(err3);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("Success", $"Swapped '{nameA}' ↔ '{nameB}'", "OK");
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("Error", "Failed to swap: " + ex.Message, "OK");
            }
        }

        private void AssignSelectedAssets()
        {
            Object[] selection = Selection.GetFiltered<Object>(SelectionMode.Assets);

            if (selection.Length != 2)
            {
                EditorUtility.DisplayDialog("Invalid Selection", 
                    "Please select exactly 2 assets in the Project window.", "OK");
                return;
            }

            string pathA = AssetDatabase.GetAssetPath(selection[0]);
            string pathB = AssetDatabase.GetAssetPath(selection[1]);

            if (string.IsNullOrEmpty(pathA) || string.IsNullOrEmpty(pathB) || pathA == pathB)
            {
                EditorUtility.DisplayDialog("Error", 
                    "Invalid selection. Please select two different project assets.", "OK");
                return;
            }

            _assetA = selection[0];
            _assetB = selection[1];
        }
        #endregion

        #region Private Methods - Batch Rename Functionality
        private void LoadAssetsFromFolder()
        {
            if (_targetFolder == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a target folder!", "OK");
                return;
            }

            string folderPath = AssetDatabase.GetAssetPath(_targetFolder);
            
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                EditorUtility.DisplayDialog("Error", "Selected object is not a folder!", "OK");
                return;
            }

            _assetsInFolder.Clear();
            
            var selectedObjects = Selection.objects;
            foreach (var obj in selectedObjects)
            {
                // Optional: keep only asset types you care about
                // if (obj is not GameObject && obj is not Sprite && obj is not Texture2D) continue;
                var path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogWarning(message: $"Cant find {obj.name}");
                    continue; 
                }

                if (!_assetsInFolder.Contains(obj))
                {
                    _assetsInFolder.Add(obj);
                }
            }
                
            // Refresh list after changes
            // _reorderableList.list = _assetList;

            /*string[] guids;
            if (_includeSubfolders)
            {
                guids = AssetDatabase.FindAssets("", new[] { folderPath });
            }
            else
            {
                guids = AssetDatabase.FindAssets("", new[] { folderPath })
                    .Where(guid => Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(guid)) == folderPath)
                    .ToArray();
            }

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                
                // Skip folders
                if (AssetDatabase.IsValidFolder(assetPath))
                    continue;

                // Apply extension filter if specified
                if (!string.IsNullOrEmpty(_filterByExtension))
                {
                    string ext = Path.GetExtension(assetPath);
                    if (!ext.Equals(_filterByExtension, System.StringComparison.OrdinalIgnoreCase))
                        continue;
                }

                Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                if (asset != null)
                {
                    _assetsInFolder.Add(asset);
                }
            }*/

            Debug.Log($"Loaded {_assetsInFolder.Count} assets from folder: {folderPath}");
        }

        private void PerformBatchRenameWithUndo()
        {
            if (_assetsInFolder.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "No assets to rename!", "OK");
                return;
            }

            bool confirm = EditorUtility.DisplayDialog(
                "Confirm Batch Rename",
                $"Are you sure you want to rename {_assetsInFolder.Count} assets?",
                "Yes, Rename",
                "Cancel");

            if (!confirm) return;

            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Batch Rename Assets");
            int undoGroup = Undo.GetCurrentGroup();

            int successCount = 0;
            int failCount = 0;

            for (int i = 0; i < _assetsInFolder.Count; i++)
            {
                if (_assetsInFolder[i] == null) continue;

                string assetPath = AssetDatabase.GetAssetPath(_assetsInFolder[i]);
                string extension = Path.GetExtension(assetPath);
                string newName = _baseNamePrefix + string.Format(_suffixFormat, _startIndex + i);

                try
                {
                    string error = AssetDatabase.RenameAsset(assetPath, newName);
                    if (string.IsNullOrEmpty(error))
                    {
                        successCount++;
                    }
                    else
                    {
                        Debug.LogError($"Failed to rename {_assetsInFolder[i].name}: {error}");
                        failCount++;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Exception renaming {_assetsInFolder[i].name}: {ex.Message}");
                    failCount++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Undo.CollapseUndoOperations(undoGroup);

            EditorUtility.DisplayDialog(
                "Batch Rename Complete",
                $"Successfully renamed: {successCount}\nFailed: {failCount}",
                "OK");

            // Refresh the list
            _assetsInFolder.Clear();
        }

        private void RemoveAssetFromListWithUndo(int index)
        {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Remove Asset from List");
            int undoGroup = Undo.GetCurrentGroup();

            Undo.RecordObject(this, "Remove Asset");
            _assetsInFolder.RemoveAt(index);

            Undo.CollapseUndoOperations(undoGroup);
        }
        #endregion
    }
}
#endif