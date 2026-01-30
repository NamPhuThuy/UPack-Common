#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

//Use backtracking to show the path of the current clicked UIElement
namespace NamPhuThuy.Common
{
    public class UIClickDetector : MonoBehaviour
    {
        
        private EventSystem eventSystem;
        
        
        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                if (results.Count > 0)
                {
                    GameObject clickedObject = results[0].gameObject;
                    string path = GetHierarchyPath(clickedObject.transform);
                    DebugLogger.Log(message:$"Path: {path}", context:clickedObject);
                }
            }
        }

        private string GetHierarchyPath(Transform transform)
        {
            if (transform.parent == null)
            {
                return transform.name;
            }

            return GetHierarchyPath(transform.parent) + $"-> {transform.name}";
        }
        
        /*private bool IsPointerOverUI()
        {
            // This is the most reliable way to check for UI clicks with the new Input System.
            // IsPointerOverGameObject() can be unreliable for touch inputs.
            if (eventSystem == null)
            {
                return false;
            }

            // Create a pointer event data object for the current pointer position.
            var eventData = new PointerEventData(eventSystem)
            {
                position = Pointer.current.position.ReadValue()
            };

            // Create a list to receive all raycast results.
            var results = new List<RaycastResult>();

            // Raycast against all UI elements.
            eventSystem.RaycastAll(eventData, results);

            // If the list has one or more results, the pointer is over a UI element.
            return results.Count > 0;
        }*/
    }

    [CustomEditor(typeof(UIClickDetector))]
    public class ClickDetectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.BeginHorizontal();
            GUILayout.Space(40); // Left margin
            // Display the description
            GUILayout.Label("Enable this component if you want to Debug the 'location on the Hierarchy tree' of the game object you just clicked",  new GUIStyle() { wordWrap = true, normal = {textColor = Color.black}} );
            GUILayout.Space(40); // Right margin
            GUILayout.EndHorizontal();
        }
    }
}
#endif
