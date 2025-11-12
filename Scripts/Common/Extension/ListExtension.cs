using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NamPhuThuy.Common
{
    
    public static class ListExtensions
    {
        #region Randomization

        /// <summary>
        /// Shuffles the list in place using the Fisher–Yates algorithm.
        /// </summary>
        public static void FisherYatesShuffle<T>(this List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        /// <summary>
        /// Returns a new shuffled copy of the list, leaving the original untouched.
        /// </summary>
        public static List<T> FisherYatesShuffled<T>(this List<T> list)
        {
            var copy = new List<T>(list);
            copy.FisherYatesShuffle();
            return copy;
        }
        
        public static T RandomElement<T>(this List<T> list)
        {
            if (list.Count == 0) return default(T);
            return list[Random.Range(0, list.Count)];
        }

        public static int RandomIndex<T>(this List<T> list)
        {
            return Random.Range(0, list.Count);
        }
        
        public static List<T> RandomElements<T>(this List<T> list, int count, bool allowRepeat = false)
        {
            List<T> cloner = list.Clone();
            List<T> randList = new List<T>();
            if (cloner.Count == 0) return randList;
            while (count > 0)
            {
                count--;
                var randElement = cloner.PopRandomElement();
                randList.Add(randElement);
                if (cloner.Count == 0)
                {
                    if (allowRepeat)
                    {
                        cloner = list.Clone();

                    }
                    else
                    {
                        break;
                    }
                }
            }
            return randList;
        }
        
        public static List<T> Mix<T>(this List<T> list)
        {
            List<T> newList = new List<T>();
            var cloneList = list.Clone();
            while (cloneList.Count > 0)
            {
                newList.Add(cloneList.PopRandomElement());
            }
            return newList;
        }

        #endregion

        #region Accessors

        public static T GetElementClamp<T>(this List<T> list, int index)
        {
            if (list.Count == 0) return default(T);
            return list[Mathf.Clamp(index, 0, list.Count - 1)];
        }

        public static int LastIndex<T>(this List<T> list)
        {
            return list.Count - 1;
        }
        
        public static T LastElement<T>(this List<T> list)
        {
            if (list.Count == 0) return default(T);
            return list[list.Count - 1];
        }
        public static T FirstElement<T>(this List<T> list)
        {
            if (list.Count == 0) return default(T);
            return list[0];
        }

        #endregion

        #region Stack-like Operations

        public static T PopLastElement<T>(this List<T> list)
        {
            if (list.Count == 0) return default(T);
            T result = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return result;
        }
        public static T PopFirstElement<T>(this List<T> list)
        {
            if (list.Count == 0) return default(T);
            T result = list[0];
            list.RemoveAt(0);
            return result;
        }

        public static void Swap<T>(this List<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }
        #endregion

        #region Removal & Search

        public static T Remove<T>(this List<T> list, Predicate<T> match)
        {
            int index = list.FindIndex(match);
            if (index != -1)
            {
                T result = list[index];
                list.RemoveAt(index);
                return result;
            }
            return default(T);
        }

        public static bool Contain<T>(this List<T> list, Predicate<T> match)
        {
            foreach (T item in list)
            {
                if (match(item)) return true;
            }
            return false;
        }

        #endregion
        
        /// <summary>
        /// Returns a random subset (distinct elements) of size <paramref name="count"/> from the source list.
        /// If count >= list.Count returns a shallow copy of the whole list.
        /// </summary>
        private static List<T> RandomSubListDistinct<T>(this IList<T> list, int count)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            int n = list.Count;
            if (count <= 0 || n == 0) return new List<T>();
            if (count >= n) return new List<T>(list);

            // Partial Fisher–Yates on indices
            int[] indices = new int[n];
            for (int i = 0; i < n; i++) indices[i] = i;

            var result = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                int r = UnityEngine.Random.Range(i, n);
                (indices[i], indices[r]) = (indices[r], indices[i]);
                result.Add(list[indices[i]]);
            }
            return result;
        }
        
        /// <summary>
        /// Returns a random sublist of size <paramref name="count"/>.
        /// If allowRepeat == true elements may repeat; otherwise distinct.
        /// </summary>
        public static List<T> RandomSubList<T>(this IList<T> list, int count, bool allowRepeat = false)
        {
            if (!allowRepeat) return RandomSubListDistinct(list, count);

            if (list == null) throw new ArgumentNullException(nameof(list));
            int n = list.Count;
            var result = new List<T>(Math.Max(0, count));
            if (count <= 0 || n == 0) return result;

            for (int i = 0; i < count; i++)
            {
                result.Add(list[UnityEngine.Random.Range(0, n)]);
            }
            return result;
        }
        
        
        
        public static List<T> Clone<T>(this List<T> list)
        {
            List<T> temp = new List<T>();
            foreach (var item in list)
            {
                temp.Add(item);
            }
            return temp;
        }


        
        
       
       
        
        public static T PopRandomElement<T>(this List<T> list)
        {
            if (list.Count == 0) return default(T);
            int index = Random.Range(0, list.Count);
            T result = list[index];
            list.RemoveAt(index);
            return result;
        }

       

      
       

       
        public static void Add<T>(this List<T> list, List<T> additionList)
        {
            foreach (var item in additionList)
            {
                list.Add(item);
            }
        }
        public static List<T> ValueList<S, T>(this Dictionary<S, T> dict)
        {
            List<T> newList = new List<T>();
            foreach (var item in dict)
            {
                newList.Add(item.Value);
            }
            return newList;
        }
        public static List<T> ValueList<T>(this Queue<T> dict)
        {
            List<T> newList = new List<T>();
            foreach (var item in dict)
            {
                newList.Add(item);
            }
            return newList;
        }

        public static HashSet<T> KeyHashset<T, S>(this Dictionary<T, S> dict)
        {
            HashSet<T> result = new HashSet<T>();
            foreach (var item in dict)
            {
                result.Add(item.Key);
            }
            return result;
        }

        public static Queue<T> ToQueue<T>(this List<T> list)
        {
            Queue<T> temp = new Queue<T>();
            foreach (var item in list)
            {
                temp.Enqueue(item);
            }
            return temp;
        }

        public static List<T> ReverseList<T>(this List<T> list)
        {
            List<T> clone = list.Clone();
            clone.Reverse();
            return clone;
        }

        public static IEnumerable<T> ReverseEnumerable<T>(this List<T> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                yield return list[i];
            }
        }
        
        /// <summary>
        /// Creates a deep copy of the list. It handles both value types and reference types
        /// by casting to ICloneable and creating new instances.
        /// </summary>
        public static List<T> DeepCopy<T>(this List<T> list)
        {
            List<T> newList = new List<T>();

            if (typeof(T).IsValueType)
            {
                return new List<T>(list);
            }
            
            // If type T implements the ICloneable interface.
            if (typeof(ICloneable).IsAssignableFrom(typeof(T)))
            {
                foreach (var item in list)
                {
                    if (item == null)
                    {
                        newList.Add(default(T));
                    }
                    else
                    {
                        newList.Add((T)((ICloneable)item).Clone());
                    }
                }
                return newList;
            }
            
            // Shallow copy for non-ICloneable reference types
            return new List<T>(list);
        }
    }
}