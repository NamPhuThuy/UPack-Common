using System;
using System.Collections.Generic;

namespace NamPhuThuy.Common
{
    
    public static class DictionaryExtension
    {
        public static Dictionary<T1, T2> ShuffleItemsWithSameValue<T1, T2>(this Dictionary<T1, T2> dict)
        {
            // Group keys by their value
            var valueGroups = new Dictionary<T2, List<T1>>();
            foreach (var kvp in dict)
            {
                if (!valueGroups.TryGetValue(kvp.Value, out var list))
                {
                    list = new List<T1>();
                    valueGroups[kvp.Value] = list;
                }
                list.Add(kvp.Key);
            }

            valueGroups.SortByKeyDescending();
            DebugLogger.LogDictionary(valueGroups, title:" Value Groups Before Shuffle");
            foreach (var keyValuePair in valueGroups)
            {
                var sb = new System.Text.StringBuilder();
                sb.Append($"Value: {keyValuePair.Key} -> Keys: ");
                foreach (var key in keyValuePair.Value)
                {
                    sb.Append($"{key}, ");
                }
                DebugLogger.LogWithoutHeader(message:$"{sb}");
            }

            // Shuffle keys *within each value group*
            var rng = new Random();
            foreach (var group in valueGroups.Values)
            {
                ShuffleList(group, rng);
            }

            DebugLogger.LogDictionary(valueGroups, title:" Value Groups After Shuffle");
            
            // Rebuild dict by iterating original order of values,
            // but taking a shuffled key from the corresponding group
            var shuffledDict = new Dictionary<T1, T2>();
            
            foreach (var keyValuePair in valueGroups)
            {
                foreach (T1 t1Value in keyValuePair.Value)
                {
                    shuffledDict.Add(t1Value, keyValuePair.Key);
                }
            }
            
            DebugLogger.LogDictionary(shuffledDict, title:" Shuffled Dictionary");

            return shuffledDict;
        }

        private static void ShuffleList<T>(List<T> list, Random rng)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }

   
}