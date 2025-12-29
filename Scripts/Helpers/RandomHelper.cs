// Assets/_Project/_Code/Runtime/Common/RandomHelper.cs
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NamPhuThuy.Common
{
    public static class RandomHelper
    {
        /// <summary>
        /// Returns <paramref name="count"/> ints in range [minInclusive, maxExclusive).
        /// If <paramref name="unique"/> is true: values are unique (no duplicates).
        /// If false: values may repeat.
        /// </summary>
        public static int[] TakeRange(int minInclusive, int maxExclusive, int count, bool unique)
        {
            if (maxExclusive <= minInclusive)
                throw new ArgumentOutOfRangeException(nameof(maxExclusive), "Range must have at least 1 element.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be >= 0.");

            int rangeSize = maxExclusive - minInclusive;

            if (unique)
            {
                if (count > rangeSize)
                    throw new ArgumentOutOfRangeException(nameof(count), "Count cannot exceed range size when unique is true.");

                var pool = new int[rangeSize];
                for (int i = 0; i < rangeSize; i++)
                    pool[i] = minInclusive + i;

                // Partial Fisher–Yates: only shuffle first 'count' positions
                for (int i = 0; i < count; i++)
                {
                    int r = Random.Range(i, rangeSize); // [i, rangeSize)
                    (pool[i], pool[r]) = (pool[r], pool[i]);
                }

                var resultUnique = new int[count];
                Array.Copy(pool, 0, resultUnique, 0, count);
                return resultUnique;
            }
            else
            {
                var result = new int[count];
                for (int i = 0; i < count; i++)
                    result[i] = Random.Range(minInclusive, maxExclusive);

                return result;
            }
        }

        // Backward-compatible wrapper (keeps your old API working)
        public static int[] TakeUniqueRange(int minInclusive, int maxExclusive, int count)
            => TakeRange(minInclusive, maxExclusive, count, unique: true);
        
        
    }
}