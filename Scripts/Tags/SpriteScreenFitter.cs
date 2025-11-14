using System;
using DG.Tweening;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.Common
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteScreenFitter : MonoBehaviour
    {
        public enum FitMode
        {
            MaintainAspectRatio,
            StretchToFit
        }

        #region MonoBehaviour Callbacks

        [Header("Components")]
        [SerializeField] private Camera currentCamera;
        public Camera CurrentCamera => currentCamera;

        [Header("Settings")]
        [SerializeField] private FitMode fitMode = FitMode.MaintainAspectRatio;
        public FitMode CurrentFitMode => fitMode;

        private void Awake()
        {
            if (currentCamera == null)
            {
                currentCamera = Camera.main;
            }
            transform.DOKill(); // Kills all DOTween animations on this transform
            ScaleSpriteToScreen();
        }

        #endregion

        #region Private Methods

        private void ScaleSpriteToScreen()
        {
            DebugLogger.Log();
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            if (sr.sprite == null)
            {
                Debug.LogWarning("SpriteAutoScaler: No sprite assigned to SpriteRenderer.");
                return;
            }

            Vector2 spriteSize = sr.sprite.bounds.size;

            float screenHeight = currentCamera.orthographicSize * 2f;
            float screenWidth = screenHeight * Screen.width / Screen.height;

            float scaleX = screenWidth / spriteSize.x;
            float scaleY = screenHeight / spriteSize.y;

            if (fitMode == FitMode.MaintainAspectRatio)
            {
                // Use the larger scale to ensure full coverage
                float finalScale = Mathf.Max(scaleX, scaleY);
                transform.localScale = new Vector3(finalScale, finalScale, 1f);
            }
            else // StretchToFit
            {
                // Use independent X and Y scales to stretch the sprite
                transform.localScale = new Vector3(scaleX, scaleY, 1f);
            }
        }

        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SpriteScreenFitter))]
    public class SpriteAutoScalerEditor : Editor
    {
        private SerializedProperty currentCameraProperty;
        private SerializedProperty fitModeProperty;
        private SpriteScreenFitter _script;

        private void OnEnable()
        {
            _script = (SpriteScreenFitter)target;
            currentCameraProperty = serializedObject.FindProperty("currentCamera");
            fitModeProperty = serializedObject.FindProperty("fitMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(currentCameraProperty);

            if (currentCameraProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Camera is not assigned. Will use Camera.main at runtime.", MessageType.Info);
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(fitModeProperty);

            if (fitModeProperty.enumValueIndex == (int)SpriteScreenFitter.FitMode.MaintainAspectRatio)
            {
                EditorGUILayout.HelpBox("Sprite will maintain aspect ratio and cover the entire screen. May crop edges.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Sprite will stretch to exactly fit the screen. May appear distorted.", MessageType.Warning);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Preview Scale"))
            {
                SpriteScreenFitter scaler = (SpriteScreenFitter)target;
                scaler.SendMessage("ScaleSpriteToScreen", SendMessageOptions.DontRequireReceiver);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
