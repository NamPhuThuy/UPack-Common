using System;
using UnityEngine;

namespace NamPhuThuy.Common
{
    /// <summary>
    /// Retrieve information of time in the real world
    /// </summary>
    public class TimeHelper : MonoBehaviour
    {
        #region Converters
        
        /// <summary>
        /// Converts the specified <see cref="DateTime"/> to the total number of seconds
        /// elapsed since the custom epoch 2000-01-01T00:00:00Z.
        /// </summary>
        /// <param name="time">The time to convert. Pass UTC to avoid timezone offsets.</param>
        /// <returns>The number of seconds since 2000-01-01 UTC.</returns>
        /// <remarks>
        /// This uses a custom epoch (year 2000), not the standard Unix epoch (1970-01-01).
        /// </remarks>
        public static double ConvertToUnixTime(DateTime time)
        {
            DateTime epoch = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            return (time - epoch).TotalSeconds;
        }

        /// <summary>
        /// Converts a timestamp (seconds since 2000-01-01T00:00:00Z) to a UTC <see cref="DateTime"/>.
        /// </summary>
        /// <param name="timeStamp">Seconds elapsed since 2000-01-01 UTC (custom epoch).</param>
        /// <returns>The corresponding UTC <see cref="DateTime"/>.</returns>
        /// <remarks>
        /// Uses a custom epoch (year 2000), not the standard Unix epoch (1970-01-01).
        /// </remarks>
        public static DateTime ConvertFromUnixTime(double timeStamp)
        {
            DateTime epoch = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime time = epoch.AddSeconds(timeStamp);
            return time;
        }
        
        /// <summary>
        /// Formats total seconds as "MM:SS" with leading zeros (seconds normalized to 0–59).
        /// </summary>
        /// <param name="totalSeconds">Total elapsed seconds.</param>
        /// <returns>A string in MM:SS form.</returns>
        public static string ToMMSS(float totalSeconds)
        {
            totalSeconds = Mathf.Max(0f, totalSeconds);
            int minutes = Mathf.FloorToInt(totalSeconds / 60f);
            int seconds = Mathf.FloorToInt(totalSeconds % 60f);
            return $"{minutes:00}:{seconds:00}";
        }

        #endregion

        #region Formatting

        public static int GetHours(float totalSeconds)
        {
            return (int)(totalSeconds / 3600f);
        }

        public static int GetMinutes(float totalSeconds)
        {
            return (int)((totalSeconds / 60) % 60);
        }

        public static int GetSeconds(float totalSeconds)
        {
            return (int)(totalSeconds % 60);
        }

        #endregion
    }
}
