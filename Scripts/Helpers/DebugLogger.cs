using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

/*
 Why should keep the suffix of class is "Logger" and prefix of method is "Log"
 - Reddit: https://www.reddit.com/r/Unity3D/comments/17eikh0/i_found_a_way_to_go_to_the_right_line_in_your/
 - Sample gist: https://gist.github.com/AnatoleCF/9f4b28750ebd8c30bebd3cd04f04e520
 - Any class whose name ends with "Logger" that implements a method starting with "Log" is ignored by the console's double click, unless it is the last call in the stack trace.
 - 
 */

// Icon pool: ❌ 🔍 ⚠️📦✅🎯🐝📂🐸 🎮 🔧 ⚠️ ❌ ✅ 💡 🚀 ⭐ 🎯 🔥 💰 🏆 👾 🎲 📊 🔔 ⚡ 🛡️ ⚔️ 🎪 🎨 🔊 📱 🐛 ⏰ 💀 ❤️ 🎵 📦 🗝️ 🌟 🏃 💥 🎁 📍 🔒 🔓 ⏸️ ▶️ ⏹️

/* Icon pool
 Solid shapes: ■ ● ⬛ ⬤ ◆ ⬟ ⬢ ⬣ ◼ ◾
Triangles/arrows: ▶ ► ▸ ▴ ▾ ⯈ ⮞ ➤
Blocks/bullets: ▪ • ‣ ∙ ◉ ◍ ◘
Stars/sparks: ★ ✦ ✧ ✪ ✯ ✵ ✶
Cross/check (bold look): ✖ ✕ ✗ ✔ ✓ ☑
Math/other: ∎ ■ ⬛ ⦿ ⧈ ⬚
*/
namespace NamPhuThuy.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class DebugLogger
    {
        public static bool enableLog = true;
        public static readonly Color defaultColor = Color.cyan;
        public static readonly Color eventColor = Color.yellow;
        
        [System.Flags]
        public enum LogTag
        {
            None        = 0,
            Inform      = 1 << 0,
            NullInvalid = 1 << 1,
            Event       = 1 << 2,
            Warning     = 1 << 3,
            Error       = 1 << 4,
            Break       = 1 << 5,
            Exception   = 1 << 6,
            All         = ~0
        }
        public static LogTag enabledTags = LogTag.All;
        private enum LogLevel { Log, Warning, Error }

        #region New

        public static bool IsTagEnabled(LogTag tag)
        {
            if (tag == LogTag.None) return true;
            return (enabledTags & tag) != 0;
        }

        public static void SetTagEnabled(LogTag tag, bool enabled)
        {
            if (enabled) enabledTags |= tag;
            else enabledTags &= ~tag;
        }
        
        private static void LogInternal(
            LogLevel level,
            LogTag tag,
            string message,
            Color color = default,
            bool setBold = false,
            Object context = null)
        {
            if (!enableLog) return;
            if (!IsTagEnabled(tag)) return;

            Color currentColor = color == default ? defaultColor : color;

            string tagged = tag == LogTag.None ? message : $"[{tag}] {message}";
            string formatted = ColorizedText(tagged, currentColor, setBold);

            switch (level)
            {
                case LogLevel.Warning:
                    Debug.LogWarning(formatted, context);
                    break;
                case LogLevel.Error:
                    Debug.LogError(formatted, context);
                    break;
                default:
                    Debug.Log(formatted, context);
                    break;
            }
        }

        private static string BuildHeader(string filePath, string memberName)
        {
            string className = Path.GetFileNameWithoutExtension(filePath);

            return $"{className}().{memberName}";
        }

        #endregion
        
        #region ✧ Log Error

        public static void LogError(string message, Color color = default, bool setBold = false, Object context = null)
        {
            if (!enableLog)
                return;
            Debug.LogError(ColorizedText(message, color, setBold), context);
        }
        
        /// <summary>
        /// Log error only if condition is true
        /// </summary>
        public static void LogErrorIf(bool condition, string content, Color color = default, bool setBold = false)
        {
            if (!enableLog)
                return;
        
            if (condition)
            {
                LogError(content, color, setBold);
            }
        }

        #endregion

        #region ✧ Log Warning
        
        public static void LogWarning(string message, Color color = default, bool setBold = false, Object context = null)
        {
            if (!enableLog)
                return;
            Debug.LogWarning(ColorizedText(message, color, setBold), context);
            return;
        }

        /// <summary>
        /// Log warning only if condition is true
        /// </summary>
        public static void LogWarningIf(bool condition, string content, Color color = default, bool setBold = false)
        {
            if (!enableLog)
                return;
        
            if (condition)
            {
                LogWarning(content, color, setBold);
            }
        }
        #endregion
        
        #region ✧ Log 

        /// <summary>
        /// Log only if condition is true
        /// </summary>
        public static void LogIf(bool condition, string content, Color color = default, bool setBold = false)
        {
            if (!enableLog)
                return;
        
            if (condition)
            {
                Log(message:content, color:color, setBold:setBold);
            }
        }
        
        public static void LogWithFrame(string content, Color color = default, bool setBold = false)
        {
            string frameInfo = $"[Frame {Time.frameCount}] ";
            Debug.Log(ColorizedText($"{frameInfo} - {content}", color, setBold));
        }

        public static void LogFrog([CallerLineNumber] int line = 0
            , [CallerMemberName] string memberName = ""
            , [CallerFilePath] string filePath = "", string message = "", Color color = default, Object context = null, bool setBold = false)
        {
            if (!enableLog)
                return;
            
            string className = Path.GetFileNameWithoutExtension(filePath);

            Color currentColor = color == default ? Color.cyan : color;
            
            string resMessage = $"🐸{className}().{memberName}: {message}";
            
            Debug.Log(ColorizedText(resMessage, currentColor, setBold), context: context);
        }
        
            
        public static void Log(
            [CallerLineNumber] int line = 0
            , [CallerMemberName] string memberName = ""
            , [CallerFilePath] string filePath = "", string message = "", Color color = default, Object context = null, bool setBold = false
            /*, [CallerArgumentExpression("message")] string expression = ""*/
        )
        {
            if (!enableLog)
                return;
            
            string className = Path.GetFileNameWithoutExtension(filePath);

            Color currentColor = color == default ? Color.cyan : color;

            
            string resMessage = $"{className}().{memberName}: {message}";
            
            Debug.Log(ColorizedText(resMessage, currentColor, setBold), context: context);
            
            /*Can replace UnityEngine.Debug.Log with any logging API you want

            Usage is simple: just put a LogCaller(); at any line you want. The compiler will pass in the 3 parameters for you.*/
        }

        public static void LogWithoutHeader(string message = "", Color color = default, Object context = null,
            bool setBold = false)
        {
            if (!enableLog)
                return;
            
            Color currentColor = color == default ? Color.cyan : color;
            Debug.Log(ColorizedText(message, currentColor, setBold), context: context);
        }
        
        

        #endregion

        #region ✧ Break

        
        /// <summary>
        /// Breaks execution in the editor and logs a message
        /// </summary>
        public static void LogBreak(string message = "", Color color = default, bool setBold = false,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "")
        {
            if (!enableLog)
                return;

            string className = Path.GetFileNameWithoutExtension(filePath);
            string location = $"{className}.{memberName}()::{line}";
            string finalMessage = $"[BREAK] {location} - {message}";

            if (color == default)
                color = Color.red;

            Debug.LogError(ColorizedText(finalMessage, color, setBold));

#if UNITY_EDITOR
            Debug.Break();
#endif
        }

        /// <summary>
        /// Conditional break - only breaks if condition is true
        /// </summary>
        public static void LogBreakIf(bool condition, string message = "", Color color = default, bool setBold = false,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "")
        {
            if (condition)
            {
                LogBreak(message, color, setBold, line, memberName, filePath);
            }
        }

        /// <summary>
        /// Assert with break - breaks if condition is false
        /// </summary>
        public static void LogAssert(bool condition, string message = "", Color color = default, bool setBold = false,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "")
        {
            if (!condition)
            {
                LogBreak($"ASSERTION FAILED: {message}", color, setBold, line, memberName, filePath);
            }
        }

        #endregion

        #region ✧ Try-catch 

        /// <summary>
        /// Try-catch wrapper with logging
        /// </summary>
        public static void LogTry(System.Action action, string context = "Unknown operation", Object contextObject = null)
        {
            try
            {
                action?.Invoke();
            }
            catch (System.Exception ex)
            {
                LogException(ex, context, contextObject);
            }
        }
        
        /// <summary>
        /// Log exception with full details
        /// </summary>
        public static void LogException(System.Exception ex, string context = "", Object contextObject = null)
        {
            if (!enableLog)
                return;

            string message = string.IsNullOrEmpty(context) 
                ? $"EXCEPTION: {ex.Message}\nStackTrace: {ex.StackTrace}" 
                : $"EXCEPTION in {context}: {ex.Message}\nStackTrace: {ex.StackTrace}";
    
            Debug.LogError(ColorizedText(message, Color.red, true), contextObject);
        }

        #endregion

        #region ✧ Data Structure Logging

        
        public static void LogDictionary(IDictionary dict, string title = "Dictionary", 
            Color color = default, bool setBold = false, Object context = null)
        {
            if (!enableLog)
                return;

            if (dict == null)
            {
                LogWithoutHeader($"{title}: NULL dictionary", color, context, setBold);
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"{title} (count={dict.Count}):");

            foreach (DictionaryEntry entry in dict)
            {
                sb.AppendLine($"  {entry.Key} -> {entry.Value}");
            }

            LogWithoutHeader(sb.ToString(), color, context, setBold);
        }
        
        public static void LogList(IList list, string title = "List", Color color = default, bool setBold = false, Object context = null)
        {
            if (!enableLog)
                return;

            if (list == null)
            {
                LogWithoutHeader($"{title}: NULL list", color, context, setBold);
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"{title} (count={list.Count}):");

            for (int i = 0; i < list.Count; i++)
            {
                sb.AppendLine($"  [{i}] -> {list[i]}");
            }

            LogWithoutHeader(sb.ToString(), color, context, setBold);
        }

        #endregion

        #region ✧ Helper

        /// <summary>
        /// Get full hierarchy path of GameObject
        /// </summary>
        private static string GetGameObjectPath(GameObject go)
        {
            if (go == null)
                return "NULL";

            string path = go.name;
            Transform parent = go.transform.parent;
    
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
    
            return path;
        }
        
        /// <summary>
        /// Format array/collection for logging
        /// </summary>
        public static string FormatCollection<T>(System.Collections.Generic.IEnumerable<T> collection, string separator = ", ")
        {
            if (collection == null)
                return "NULL";
    
            return string.Join(separator, collection);
        }

        /// <summary>
        /// Format Vector3 for logging
        /// </summary>
        public static string FormatVector3(Vector3 vector, int decimals = 2)
        {
            return $"({vector.x.ToString($"F{decimals}")}, {vector.y.ToString($"F{decimals}")}, {vector.z.ToString($"F{decimals}")})";
        }
        
        public static string ColorizedText(string content, Color color, bool setBold = false)
        {
            if (color == default)
                color = Color.cyan;
            
            if (setBold)
                return $"<b><color=#{ColorUtility.ToHtmlStringRGB(color)}>{content}</color></b>";
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{content}</color>";
        }
        
        public static string ColorizedText(string content, bool setBold = false)
        {
            if (setBold)
                return $"<b>{content}</b>";
            return $"{content}";
        }

        #endregion
    }
}