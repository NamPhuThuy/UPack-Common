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
        public enum FitMode
        {
            NONE = 0,
            STRETCH_TO_FIT = 1, // Stretch to fill (ignores aspect ratio)
            FIT_INSIDE = 2,      // Fit completely inside screen (letterbox/pillarbox)
            FILL_SCREEN = 3,     // Fill entire screen (may crop sprite)
            FIT_WIDTH = 4,       // Fit to screen width only
            FIT_HEIGHT = 5      // Fit to screen height only
        }
        
        [Header("Flags")]
        [SerializeField] private FitMode fitMode = FitMode.STRETCH_TO_FIT;
        public FitMode CurrentFitMode => fitMode;
        [SerializeField] private bool scaleOnAwake = false;
        [SerializeField] private bool updateContinuously = false;
        [SerializeField] private bool maintainAspectRatio = false;

        [Header("Padding (in Unity units)")]
        [SerializeField] private float paddingHorizontal = 0f;
        [SerializeField] private float paddingVertical = 0f;
        
        [Header("Components")]
        [SerializeField] private Camera currentCamera;
        public Camera CurrentCamera => currentCamera;
        [SerializeField] private SpriteRenderer spriteRenderer;
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        
        #region MonoBehaviour Callbacks

        private void Awake()
        {
            if (currentCamera == null)
            {
                currentCamera = Camera.main;
            }
            
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
          
            if (scaleOnAwake)
            {
                ScaleSpriteToScreen();
            }
        }

        private void Update()
        {
            if (updateContinuously)
            {
                ScaleSpriteToScreen();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Scale sprite to fill a percentage of screen
        /// </summary>
        public void FitToScreenPercentage(float widthPercent, float heightPercent)
        {
            if (currentCamera == null || !currentCamera.orthographic)
                return;
        
            Vector2 screenSize = GetScreenSizeInWorldUnits();
            float targetWidth = screenSize.x * (widthPercent / 100f);
            float targetHeight = screenSize.y * (heightPercent / 100f);
        
            SetWorldSize(targetWidth, targetHeight, maintainAspectRatio);
        }
        
        /// <summary>
        /// Get the current screen size in world units
        /// </summary>
        public Vector2 GetScreenSizeInWorldUnits()
        {
            if (currentCamera == null || !currentCamera.orthographic)
                return Vector2.zero;
        
            float height = currentCamera.orthographicSize * 2f;
            float width = height * currentCamera.aspect;
            return new Vector2(width, height);
        }
    
        /// <summary>
        /// Get the current sprite size in world units (after scaling)
        /// </summary>
        public Vector2 GetSpriteSizeInWorldUnits()
        {
            if (spriteRenderer == null || spriteRenderer.sprite == null)
                return Vector2.zero;
        
            Sprite sprite = spriteRenderer.sprite;
            float width = (sprite.rect.width / sprite.pixelsPerUnit) * transform.localScale.x;
            float height = (sprite.rect.height / sprite.pixelsPerUnit) * transform.localScale.y;
            return new Vector2(width, height);
        }
        
        /// <summary>
        /// Scale sprite to specific world dimensions
        /// </summary>
        public void SetWorldSize(float width, float height, bool maintainAspect = true)
        {
            if (spriteRenderer == null || spriteRenderer.sprite == null)
                return;
        
            Sprite sprite = spriteRenderer.sprite;
            float spriteWidth = sprite.rect.width / sprite.pixelsPerUnit;
            float spriteHeight = sprite.rect.height / sprite.pixelsPerUnit;
        
            float scaleX = width / spriteWidth;
            float scaleY = height / spriteHeight;
        
            if (maintainAspect)
            {
                float uniformScale = Mathf.Min(scaleX, scaleY);
                transform.localScale = new Vector3(uniformScale, uniformScale, 1f);
            }
            else
            {
                transform.localScale = new Vector3(scaleX, scaleY, 1f);
            }
        }


        #endregion

        #region Private Methods

        public void ScaleSpriteToScreen()
        {
            // DebugLogger.Log(context:this);
            if (!currentCamera.orthographic)
            {
                Debug.LogWarning("Camera is not orthographic! This script is designed for orthographic cameras.");
                return;
            }

            // float screenHeight = currentCamera.orthographicSize * 2f;
            float cameraHeight = currentCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * currentCamera.aspect;
            
            // Apply padding
            float targetWidth = cameraWidth - (paddingHorizontal * 2f);
            float targetHeight = cameraHeight - (paddingVertical * 2f);
            
            // Get sprite dimensions in world units (based on PPU)
            Sprite sprite = spriteRenderer.sprite;
            float spriteWidth = sprite.rect.width / sprite.pixelsPerUnit;
            float spriteHeight = sprite.rect.height / sprite.pixelsPerUnit;
            
            Vector3 scale = CalculateScale(spriteWidth, spriteHeight, targetWidth, targetHeight);

            // Apply scale
            transform.localScale = scale;
        
            // Debug info
            /*DebugLogger.Log(message:$"Sprite PPU: {sprite.pixelsPerUnit}");
            Debug.Log($"Sprite Size: {sprite.rect.width}x{sprite.rect.height} pixels");
            DebugLogger.Log(message:$"spriteWidth: {spriteWidth}, spriteHeight: {spriteHeight}", context:this);
            DebugLogger.Log(message:$"targetWidth: {targetWidth}, targetHeight: {targetHeight}", context:this);
            Debug.Log($"Camera View: {cameraWidth}x{cameraHeight} units");
            DebugLogger.Log(message:$"Applied Scale: {scale}");
            Debug.Log($"Final World Size: {spriteWidth * scale.x}x{spriteHeight * scale.y} units");*/
        }

        /// <summary>
        /// Calculate the appropriate scale based on fit mode
        /// </summary>
        private Vector3 CalculateScale(float spriteWidth, float spriteHeight, float targetWidth, float targetHeight)
        {
            float scaleX = targetWidth / spriteWidth;
            float scaleY = targetHeight / spriteHeight;
        
            switch (fitMode)
            {
                case FitMode.FIT_INSIDE:
                    // Use the smaller scale to fit completely inside
                    if (maintainAspectRatio)
                    {
                        float uniformScale = Mathf.Min(scaleX, scaleY);
                        return new Vector3(uniformScale, uniformScale, 1f);
                    }
                    return new Vector3(scaleX, scaleY, 1f);
                
                case FitMode.FILL_SCREEN:
                    // Use the larger scale to fill screen completely
                    if (maintainAspectRatio)
                    {
                        float uniformScale = Mathf.Max(scaleX, scaleY);
                        return new Vector3(uniformScale, uniformScale, 1f);
                    }
                    return new Vector3(scaleX, scaleY, 1f);
                
                case FitMode.STRETCH_TO_FIT:
                    // Stretch to fill exactly (no aspect ratio)
                    return new Vector3(scaleX, scaleY, 1f);
                
                case FitMode.FIT_WIDTH:
                    // Fit to width only
                    if (maintainAspectRatio)
                        return new Vector3(scaleX, scaleX, 1f);
                    return new Vector3(scaleX, 1f, 1f);
                
                case FitMode.FIT_HEIGHT:
                    // Fit to height only
                    if (maintainAspectRatio)
                        return new Vector3(scaleY, scaleY, 1f);
                    return new Vector3(1f, scaleY, 1f);
                
                default:
                    return Vector3.one;
            }
        }
        #endregion
        
#if UNITY_EDITOR
        [ContextMenu("Fit to Screen Now")]
        private void FitToScreenContextMenu()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (currentCamera == null)
                currentCamera = Camera.main;
        
            ScaleSpriteToScreen();
        }
    
        [ContextMenu("Print Debug Info")]
        private void PrintDebugInfo()
        {
            if (spriteRenderer == null || spriteRenderer.sprite == null || currentCamera == null)
                return;
        
            Sprite sprite = spriteRenderer.sprite;
            Vector2 screenSize = GetScreenSizeInWorldUnits();
            Vector2 spriteSize = GetSpriteSizeInWorldUnits();
        
            Debug.Log("=== Sprite Screen Fitter Debug Info ===");
            Debug.Log($"Sprite: {sprite.name}");
            Debug.Log($"PPU: {sprite.pixelsPerUnit}");
            Debug.Log($"Texture Size: {sprite.rect.width}x{sprite.rect.height} pixels");
            Debug.Log($"Screen Size: {screenSize.x:F2}x{screenSize.y:F2} units");
            Debug.Log($"Sprite World Size: {spriteSize.x:F2}x{spriteSize.y:F2} units");
            Debug.Log($"Current Scale: {transform.localScale}");
            Debug.Log($"Camera Orthographic Size: {currentCamera.orthographicSize}");
            Debug.Log($"Camera Aspect: {currentCamera.aspect}");
        }
#endif
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
           
        }
    }
#endif
}
