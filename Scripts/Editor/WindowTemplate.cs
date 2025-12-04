using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.Common
{
#if UNITY_EDITOR
    public class WindowTemplate : EditorWindow
    {
        #region Private Fields
        private Vector2 _scrollPos;
        private Vector2 _scrollPosSection1;
        private Vector2 _scrollPosSection2;
        private GUIStyle _centeredButtonStyle;
        private GUIStyle _centeredLabelStyle;
        #endregion

        #region Menu Item
        [MenuItem("NamPhuThuy/Common/Window Template")]
        public static void ShowWindow()
        {
            WindowTemplate window = GetWindow<WindowTemplate>("Window Template");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }
        #endregion

        #region Unity Callbacks
        private void OnEnable()
        {
            // Initialize data when window opens
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
            
            // Main scroll view that wraps everything
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
            if (_centeredButtonStyle == null)
            {
                _centeredButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 14,
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
        }

        private void DrawHeader()
        {
            GUILayout.Label("Window Template", _centeredLabelStyle);
            EditorGUILayout.HelpBox("Description of what this window does.", MessageType.Info);
        }

        private void DrawContent()
        {
            // Section 1
            GUILayout.Label("Section 1", _centeredLabelStyle);
            _scrollPosSection1 = EditorGUILayout.BeginScrollView(_scrollPosSection1, GUILayout.Height(250));
            
            // Add content here - example list
            for (int i = 0; i < 20; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Item [{i}]", GUILayout.Width(60));
                EditorGUILayout.TextField($"Value {i}");
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    // Remove action
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();

            GUILayout.Space(16);

            // Section 2
            GUILayout.Label("Section 2", _centeredLabelStyle);
            _scrollPosSection2 = EditorGUILayout.BeginScrollView(_scrollPosSection2, GUILayout.Height(250));
            
            // Add content here - example list
            for (int i = 0; i < 15; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Item [{i}]", GUILayout.Width(60));
                EditorGUILayout.ObjectField(null, typeof(GameObject), false);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    // Remove action
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
        }

        private void DrawButtons()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Action 1", _centeredButtonStyle, GUILayout.Height(30)))
            {
                // Button action
            }

            if (GUILayout.Button("Action 2", _centeredButtonStyle, GUILayout.Height(30)))
            {
                // Button action
            }

            GUILayout.EndHorizontal();
        }
        #endregion
    }
#endif
}
