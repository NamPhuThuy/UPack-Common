using System.Collections.Generic;
using NamPhuThuy.Common;
using UnityEngine;

namespace NamPhuThuy.Common
{
    public static class TransformExtension
    {
        #region Position Helpers

        public static void SetLocalPositionX(this Transform trans, float value)
        {
            var v = trans.localPosition;
            v.x = value;
            trans.localPosition = v;
        }

        public static void SetLocalPositionY(this Transform trans, float value)
        {
            var v = trans.localPosition;
            v.y = value;
            trans.localPosition = v;
        }

        public static void SetLocalPositionZ(this Transform trans, float value)
        {
            var v = trans.localPosition;
            v.z = value;
            trans.localPosition = v;
        }

        public static void SetPositionX(this Transform trans, float value)
        {
            var v = trans.position;
            v.x = value;
            trans.position = v;
        }

        public static void SetPositionY(this Transform trans, float y)
        {
            var v = trans.position;
            v.y = y;
            trans.position = v;
        }

        public static void SetPositionZ(this Transform trans, float value)
        {
            var v = trans.position;
            v.z = value;
            trans.position = v;
        }
        #endregion

        #region Scale Helpers

        public static void SetLocalScaleX(this Transform trans, float value)
        {
            var v = trans.localScale;
            v.x = value;
            trans.localScale = v;
        }

        public static void SetLocalScaleY(this Transform trans, float value)
        {
            var v = trans.localScale;
            v.y = value;
            trans.localScale = v;
        }

        public static void SetLocalScaleZ(this Transform trans, float value)
        {
            var v = trans.localScale;
            v.z = value;
            trans.localScale = v;
        }

        #endregion

        #region Rotation Helpers

        public static void SetEulerAngleZ(this Transform trans, float value)
        {
            var v = trans.eulerAngles;
            v.z = value;
            trans.eulerAngles = v;
        }

        public static void SetEulerAngleY(this Transform trans, float value)
        {
            var v = trans.eulerAngles;
            v.y = value;
            trans.eulerAngles = v;
        }

        public static void SetEulerAngleX(this Transform trans, float value)
        {
            var v = trans.eulerAngles;
            v.x = value;
            trans.eulerAngles = v;
        }

        public static void SetLocalEulerAngleX(this Transform trans, float value)
        {
            var v = trans.localEulerAngles;
            v.x = value;
            trans.localEulerAngles = v;
        }

        public static void SetLocalEulerAngleY(this Transform trans, float value)
        {
            var v = trans.localEulerAngles;
            v.y = value;
            trans.localEulerAngles = v;
        }

        /// <summary>
        /// Sets only the local Z angle of the Transform's rotation, preserving X and Y.
        /// </summary>
        /// <param name="trans">The Transform to modify.</param>
        /// <param name="value">The new Z angle in degrees.</param>
        public static void SetLocalEulerAngleZ(this Transform trans, float value)
        {
            var v = trans.localEulerAngles;
            v.z = value;
            trans.localEulerAngles = v;
        }

        #endregion

        #region Find Object Helpers

        public static List<GameObject> FindObjectsByNameContain(this Transform transform, string name)
        {
            List<GameObject> taggedGameObjects = new List<GameObject>();

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name.Contains(name))
                {
                    taggedGameObjects.Add(child.gameObject);
                }

                if (child.childCount > 0)
                {
                    taggedGameObjects.AddRange(FindObjectsByNameContain(child, name));
                }
            }

