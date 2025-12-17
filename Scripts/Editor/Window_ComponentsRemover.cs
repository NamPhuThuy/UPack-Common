using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace NamPhuThuy.Common
{
    public class Window_ComponentsRemover : EditorWindow
    {
        #region Enums
        // Extend this enum with any components you often want to remove
        private enum RemovableComponentType
        {
            MESH_COLLIDER          = 0,
            BOX_COLLIDER           = 1,
            SPHERE_COLLIDER        = 2,
            CAPSULE_COLLIDER       = 3,
            BOX_COLLIDER_2D        = 10,
            CIRCLE_COLLIDER_2D     = 11,
            POLYGON_COLLIDER_2D    = 12,
            EDGE_COLLIDER_2D       = 13,
            RIGIDBODY              = 20,
            RIGIDBODY_2D           = 21,
            MESH_RENDERER          = 30,
            SPRITE_RENDERER        = 31,
            TRAIL_RENDERER         = 32,
            LINE_RENDERER          = 33,
            CANVAS_RENDERER        = 34,
            AUDIO_SOURCE           = 40,
            
        }
        #endregion

        #region Private Fields
        private readonly List<GameObject> _targets = new List<GameObject>();

        private bool _includeChildren = true;
        private RemovableComponentType _selectedComponentType = RemovableComponentType.MESH_COLLIDER;

        private Vector2 _scrollPos;          // main scroll view
        private Vector2 _scrollPosTargets;   // scroll for target list

        private GUIStyle _centeredButtonStyle;
        private GUIStyle _centeredLabelStyle;
        #endregion

        #region Menu Item
        [MenuItem("NamPhuThuy/Common/Window - Components Remover")]
        public static void ShowWindow()
        {
            var window = GetWindow<Window_ComponentsRemover>("Components Remover");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }
        #endregion

        #region Unity Callbacks
        private void OnEnable()
        {
            // Initialize data when window opens (if needed)
        }

        private void OnDisable()
        {
            // Cleanup when window closes (if needed)
        }

        private void OnGUI()
        {
            InitializeStyles();

            float padding = 20f;
            Rect areaRect = new Rect(
                padding,
                padding,
                position.width - 2f * padding,
                position.height - 2f * padding
            );

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

        #region UI Drawing
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
            GUILayout.Label("Components Remover", _centeredLabelStyle);
            EditorGUILayout.HelpBox(
                "Manage a list of GameObjects and remove selected component type from them.\n" +
                "Works for regular 2D/3D GameObjects, optionally including children.",
                MessageType.Info
            );
        }

        private void DrawContent()
        {
            // Options section
            GUILayout.Label("Options", _centeredLabelStyle);

            // Component type selection
            _selectedComponentType = (RemovableComponentType)EditorGUILayout.EnumPopup(
                "Component Type",
                _selectedComponentType
            );

            _includeChildren = EditorGUILayout.Toggle("Include Children", _includeChildren);

            GUILayout.Space(16);

            // Targets section
            GUILayout.Label("Targets", _centeredLabelStyle);

            // Buttons for managing list
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Selection"))
            {
                AddCurrentSelection();
            }

            if (GUILayout.Button("Clear All"))
            {
                _targets.Clear();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(8);

            // Scrollable list of targets
            _scrollPosTargets = EditorGUILayout.BeginScrollView(_scrollPosTargets, GUILayout.Height(350));

            for (int i = _targets.Count - 1; i >= 0; i--)
            {
                EditorGUILayout.BeginHorizontal();

                _targets[i] = (GameObject)EditorGUILayout.ObjectField(_targets[i], typeof(GameObject), true);

                if (GUILayout.Button("X", GUILayout.Width(20f)))
                {
                    _targets.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawButtons()
        {
            GUILayout.BeginHorizontal();

            GUI.enabled = _targets.Count > 0;

            if (GUILayout.Button("Remove Selected Components From Targets", _centeredButtonStyle, GUILayout.Height(30)))
            {
                RemoveComponentsFromTargets();
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();
            
            
            GUILayout.Space(6);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Select Children With Component", _centeredButtonStyle, GUILayout.Height(24)))
            {
                SelectChildrenWithSelectedComponent();
            }

            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Logic
        private void AddCurrentSelection()
        {
            GameObject[] selectedObjects = Selection.gameObjects;

            if (selectedObjects == null || selectedObjects.Length == 0)
            {
                Debug.LogWarning("No GameObjects selected to add.");
                return;
            }

            int added = 0;
            foreach (GameObject go in selectedObjects)
            {
                if (go == null) continue;
                if (_targets.Contains(go)) continue;

                _targets.Add(go);
                added++;
            }

            Debug.Log($"Added {added} GameObject(s) to ComponentsRemover list.");
        }

        private Type GetSelectedType()
        {
            switch (_selectedComponentType)
            {
                case RemovableComponentType.MESH_COLLIDER:            return typeof(MeshCollider);
                case RemovableComponentType.BOX_COLLIDER:             return typeof(BoxCollider);
                case RemovableComponentType.SPHERE_COLLIDER:          return typeof(SphereCollider);
                case RemovableComponentType.CAPSULE_COLLIDER:         return typeof(CapsuleCollider);
                case RemovableComponentType.BOX_COLLIDER_2D:          return typeof(BoxCollider2D);
                case RemovableComponentType.CIRCLE_COLLIDER_2D:       return typeof(CircleCollider2D);
                case RemovableComponentType.POLYGON_COLLIDER_2D:      return typeof(PolygonCollider2D);
                case RemovableComponentType.EDGE_COLLIDER_2D:         return typeof(EdgeCollider2D);
                case RemovableComponentType.RIGIDBODY:                return typeof(Rigidbody);
                case RemovableComponentType.RIGIDBODY_2D:             return typeof(Rigidbody2D);
                case RemovableComponentType.MESH_RENDERER:            return typeof(MeshRenderer);
                case RemovableComponentType.SPRITE_RENDERER:          return typeof(SpriteRenderer);
                case RemovableComponentType.AUDIO_SOURCE:             return typeof(AudioSource);
                default:
                    return null;
            }
        }

        private void RemoveComponentsFromTargets()
        {
            if (_targets.Count == 0)
            {
                Debug.LogWarning("No targets to process.");
                return;
            }

            Type selectedType = GetSelectedType();
            if (selectedType == null)
            {
                Debug.LogError("Unsupported component type selected.");
                return;
            }

            int removedCount = 0;

            // Create a single undo group for all deletions
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Components Remover");
            int undoGroup = Undo.GetCurrentGroup();

            foreach (GameObject go in _targets)
            {
                if (go == null) continue;

                Component[] components = _includeChildren
                    ? go.GetComponentsInChildren(selectedType, true)
                    : go.GetComponents(selectedType);

                foreach (Component comp in components)
                {
                    if (comp == null) continue;
                    Undo.DestroyObjectImmediate(comp);
                    removedCount++;
                }
            }

            Undo.CollapseUndoOperations(undoGroup);

            Debug.Log($"Removed {removedCount} instance(s) of `{_selectedComponentType}` from targets.");
        }
        
        
        // --- NEW: select all children that contain the currently selected component type
        private void SelectChildrenWithSelectedComponent()
        {
            Type selectedType = GetSelectedType();
            if (selectedType == null)
            {
                Debug.LogError("Unsupported component type selected.");
                return;
            }

            List<GameObject> result = GetChildrenWithSelectedComponent(selectedType);

            if (result.Count == 0)
            {
                Debug.Log($"No children with `{selectedType.Name}` found in targets.");
                return;
            }

            Selection.objects = result.ToArray();
            Debug.Log($"Selected {result.Count} child GameObject(s) with `{selectedType.Name}`.");
        }

        /// <summary>
        /// Returns a list of all unique child GameObjects (including the targets themselves)
        /// that contain at least one component of the given type.
        /// </summary>
        private List<GameObject> GetChildrenWithSelectedComponent(Type componentType)
        {
            var collected = new List<GameObject>();
            var seen = new HashSet<GameObject>();

            foreach (GameObject root in _targets)
            {
                if (root == null) continue;

                Component[] comps = root.GetComponentsInChildren(componentType, true);
                foreach (Component comp in comps)
                {
                    if (comp == null) continue;

                    GameObject go = comp.gameObject;
                    if (go != null && !seen.Contains(go))
                    {
                        seen.Add(go);
                        collected.Add(go);
                    }
                }
            }

            return collected;
        }
        #endregion
    }
}
#endif
