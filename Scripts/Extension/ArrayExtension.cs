// `Assets/_Project/_Code/Runtime/Common/ArrayExtensions.cs`
using System;
using UnityEngine;

namespace NamPhuThuy.Common
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// In-place Fisher–Yates shuffle using UnityEngine.Random.
        /// </summary>
        public static void ShuffleInPlace<T>(this T[] array)
        {
            if (array == null || array.Length <= 1) return;

            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        /// <summary>
        /// In-place Fisher–Yates shuffle using a provided System.Random (deterministic if seeded).
        /// </summary>
        public static void ShuffleInPlace<T>(this T[] array, System.Random rng)
        {
            if (array == null || array.Length <= 1) return;
            if (rng == null) throw new ArgumentNullException(nameof(rng));

            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = rng.Next(0, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }
    }
}

// Usage:
// validId.ShuffleInPlace();
// or:
// validId.ShuffleInPlace(new System.Random(seed));