            return taggedGameObjects;
        }

        public static GameObject FindObjectByNameContain(this Transform transform, string name)
        {
            Queue<Transform> check = transform.ChildsQueue();
            while (check.Count > 0)
            {
                var child = check.Dequeue();
                if (child.name.Contains(name))
                {
                    return child.gameObject;
                }
                else
                {
                    check.AddRange(child.ChildsQueue());
                }
            }

            return null;
        }

        public static GameObject FindObjectByName(this Transform transform, string name)
        {
            Queue<Transform> check = transform.ChildsQueue();
            while (check.Count > 0)
            {
                var child = check.Dequeue();
                if (child.name.Equals(name))
                {
                    return child.gameObject;
                }
                else
                {
                    check.AddRange(child.ChildsQueue());
                }
            }

            return null;
        }

        public static GameObject FindObjectWithTag(this Transform transform, string tag)
        {
            Queue<Transform> check = transform.ChildsQueue();
            while (check.Count > 0)
            {
                var child = check.Dequeue();
                if (child.CompareTag(tag))
                {
                    return child.gameObject;
                }
                else
                {
                    check.AddRange(child.ChildsQueue());
                }
            }

            return null;
        }

        #endregion

        #region Children helpers
        
        public static Queue<Transform> ChildsQueue(this Transform transform)
        {
            Queue<Transform> result = new Queue<Transform>();

            for (int i = 0; i < transform.childCount; i++)
            {
                result.Enqueue(transform.GetChild(i));
            }

            return result;
        }

        public static List<Transform> ChildsList(this Transform transform)
        {
            List<Transform> result = new List<Transform>();

            for (int i = 0; i < transform.childCount; i++)
            {
                result.Add(transform.GetChild(i));
            }

            return result;
        }

        /// <summary>
        /// Returns the index of this Transform among all its siblings that have component T.
        /// Only siblings with component T are counted.
        /// Returns -1 if this Transform has no parent or no component T.
        /// </summary>
        public static int GetSiblingIndexWithComponent<T>(this Transform transform) where T : Component
        {
            var parent = transform.parent;
            if (parent == null)
            {
                return -1;
            }

            // This object must also have T, otherwise it is not in the filtered set.
            if (transform.GetComponent<T>() == null)
            {
                return -1;
            }

            int filteredIndex = 0;
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);

                // Count only children that have T
                if (child.GetComponent<T>() != null)
                {
                    // When we reach `transform`, return the current filtered index
                    if (child == transform)
                    {
                        return filteredIndex;
                    }

                    filteredIndex++;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets components of type T from this transform and all its children.
        /// This is a more convenient wrapper for the built-in GetComponentsInChildren.
        /// </summary>
        /// <param name="parent">The parent transform to search from.</param>
        /// <param name="includeInactive">Whether to include components on inactive GameObjects.</param>
        /// <typeparam name="T">The type of component to find.</typeparam>
        /// <returns>An array of components of type T found.</returns>
        public static T[] GetComponentsFromAllChildren<T>(this Transform parent, bool includeInactive = true) where T : Component
        {
            List<T> components = new List<T>();
            GetComponentsFromAllChildrenRecursive(parent, components);
            return components.ToArray();
        }
        
        private static void GetComponentsFromAllChildrenRecursive<T>(Transform parent, List<T> components) where T : Component
        {
            T component = parent.GetComponent<T>();
            if (component != null)
            {
                components.Add(component);
            }

            foreach (Transform child in parent)
            {
                GetComponentsFromAllChildrenRecursive<T>(child, components);
            }
        }
        
        #endregion
        
        public static bool IsInRange(this Transform transform, Vector3 target, float range)
        {
            return transform.position.IsInRange(target, range);
        }

        public static List<Transform> OutRange(this List<Transform> transforms, Vector3 position, float distance)
        {
            List<Transform> result = new List<Transform>();
            foreach (var item in transforms)
            {
                var inRange = (item.position - position).sqrMagnitude > distance * distance;
                if (inRange)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public static List<Transform> InRange(this List<Transform> transforms, Vector3 position, float minDistance,
            float maxDistance)
        {
            List<Transform> result = new List<Transform>();
            foreach (var item in transforms)
            {
                float distanceSq = (item.position - position).sqrMagnitude;
                float minDistanceSq = minDistance * minDistance;
                float maxDistanceSq = maxDistance * maxDistance;
                if (distanceSq > minDistanceSq && distanceSq < maxDistanceSq)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        #region Character Look

        public static void CharacterLook(this Transform transform, Transform target)
        {
            transform.rotation = transform.GetCharacterLookRotation(target);
        }

        public static Quaternion GetCharacterLookRotation(this Transform transform, Transform target)
        {
            return GetCharacterLookRotation(transform, target.position);
        }

        public static Quaternion GetCharacterLookRotation(this Transform transform, Vector3 target)
        {
            var targetPos = target;
            var currentPos = transform.position;
            targetPos.y = 0;
            currentPos.y = 0;
            var dir = targetPos - currentPos;
            var targetRotation = Quaternion.LookRotation(dir);
            return targetRotation;
        }

        #endregion

        #region Nearest / Furthest
        
        /// <summary>
        /// Find the nearest Component of type T from the list to the target position.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Nearest<T>(this IEnumerable<T> list, Vector3 target) where T : Component
        {
            T best = null;
            float bestDistance = float.MaxValue;
            foreach (var item in list)
            {
                float distance = Vector3.Distance(target, item.transform.position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = item;
                }
            }

            return best;
        }

        public static T Furthest<T>(this IEnumerable<T> list, Vector3 target) where T : Component
        {
            T best = null;
            float bestDistance = float.MaxValue;
            foreach (var item in list)
            {
                float distance = Vector3.Distance(target, item.transform.position);
                if (distance > bestDistance)
                {
                    bestDistance = distance;
                    best = item;
                }
            }

            return best;
        }
        
        #endregion
        
    }
}