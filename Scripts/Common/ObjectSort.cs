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
        public bool Active = true;

        [SerializeField] SortAnchor sortMode;

        [SerializeField] bool sortByX;

        [SerializeField] float distanceBetweenX;

        [SerializeField] bool sortByY;

        [SerializeField] float distanceBetweenY;

        [SerializeField] bool sortByZ;

        [SerializeField] float distanceBetweenZ;

        public void Sort()
        {
            if (sortByX)
            {
                transform.SortChildByX(sortMode, distanceBetweenX);
            }
            if (sortByY)
            {
                transform.SortChildByY(sortMode, distanceBetweenY);
            }
            if (sortByZ)
            {
                transform.SortChildByZ(sortMode, distanceBetweenZ);
            }
        }
#if UNITY_EDITOR

        public void SortChild()
        {
            if (!Active) return;
            Sort();
            Save();
        }

        public void Save()
        {
            EditorUtility.SetDirty(gameObject);
            AssetDatabase.SaveAssets();
        }

        public void SortByName()
        {
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

        static float GetStartPosBySortMode(SortAnchor mode, int totalChildCount, float distance)
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
}