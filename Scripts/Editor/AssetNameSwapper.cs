using System.IO;
using UnityEngine;
using UnityEditor;

namespace NamPhuThuy.EditorTools
{
    public class AssetNameSwapper : EditorWindow
    {
        // Any asset type
        private Object assetA;
        private Object assetB;

        // Generic ScriptableObject field
        private ScriptableObject scriptableObjectTarget;
        private int indexA;
        private int indexB;

        [MenuItem("NamPhuThuy/Common/Asset Name Swapper")]
        public static void ShowWindow()
        {
            GetWindow<AssetNameSwapper>("Asset Name Swapper");
        }

        #region Callbacks

        private void OnGUI()
        {
            GUILayout.Label("Swap the names of 2 Assets", EditorStyles.boldLabel);

            // Accept any asset type
            assetA = EditorGUILayout.ObjectField("Asset A", assetA, typeof(Object), false);
            assetB = EditorGUILayout.ObjectField("Asset B", assetB, typeof(Object), false);

            EditorGUILayout.Space();

            if (GUILayout.Button("Use Selected (Any Assets)"))
            {
                AssignSelectedAssets();
            }

            if (GUILayout.Button("Clear Fields"))
            {
                assetA = null;
                assetB = null;
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Swap Names"))
            {
                if (assetA == null || assetB == null)
                {
                    EditorUtility.DisplayDialog("Error", "Please assign both assets!", "OK");
                    return;
                }

                SwapAssetNames(assetA, assetB);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // =============================
            // Generic ScriptableObject Section
            // =============================
            GUILayout.Label("Swap Elements in ScriptableObject", EditorStyles.boldLabel);

            scriptableObjectTarget = (ScriptableObject)EditorGUILayout.ObjectField(
                "Target SO",
                scriptableObjectTarget,
                typeof(ScriptableObject),
                false);

            indexA = EditorGUILayout.IntField("Index A", indexA);
            indexB = EditorGUILayout.IntField("Index B", indexB);

            if (GUILayout.Button("Swap Elements (generic)"))
            {
                if (scriptableObjectTarget == null)
                {
                    EditorUtility.DisplayDialog("Error", "Assign a ScriptableObject!", "OK");
                    return;
                }

                SwapScriptableObjectElements(scriptableObjectTarget, indexA, indexB);
            }
        }

        #endregion

        #region Functionalities

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

            string tempName = nameA + "_temp_swap";
            string tempPath = Path.Combine(dirA, tempName + extA);

            try
            {
                if (AssetDatabase.LoadAssetAtPath<Object>(tempPath) != null)
                {
                    AssetDatabase.DeleteAsset(tempPath);
                }

                var err1 = AssetDatabase.RenameAsset(pathA, tempName);
                if (!string.IsNullOrEmpty(err1)) throw new System.Exception(err1);

                var err2 = AssetDatabase.RenameAsset(pathB, nameA);
                if (!string.IsNullOrEmpty(err2)) throw new System.Exception(err2);

                var err3 = AssetDatabase.RenameAsset(tempPath, nameB);
                if (!string.IsNullOrEmpty(err3)) throw new System.Exception(err3);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("Success", $"Swapped asset names '{nameA}' and '{nameB}'", "OK");
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
                EditorUtility.DisplayDialog(
                    "Invalid Selection",
                    "Please select exactly 2 assets in the Project window.",
                    "OK");
                return;
            }

            string pathA = AssetDatabase.GetAssetPath(selection[0]);
            string pathB = AssetDatabase.GetAssetPath(selection[1]);

            if (string.IsNullOrEmpty(pathA) || string.IsNullOrEmpty(pathB))
            {
                EditorUtility.DisplayDialog(
                    "Error",
                    "Selection must be project assets (not scene objects).",
                    "OK");
                assetA = null;
                assetB = null;
                return;
            }

            if (pathA == pathB)
            {
                EditorUtility.DisplayDialog(
                    "Error",
                    "The two selections point to the same asset. Pick two different assets.",
                    "OK");
                assetA = null;
                assetB = null;
                return;
            }

            assetA = selection[0];
            assetB = selection[1];
        }

        #endregion

        #region SO Functions

        private void SwapScriptableObjectElements(ScriptableObject so, int a, int b)
        {
            EditorUtility.DisplayDialog(
                "Info",
                "Generic ScriptableObject swap not implemented yet. Please add your own logic here.",
                "OK");
        }

        #endregion
    }
}
