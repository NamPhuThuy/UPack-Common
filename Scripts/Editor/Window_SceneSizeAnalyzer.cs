// File: `Assets/_Project/Code/UPack-Common/Scripts/Editor/Window_SceneSizeAnalyzer.cs`

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

/// <summary>
/// This script will be checked later
/// </summary>
public class Window_SceneSizeAnalyzer : EditorWindow
{
    #region Private Fields
    private Vector2 _scrollPos;
    private Vector2 _scrollPosResults;

    private GUIStyle _centeredButtonStyle;
    private GUIStyle _centeredLabelStyle;

    [SerializeField] private bool _includeInactive = true;
    [SerializeField] private bool _groupByType = true;
    [SerializeField] private bool _uniqueAssetsOnly = true;
    [SerializeField] private bool _showRuntimeMemory = true;

    private readonly List<Row> _rows = new();
    private long _totalBytes;
    private long _totalRuntimeBytes;

    private struct Row
    {
        public string assetPath;
        public string typeName;
        public long bytes;
        public long runtimeBytes;
    }
    #endregion

    #region Menu Item
    [MenuItem("NamPhuThuy/Common/Window - Analyze Selected GameObjects (Referenced Assets)")]
    private static void Open()
    {
        Window_SceneSizeAnalyzer window = GetWindow<Window_SceneSizeAnalyzer>("Scene Asset Size Analyzer");
        window.minSize = new Vector2(520, 640);
        window.Show();
    }
    #endregion

    #region Unity Callbacks
    private void OnGUI()
    {
        InitializeStyles();

        float padding = 20f;
        Rect areaRect = new Rect(padding, padding, position.width - 2f * padding, position.height - 2f * padding);

        GUILayout.BeginArea(areaRect);

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        DrawHeader();
        GUILayout.Space(10);
        DrawContent();
        GUILayout.Space(10);
        DrawButtons();

        EditorGUILayout.EndScrollView();

        GUILayout.EndArea();
    }
    #endregion

