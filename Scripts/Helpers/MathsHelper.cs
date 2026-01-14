using System;
using System.Collections.Generic;
using UnityEngine;

namespace NamPhuThuy.Common
{
    public static class MathsHelper
    {
        //Map a value from scale-A to scale-B

        #region MapValue

        public static float Map(float value, float originMin, float originMax, float newMin, float newMax, bool clamp)
        {
            float newValue = (value - originMin) / (originMax - originMin) * (newMax - newMin) + newMin;

            if (clamp)
            {
                newValue = Mathf.Clamp(newValue, newMin, newMax);
            }

            return newValue;
        }

        public static float ReverseMap(float value, float originMin, float originMax, float newMin, float newMax,
            bool clamp)
        {
            float newValue = (1f - ((value - originMin) / (originMax - originMin))) * (newMax - newMin) + newMin;

            if (clamp)
            {
                newValue = Mathf.Clamp(newValue, newMin, newMax);
            }

            return newValue;
        }

        #endregion

        #region Iterpolation

        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static float LinearInterpolate(float from, float to, float percentage)
        {
            return (to - from) * percentage + from;
        }

        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static Vector2 LinearInterpolate(Vector2 from, Vector2 to, float percentage)
        {
            return new Vector2(LinearInterpolate(from.x, to.x, percentage),
                LinearInterpolate(from.y, to.y, percentage));
        }

        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static Vector3 LinearInterpolate(Vector3 from, Vector3 to, float percentage)
        {
            return new Vector3(LinearInterpolate(from.x, to.x, percentage), LinearInterpolate(from.y, to.y, percentage),
                LinearInterpolate(from.z, to.z, percentage));
        }

        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static Vector4 LinearInterpolate(Vector4 from, Vector4 to, float percentage)
        {
            return new Vector4(LinearInterpolate(from.x, to.x, percentage), LinearInterpolate(from.y, to.y, percentage),
                LinearInterpolate(from.z, to.z, percentage), LinearInterpolate(from.w, to.w, percentage));
        }

        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static Rect LinearInterpolate(Rect from, Rect to, float percentage)
        {
            return new Rect(LinearInterpolate(from.x, to.x, percentage), LinearInterpolate(from.y, to.y, percentage),
                LinearInterpolate(from.width, to.width, percentage),
                LinearInterpolate(from.height, to.height, percentage));
        }

        /// <summary>
        /// Linear interpolation.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="percentage">Percentage.</param>
        public static Color LinearInterpolate(Color from, Color to, float percentage)
        {
            return new Color(LinearInterpolate(from.r, to.r, percentage), LinearInterpolate(from.g, to.g, percentage),
                LinearInterpolate(from.b, to.b, percentage), LinearInterpolate(from.a, to.a, percentage));
        }

        #endregion

        //animation curves:
        /// <summary>
        /// Evaluates the curve.
        /// </summary>
        /// <returns>The value evaluated at the percentage of the clip.</returns>
        /// <param name="curve">Curve.</param>
        /// <param name="percentage">Percentage.</param>
        public static float EvaluateCurve(AnimationCurve curve, float percentage)
        {
            return curve.Evaluate((curve[curve.length - 1].time) * percentage);
        }

        #region Random

        //To get a truly consistent (repeatable) random sequence, you need to provide a specific seed:
        //private static System.Random mRandom = new System.Random(12345); // example fixed seed
        private static System.Random mRandom = new System.Random();

        public static int EasyRandom(int range)
        {
            return mRandom.Next(range);
        }

        public static int EasyRandom(int min, int max)
        {
            return mRandom.Next(min, max);
        }

        public static float EasyRandom(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }


        /// <summary>
        /// Picks 'count' distinct random elements from source (no duplicates).
        /// Throws if count &gt; source.Count unless allowLess = true (then it clamps).
        /// </summary>
        public static List<T> PickRandomDistinct<T>(IList<T> source, int count, bool allowLess = false,
            System.Random rng = null)
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            int n = source.Count;
            if (count < 0) throw new System.ArgumentOutOfRangeException(nameof(count));
            if (count > n)
            {
                if (allowLess) count = n;
                else throw new System.ArgumentException("count greater than source size.");
            }

            rng ??= mRandom;

            // Indices array
            int[] indices = new int[n];
            for (int i = 0; i < n; i++) indices[i] = i;

            // Partial Fisher-Yates
            for (int i = 0; i < count; i++)
            {
                int r = rng.Next(i, n); // inclusive i, exclusive n
                (indices[i], indices[r]) = (indices[r], indices[i]);
            }

            var result = new List<T>(count);
            for (int i = 0; i < count; i++)
                result.Add(source[indices[i]]);
            return result;
        }

