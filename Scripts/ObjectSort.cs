using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NamPhuThuy.Common
{
    public class ObjectsSort : MonoBehaviour
    {
        [Tooltip("If true, sorting will be applied automatically whenever a value is changed in the Inspector.")]
        public bool liveSortInEditor = true;
        [Tooltip("If true, the sorting buttons will be visible in the Inspector.")]
        public bool showSortButtons = true;

        [SerializeField] SortAnchor sortMode;

        [SerializeField] bool sortByX;

        [SerializeField] float distanceBetweenX;

        [SerializeField] bool sortByY;

        [SerializeField] float distanceBetweenY;

        [SerializeField] bool sortByZ;

        [SerializeField] float distanceBetweenZ;

        [Tooltip("An additional position offset applied to all children after sorting.")]
        [SerializeField] private Vector3 offset;

        public void Sort()
        {
            int childCount = transform.childCount;
            if (childCount == 0) return;

            // Calculate starting positions for each axis based on sort mode
            float startX = sortByX ? SortExtension.GetStartPosBySortMode(sortMode, childCount, distanceBetweenX) : 0;
            float startY = sortByY ? SortExtension.GetStartPosBySortMode(sortMode, childCount, distanceBetweenY) : 0;
            float startZ = sortByZ ? SortExtension.GetStartPosBySortMode(sortMode, childCount, distanceBetweenZ) : 0;

            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);

                // Calculate the target position for this child, preserving the original position for axes that are not being sorted.
                float targetX = sortByX ? startX + (i * distanceBetweenX) : child.localPosition.x;
                float targetY = sortByY ? startY + (i * distanceBetweenY) : child.localPosition.y;
                float targetZ = sortByZ ? startZ + (i * distanceBetweenZ) : child.localPosition.z;

                // Apply the final position with the offset
                child.localPosition = new Vector3(targetX, targetY, targetZ) + offset;
            }
        }

        private void OnValidate()
        {
            if (!liveSortInEditor) return;
            Sort();
        }
        
#if UNITY_EDITOR

        public void Save()
        {
            // SetDirty is sufficient for scene objects. SaveAssets is for project assets and can be slow.
            EditorUtility.SetDirty(gameObject);
        }

        public void SortByName()
        {
            // This will reorder the children in the hierarchy based on their name.
            // Note: The ChildsList() extension method is assumed to be defined in your project.
            Undo.RecordObject(transform, "Sort Children by Name");
            var childs = transform.ChildsList().OrderBy(x => x.name);
            foreach (var item in childs)
            {
                item.SetAsLastSibling();
            }
            Save();
        }
#endif
    }
    public static class SortExtension
    {
        public static void SortChildByX(this Transform target, SortAnchor mode, float distance)
        {
            Sort(target, mode, distance, (transform, value) =>
            {
                transform.SetLocalPositionX(value);
            });
        }

        public static void SortChildByY(this Transform target, SortAnchor mode, float distance)
        {
            Sort(target, mode, distance, (transform, value) =>
            {
                transform.SetLocalPositionY(value);
            });
        }

        public static void SortChildByZ(this Transform target, SortAnchor mode, float distance)
        {
            Sort(target, mode, distance, (transform, value) =>
            {
                transform.SetLocalPositionZ(value);
            });
        }
        static void Sort(Transform tranform, SortAnchor mode, float distance, Action<Transform, float> callback)
        {
            float positionValue = GetStartPosBySortMode(mode, tranform.childCount, distance);
            for (int i = 0; i < tranform.childCount; i++)
            {
                var child = tranform.GetChild(i);
                callback?.Invoke(child, positionValue);
                positionValue += distance;
            }
        }

        public static float GetStartPosBySortMode(SortAnchor mode, int totalChildCount, float distance)
        {
            switch (mode)
            {
                case SortAnchor.FIRST:
                    return 0;
                case SortAnchor.MIDDLE:
                    return -(totalChildCount - 1) * distance / 2f;
                case SortAnchor.LAST:
                default:
                    return -(totalChildCount - 1) * distance;
            }
        }
    }

    public enum SortAnchor
    {
        FIRST = 0,
        MIDDLE = 1,
        LAST = 2
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ObjectsSort))]
    public class ObjectsSortEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector fields (like 'Active', 'sortMode', etc.)
            DrawDefaultInspector();

            ObjectsSort sorter = (ObjectsSort)target;

            // Don't show the button if the component is inactive
            if (!sorter.showSortButtons)
            {
                EditorGUILayout.HelpBox("Sorting buttons are hidden. Check 'Show Sort Buttons' to enable.", MessageType.Info);
                return;
            }

            EditorGUILayout.Space();

            // Add the button to the inspector
            if (GUILayout.Button("Sort Children by Position"))
            {
                // Record all child transforms so the position changes can be undone
                foreach (Transform child in sorter.transform)
                {
                    Undo.RecordObject(child, "Sort Children Position");
                }

                // Call the Sort() method on the component
                sorter.Sort();
            }

            if (GUILayout.Button("Sort Children by Name (Hierarchy)"))
            {
                // Call the SortByName() method on the component
                sorter.SortByName();
            }
        }
    }
#endif
}