    #region Private Methods
    private void InitializeStyles()
    {
        if (_centeredButtonStyle == null)
        {
            _centeredButtonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
        }

        if (_centeredLabelStyle == null)
        {
            _centeredLabelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16
            };
        }
    }

    private void DrawHeader()
    {
        GUILayout.Label("Scene Asset Size Analyzer", _centeredLabelStyle);
        EditorGUILayout.HelpBox(
            "Estimates referenced asset sizes (Editor file sizes).\n" +
            "Runtime Memory uses Profiler\\.GetRuntimeMemorySizeLong and is an estimate.\n" +
            "This does NOT equal final APK size.",
            MessageType.Info);
    }

    private void DrawContent()
    {
        GUILayout.Label("Options", _centeredLabelStyle);

        _includeInactive = EditorGUILayout.Toggle("Include inactive children", _includeInactive);
        _groupByType = EditorGUILayout.Toggle("Group by asset type", _groupByType);
        _uniqueAssetsOnly = EditorGUILayout.Toggle("Count unique assets only", _uniqueAssetsOnly);
        _showRuntimeMemory = EditorGUILayout.Toggle("Show runtime memory (estimate)", _showRuntimeMemory);

        GUILayout.Space(12);

        GUILayout.Label("Results", _centeredLabelStyle);

        if (_showRuntimeMemory)
        {
            EditorGUILayout.LabelField(
                $"Total (disk): {FormatBytes(_totalBytes)}    Total (runtime): {FormatBytes(_totalRuntimeBytes)}",
                EditorStyles.boldLabel);
        }
        else
        {
            EditorGUILayout.LabelField($"Total (sum): {FormatBytes(_totalBytes)}", EditorStyles.boldLabel);
        }

        _scrollPosResults = EditorGUILayout.BeginScrollView(_scrollPosResults, GUILayout.MinHeight(320));

        if (_rows.Count == 0)
        {
            EditorGUILayout.HelpBox("No results yet. Select one or more GameObjects in the Hierarchy and click Analyze.", MessageType.None);
        }
        else if (_groupByType)
        {
            IEnumerable<IGrouping<string, Row>> groups =
                _rows.GroupBy(r => r.typeName)
                    .OrderByDescending(g => _showRuntimeMemory ? g.Sum(x => x.runtimeBytes) : g.Sum(x => x.bytes));

            foreach (var grp in groups)
            {
                long grpDisk = grp.Sum(x => x.bytes);
                long grpRuntime = grp.Sum(x => x.runtimeBytes);

                EditorGUILayout.LabelField(
                    _showRuntimeMemory
                        ? $"{grp.Key}: disk {FormatBytes(grpDisk)} \\| runtime {FormatBytes(grpRuntime)}"
                        : $"{grp.Key}: {FormatBytes(grpDisk)}",
                    EditorStyles.boldLabel);

                foreach (var r in grp.OrderByDescending(x => _showRuntimeMemory ? x.runtimeBytes : x.bytes))
                    DrawRow(r);

                GUILayout.Space(6);
            }
        }
        else
        {
            foreach (var r in _rows.OrderByDescending(x => _showRuntimeMemory ? x.runtimeBytes : x.bytes))
                DrawRow(r);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawButtons()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Analyze Selection", _centeredButtonStyle, GUILayout.Height(30)))
            AnalyzeSelection();

        if (GUILayout.Button("Clear", _centeredButtonStyle, GUILayout.Height(30)))
            Clear();

        GUILayout.EndHorizontal();
    }

    private void DrawRow(Row r)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Ping", GUILayout.Width(50)))
            {
                var obj = AssetDatabase.LoadMainAssetAtPath(r.assetPath);
                if (obj != null) EditorGUIUtility.PingObject(obj);
            }

            EditorGUILayout.LabelField(r.typeName, GUILayout.Width(140));
            EditorGUILayout.LabelField(FormatBytes(r.bytes), GUILayout.Width(110));

            if (_showRuntimeMemory)
                EditorGUILayout.LabelField(FormatBytes(r.runtimeBytes), GUILayout.Width(110));

            EditorGUILayout.SelectableLabel(r.assetPath, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        }
    }

    private void AnalyzeSelection()
    {
        Clear();

        var selected = Selection.gameObjects;
        if (selected == null || selected.Length == 0) return;

        var roots = selected.ToList();
        var sceneObjects = new List<GameObject>();

        foreach (var root in roots)
        {
            if (root == null) continue;
            sceneObjects.Add(root);

            foreach (Transform t in root.GetComponentsInChildren<Transform>(_includeInactive))
            {
                if (t == null) continue;
                if (t.gameObject == root) continue;
                sceneObjects.Add(t.gameObject);
            }
        }

        var components = sceneObjects
            .Where(go => go != null)
            .SelectMany(go => go.GetComponents<Component>())
            .Where(c => c != null)
            .Cast<UnityEngine.Object>()
            .ToArray();

        var deps = EditorUtility.CollectDependencies(components);

        var assetPaths = new List<string>();
        foreach (var dep in deps)
        {
            if (dep == null) continue;

            var path = AssetDatabase.GetAssetPath(dep);
            if (string.IsNullOrEmpty(path)) continue;
            if (path.StartsWith("Resources/unity_builtin_extra")) continue;
            if (path == "Library/unity default resources") continue;

            assetPaths.Add(path);
        }

        if (_uniqueAssetsOnly)
            assetPaths = assetPaths.Distinct().ToList();

        foreach (var path in assetPaths)
        {
            long bytes = 0;
            try
            {
                var fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                    bytes = new FileInfo(fullPath).Length;
            }
            catch
            {
                // ignore unreadable files
            }

            var mainObj = AssetDatabase.LoadMainAssetAtPath(path);
            string typeName = mainObj != null ? mainObj.GetType().Name : "Unknown";

            long runtimeBytes = 0;
            if (_showRuntimeMemory && mainObj != null)
            {
                try
                {
                    runtimeBytes = Profiler.GetRuntimeMemorySizeLong(mainObj);
                }
                catch
                {
                    runtimeBytes = 0;
                }
            }

            _rows.Add(new Row
            {
                assetPath = path,
                typeName = typeName,
                bytes = bytes,
                runtimeBytes = runtimeBytes
            });

            _totalBytes += bytes;
            _totalRuntimeBytes += runtimeBytes;
        }

        Repaint();
    }

    private void Clear()
    {
        _rows.Clear();
        _totalBytes = 0;
        _totalRuntimeBytes = 0;
        Repaint();
    }

    private static string FormatBytes(long bytes)
    {
        string[] units = { "B", "KB", "MB", "GB" };
        double size = bytes;
        int unit = 0;

        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }

        return $"{size:0.##} {units[unit]}";
    }
    #endregion
}