        /// <summary>
        /// Picks 'count' random elements allowing duplicates (with replacement).
        /// Returns empty list if source is null or empty.
        /// </summary>
        public static List<T> PickRandomWithReplacement<T>(IList<T> source, int count, System.Random rng = null)
        {
            var result = new List<T>(count);
            if (source == null || source.Count == 0 || count <= 0) return result;

            rng ??= mRandom;
            int n = source.Count;
            for (int i = 0; i < count; i++)
            {
                int idx = rng.Next(0, n);
                result.Add(source[idx]);
            }

            return result;
        }


        /// <summary>
        /// Picks 'count' random ints in [minInclusive, maxInclusive] (with replacement, duplicates possible).
        /// Returns empty list if count <= 0 or range invalid.
        /// </summary>
        public static List<int> PickRandomRange(int minInclusive, int maxInclusive, int count, System.Random rng = null)
        {
            var result = new List<int>(count);
            if (count <= 0) return result;
            if (maxInclusive < minInclusive) return result;

            rng ??= mRandom;
            int span = maxInclusive - minInclusive + 1;
            for (int i = 0; i < count; i++)
            {
                int v = rng.Next(span) + minInclusive; // Next(span) gives [0, span)
                result.Add(v);
            }

            return result;
        }

        /// <summary>
        /// Picks 'count' distinct random ints in [minInclusive, maxInclusive] (no duplicates).
        /// Throws if count > rangeSize unless allowLess = true (then clamps to rangeSize).
        /// </summary>
        public static List<int> PickRandomRangeDistinct(int minInclusive, int maxInclusive, int count,
            bool allowLess = false, System.Random rng = null)
        {
            if (maxInclusive < minInclusive) throw new System.ArgumentException("Invalid range.");
            if (count < 0) throw new System.ArgumentOutOfRangeException(nameof(count));

            int rangeSize = maxInclusive - minInclusive + 1;
            if (count > rangeSize)
            {
                if (allowLess) count = rangeSize;
                else throw new System.ArgumentException("count greater than range size.");
            }

            rng ??= mRandom;

            // If we need most of the range, build array and partial shuffle (Fisher-Yates).
            if (count > rangeSize / 2)
            {
                int[] arr = new int[rangeSize];
                for (int i = 0; i < rangeSize; i++) arr[i] = minInclusive + i;
                for (int i = 0; i < count; i++)
                {
                    int r = rng.Next(i, rangeSize);
                    (arr[i], arr[r]) = (arr[r], arr[i]);
                }

                var result = new List<int>(count);
                for (int i = 0; i < count; i++) result.Add(arr[i]);
                return result;
            }
            else
            {
                // Need few values: use HashSet to avoid duplicates.
                var set = new HashSet<int>();
                while (set.Count < count)
                {
                    int v = rng.Next(rangeSize) + minInclusive;
                    set.Add(v);
                }

                return new List<int>(set);
            }
        }
        
        

        #endregion

        #region XO Random

        private struct RandState
        {
            public ulong Seed;
            public bool Initialized;
        }

        private static RandState _randState = new RandState();

        public static ulong RandXOR()
        {
            if (!_randState.Initialized)
            {
                _randState.Seed = (ulong)DateTime.Now.Ticks;
                _randState.Initialized = true;
            }

            ulong x = _randState.Seed;
            x ^= x << 9;
            x ^= x >> 5;
            x ^= x << 15;
            return _randState.Seed = x;
        }


        /// <summary>
        /// Returns a pseudo-random integer in the inclusive range [min, max]
        /// using the internal XOR-based random generator.
        /// </summary>
        /// <param name="min">Minimum value of the range (inclusive).</param>
        /// <param name="max">Maximum value of the range (inclusive).</param>
        /// <returns>A pseudo-random integer between min and max (inclusive).</returns>
        public static int RandRange(int min, int max)
        {
            return min + (int)(RandXOR() % (ulong)(max - min + 1));
        }

        #endregion
        
        #region Operations Helpers
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="divBy"></param>
        /// <returns></returns>
        public static int RoundDownToDiv(int value, int divBy)
        {
            if (divBy == 0) return value;
            return value - (value % divBy);
        }

        #endregion

        #region Convert

        /// <summary>
        /// Turn number to text
        /// <para>12.345f -> 12</para>
        /// <para>1234 -> 1234</para>
        /// <para>1234567 -> 1.23M</para>
        /// <para>1234567890 -> 1.23B</para>
        /// </summary>
        /// <param name="_number"></param>
        /// <returns></returns>
        public static string NumberCustomToString(float _number)
        {
            string str = "";
            if (_number < 10000)
                str = _number.ToString("00");
            else if (10000 <= _number && _number < 1000000)
                str = (_number / 1000).ToString("0.#") + "K";
            else if (1000000 <= _number && _number < 1000000000)
                str = (_number / 1000000).ToString("0.##") + "M";
            else
                str = (_number / 1000000000).ToString("0.##") + "B";
            return str;
        }

        #endregion
    }
}