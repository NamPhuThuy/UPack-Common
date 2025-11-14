using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace NamPhuThuy.Common
{
    [RequireComponent(typeof(SpriteRenderer))]

    public class SpriteScreenFitter : MonoBehaviour
    {
        #region MonoBehaviour Callbacks
        
        [Header("Components")]
        [SerializeField] private Camera currentCamera;
        public Camera CurrentCamera => currentCamera;

        private void Awake()
        {
            if (currentCamera == null)
            {
                currentCamera = Camera.main;
            }
        }

        private void Start()
        {
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

            // Maintain aspect ratio 
            Vector2 spriteSize = sr.sprite.bounds.size;

            float screenHeight = currentCamera.orthographicSize * 2f;
            float screenWidth = screenHeight * Screen.width / Screen.height;

            float scaleX = screenWidth / spriteSize.x;
            float scaleY = screenHeight / spriteSize.y;

            float finalScale = Mathf.Max(scaleX, scaleY);

            transform.localScale = new Vector3(finalScale, finalScale, 1f);
        }

        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SpriteScreenFitter))]
    public class SpriteAutoScalerEditor : Editor
    {
        private SerializedProperty currentCameraProperty;
        private SpriteScreenFitter _script;

        private void OnEnable()
        {
            _script = (SpriteScreenFitter)target;
            currentCameraProperty = serializedObject.FindProperty("currentCamera");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(currentCameraProperty);

            if (currentCameraProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Camera is not assigned. Will use Camera.main at runtime.", MessageType.Info);
            }

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
