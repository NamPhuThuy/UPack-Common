using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NamPhuThuy.Common
{
    
    public static partial class DebugLogger
    {
        #region Private Serializable Fields

        #endregion

        #region Private Fields

        private static void DisableErrorLog()
        {
            Debug.unityLogger.filterLogType = LogType.Warning; // show only Warning/Error/Exception
        }
        #endregion


        #region Private Methods
        #endregion

        #region New Methods
        
        /*
         #region ✧ Log Error

        public static void LogError(
            string message,
            LogTag tag = LogTag.Error,
            Color color = default,
            bool setBold = false,
            Object context = null)
        {
            LogInternal(LogLevel.Error, tag, message, color, setBold, context);
        }

        /// <summary>
        /// Log error only if condition is true
        /// </summary>
        public static void LogErrorIf(
            bool condition,
            string content,
            LogTag tag = LogTag.Error,
            Color color = default,
            bool setBold = false,
            Object context = null)
        {
            if (!condition) return;
            LogInternal(LogLevel.Error, tag, content, color, setBold, context);
        }

        #endregion

        #region ✧ Log Warning

        public static void LogWarning(
            string message,
            LogTag tag = LogTag.Warning,
            Color color = default,
            bool setBold = false,
            Object context = null)
        {
            LogInternal(LogLevel.Warning, tag, message, color, setBold, context);
        }

        /// <summary>
        /// Log warning only if condition is true
        /// </summary>
        public static void LogWarningIf(
            bool condition,
            string content,
            LogTag tag = LogTag.Warning,
            Color color = default,
            bool setBold = false,
            Object context = null)
        {
            if (!condition) return;
            LogInternal(LogLevel.Warning, tag, content, color, setBold, context);
        }

        #endregion

        #region ✧ Log

        /// <summary>
        /// Log only if condition is true
        /// </summary>
        public static void LogIf(
            bool condition,
            string content,
            LogTag tag = LogTag.Inform,
            Color color = default,
            bool setBold = false,
            Object context = null)
        {
            if (!condition) return;
            Log(tag, message: content, color: color, context: context, setBold: setBold);
        }

        public static void LogWithFrame(
            string content,
            LogTag tag = LogTag.Inform,
            Color color = default,
            bool setBold = false,
            Object context = null)
        {
            string frameInfo = $"[Frame {Time.frameCount}]";
            LogInternal(LogLevel.Log, tag, $"{frameInfo} - {content}", color, setBold, context);
        }

        public static void LogFrog(
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            string message = "",
            LogTag tag = LogTag.Inform,
            Color color = default,
            Object context = null,
            bool setBold = false)
        {
            string header = BuildHeader(filePath, memberName);
            LogInternal(LogLevel.Log, tag, $"🐸{header}: {message}", color, setBold, context);
        }

        public static void Log(
            LogTag tag = LogTag.Inform,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            string message = "",
            Color color = default,
            Object context = null,
            bool setBold = false)
        {
            string header = BuildHeader(filePath, memberName);
            LogInternal(LogLevel.Log, tag, $"{header}: {message}", color, setBold, context);
        }

        public static void LogWithoutHeader(
            string message = "",
            LogTag tag = LogTag.Inform,
            Color color = default,
            Object context = null,
            bool setBold = false)
        {
            LogInternal(LogLevel.Log, tag, message, color, setBold, context);
        }

        #endregion

        #region ✧ Break

        /// <summary>
        /// Breaks execution in the editor and logs a message
        /// </summary>
        public static void LogBreak(
            string content,
            LogTag tag = LogTag.Break,
            Color color = default,
            bool setBold = false,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "")
        {
            string className = Path.GetFileNameWithoutExtension(filePath);
            string location = $"{className}.{memberName}()::{line}";
            string message = $"[BREAK] {location} - {content}";

            if (color == default) color = Color.red;

            LogInternal(LogLevel.Error, tag, message, color, setBold, context: null);

#if UNITY_EDITOR
            Debug.Break();
#endif
        }

        /// <summary>
        /// Conditional break \- only breaks if condition is true
        /// </summary>
        public static void LogBreakIf(
            bool condition,
            string content,
            LogTag tag = LogTag.Break,
            Color color = default,
            bool setBold = false,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "")
        {
            if (!condition) return;
            LogBreak(content, tag, color, setBold, line, memberName, filePath);
        }

        /// <summary>
        /// Assert with break \- breaks if condition is false
        /// </summary>
        public static void LogAssert(
            bool condition,
            string content,
            LogTag tag = LogTag.Break,
            Color color = default,
            bool setBold = false,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "")
        {
            if (condition) return;
            LogBreak($"ASSERTION FAILED: {content}", tag, color, setBold, line, memberName, filePath);
        }

        #endregion

        #region ✧ Try-catch

        /// <summary>
        /// Try-catch wrapper with logging
        /// </summary>
        public static void LogTry(
            System.Action action,
            string context = "Unknown operation",
            Object contextObject = null,
            LogTag tag = LogTag.Exception)
        {
            try
            {
                action?.Invoke();
            }
            catch (System.Exception ex)
            {
                LogException(ex, context, contextObject, tag);
            }
        }

        /// <summary>
        /// Log exception with full details
        /// </summary>
        public static void LogException(
            System.Exception ex,
            string context = "",
            Object contextObject = null,
            LogTag tag = LogTag.Exception)
        {
            if (!enableLog) return;
            if (!IsTagEnabled(tag)) return;

            string message = string.IsNullOrEmpty(context)
                ? $"EXCEPTION: {ex.Message}\nStackTrace: {ex.StackTrace}"
                : $"EXCEPTION in {context}: {ex.Message}\nStackTrace: {ex.StackTrace}";

            LogInternal(LogLevel.Error, tag, message, Color.red, setBold: true, context: contextObject);
        }

        #endregion

        #region ✧ Data Structure Logging

        public static void LogDictionary(
            IDictionary dict,
            string title = "Dictionary",
            LogTag tag = LogTag.Inform,
            Color color = default,
            bool setBold = false,
            Object context = null)
        {
            if (dict == null)
            {
                LogWithoutHeader($"{title}: NULL dictionary", LogTag.NullInvalid, color, context, setBold);
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"{title} (count={dict.Count}):");

            foreach (DictionaryEntry entry in dict)
                sb.AppendLine($"  {entry.Key} -> {entry.Value}");

            LogWithoutHeader(sb.ToString(), tag, color, context, setBold);
        }

        public static void LogList(
            IList list,
            string title = "List",
            LogTag tag = LogTag.Inform,
            Color color = default,
            bool setBold = false,
            Object context = null)
        {
            if (list == null)
            {
                LogWithoutHeader($"{title}: NULL list", LogTag.NullInvalid, color, context, setBold);
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"{title} (count={list.Count}):");

            for (int i = 0; i < list.Count; i++)
                sb.AppendLine($"  [{i}] -> {list[i]}");

            LogWithoutHeader(sb.ToString(), tag, color, context, setBold);
        }

        #endregion
         */
        
        #endregion
        
        

    }
}