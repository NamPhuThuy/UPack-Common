using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Extensions for sorting Dictionary<T2, List<T1>> by keys
/// </summary>
public static class DictionarySortExtensions
{
    /// <summary>
    /// Sort dictionary by keys in ascending order
    /// </summary>
    public static IOrderedEnumerable<KeyValuePair<T2, List<T1>>> SortByKey<T1, T2>(
        this Dictionary<T2, List<T1>> dict)
    {
        return dict.OrderBy(kvp => kvp.Key);
    }

    /// <summary>
    /// Sort dictionary by keys in descending order
    /// </summary>
    public static IOrderedEnumerable<KeyValuePair<T2, List<T1>>> SortByKeyDescending<T1, T2>(
        this Dictionary<T2, List<T1>> dict)
    {
        return dict.OrderByDescending(kvp => kvp.Key);
    }

    /// <summary>
    /// Sort dictionary by keys using custom comparer
    /// </summary>
    public static IOrderedEnumerable<KeyValuePair<T2, List<T1>>> SortByKey<T1, T2>(
        this Dictionary<T2, List<T1>> dict,
        IComparer<T2> comparer)
    {
        return dict.OrderBy(kvp => kvp.Key, comparer);
    }

    /// <summary>
    /// Convert to SortedDictionary for automatic sorting
    /// </summary>
    public static SortedDictionary<T2, List<T1>> ToSortedDictionary<T1, T2>(
        this Dictionary<T2, List<T1>> dict,
        IComparer<T2> comparer = null)
    {
        if (comparer != null)
            return new SortedDictionary<T2, List<T1>>(dict, comparer);
        else
            return new SortedDictionary<T2, List<T1>>(dict);
    }
}

// ============================================
// CUSTOM COMPARERS FOR SORTING
// ============================================

/// <summary>
/// Compare Vector3 by distance from origin
/// </summary>
public class Vector3DistanceComparer : IComparer<Vector3>
{
    public int Compare(Vector3 x, Vector3 y)
    {
        float distX = x.magnitude;
        float distY = y.magnitude;
        return distX.CompareTo(distY);
    }
}

/// <summary>
/// Compare Vector3 by X, then Y, then Z
/// </summary>
public class Vector3ComponentComparer : IComparer<Vector3>
{
    public int Compare(Vector3 x, Vector3 y)
    {
        int result = x.x.CompareTo(y.x);
        if (result != 0) return result;
        
        result = x.y.CompareTo(y.y);
        if (result != 0) return result;
        
        return x.z.CompareTo(y.z);
    }
}

/// <summary>
/// Case-insensitive string comparer
/// </summary>
public class CaseInsensitiveStringComparer : IComparer<string>
{
    public int Compare(string x, string y)
    {
        return string.Compare(x, y, System.StringComparison.OrdinalIgnoreCase);
    }
}

// ============================================
// USAGE EXAMPLES
// ============================================

public class DictionarySortExample : MonoBehaviour
{
    public enum Priority { Low, Medium, High, Critical }

    void Start()
    {
        Example1_SortByIntKey();
        Example2_SortByStringKey();
        Example3_SortByEnumKey();
        Example4_SortByVector3Key();
        Example5_SortedDictionary();
        Example6_CustomSorting();
    }

    // Example 1: Sort by integer keys
    void Example1_SortByIntKey()
    {
        var scoreGroups = new Dictionary<int, List<string>>
        {
            { 100, new List<string> { "Alice", "Charlie" } },
            { 50, new List<string> { "Eve" } },
            { 85, new List<string> { "Bob", "David" } }
        };

        Debug.Log("=== Sorted by Score (Ascending) ===");
        foreach (var kvp in scoreGroups.SortByKey())
        {
            Debug.Log($"Score {kvp.Key}: {string.Join(", ", kvp.Value)}");
        }
        // Output: 50, 85, 100

        Debug.Log("=== Sorted by Score (Descending) ===");
        foreach (var kvp in scoreGroups.SortByKeyDescending())
        {
            Debug.Log($"Score {kvp.Key}: {string.Join(", ", kvp.Value)}");
        }
        // Output: 100, 85, 50
    }

    // Example 2: Sort by string keys
    void Example2_SortByStringKey()
    {
        var tagGroups = new Dictionary<string, List<int>>
        {
            { "Weapon", new List<int> { 1, 4 } },
            { "Armor", new List<int> { 3 } },
            { "Consumable", new List<int> { 2, 5 } }
        };

        Debug.Log("=== Sorted by Tag (Alphabetical) ===");
        foreach (var kvp in tagGroups.SortByKey())
        {
            Debug.Log($"{kvp.Key}: {string.Join(", ", kvp.Value)}");
        }
        // Output: Armor, Consumable, Weapon

        // Case-insensitive sorting
        var mixedCase = new Dictionary<string, List<int>>
        {
            { "weapon", new List<int> { 1 } },
            { "Armor", new List<int> { 2 } },
            { "CONSUMABLE", new List<int> { 3 } }
        };

        Debug.Log("=== Case-Insensitive Sort ===");
        foreach (var kvp in mixedCase.SortByKey(new CaseInsensitiveStringComparer()))
        {
            Debug.Log($"{kvp.Key}: {string.Join(", ", kvp.Value)}");
        }
    }

    // Example 3: Sort by enum keys
    void Example3_SortByEnumKey()
    {
        var taskGroups = new Dictionary<Priority, List<string>>
        {
            { Priority.Critical, new List<string> { "Fix crash" } },
            { Priority.Low, new List<string> { "Polish UI" } },
            { Priority.High, new List<string> { "Optimize rendering" } },
            { Priority.Medium, new List<string> { "Add sound effects" } }
        };

        Debug.Log("=== Sorted by Priority ===");
        foreach (var kvp in taskGroups.SortByKey())
        {
            Debug.Log($"{kvp.Key}: {string.Join(", ", kvp.Value)}");
        }
        // Output: Low, Medium, High, Critical (enum order)
    }

    // Example 4: Sort by Vector3 keys
    void Example4_SortByVector3Key()
    {
        var positionGroups = new Dictionary<Vector3, List<GameObject>>
        {
            { new Vector3(5, 0, 0), new List<GameObject> { new GameObject("Far") } },
            { new Vector3(1, 0, 0), new List<GameObject> { new GameObject("Near") } },
            { new Vector3(3, 0, 0), new List<GameObject> { new GameObject("Mid") } }
        };

        Debug.Log("=== Sorted by Distance from Origin ===");
        foreach (var kvp in positionGroups.SortByKey(new Vector3DistanceComparer()))
        {
            Debug.Log($"Position {kvp.Key} (dist: {kvp.Key.magnitude:F2}): {kvp.Value[0].name}");
        }
        // Output: Near (1.00), Mid (3.00), Far (5.00)

        Debug.Log("=== Sorted by X Component ===");
        foreach (var kvp in positionGroups.SortByKey(new Vector3ComponentComparer()))
        {
            Debug.Log($"Position {kvp.Key}: {kvp.Value[0].name}");
        }
    }

    // Example 5: Using SortedDictionary (auto-sorted)
    void Example5_SortedDictionary()
    {
        var dict = new Dictionary<int, List<string>>
        {
            { 3, new List<string> { "third" } },
            { 1, new List<string> { "first" } },
            { 2, new List<string> { "second" } }
        };

        // Convert to SortedDictionary - maintains sort automatically
        var sorted = dict.ToSortedDictionary();

        // Adding new items keeps it sorted
        sorted[0] = new List<string> { "zero" };
        sorted[4] = new List<string> { "fourth" };

        Debug.Log("=== SortedDictionary (Auto-sorted) ===");
        foreach (var kvp in sorted)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value[0]}");
        }
        // Always maintains sorted order: 0, 1, 2, 3, 4
    }

    // Example 6: Mobile game scenario - Sort enemies by spawn wave
    void Example6_CustomSorting()
    {
        // Group enemies by spawn wave number
        var waveEnemies = new Dictionary<int, List<GameObject>>
        {
            { 5, new List<GameObject> { new GameObject("Boss") } },
            { 1, new List<GameObject> { new GameObject("Goblin1"), new GameObject("Goblin2") } },
            { 3, new List<GameObject> { new GameObject("Orc1") } }
        };

        Debug.Log("=== Spawn Enemies in Wave Order ===");
        foreach (var kvp in waveEnemies.SortByKey())
        {
            Debug.Log($"Wave {kvp.Key}: Spawn {kvp.Value.Count} enemies");
            foreach (var enemy in kvp.Value)
            {
                // SpawnEnemy(enemy);
                Debug.Log($"  - {enemy.name}");
            }
        }

        // Reverse order for cleanup
        Debug.Log("=== Despawn Enemies (Reverse Order) ===");
        foreach (var kvp in waveEnemies.SortByKeyDescending())
        {
            Debug.Log($"Wave {kvp.Key}: Despawn {kvp.Value.Count} enemies");
        }
    }
}

// ============================================
// PERFORMANCE-OPTIMIZED APPROACH
// ============================================

public class OptimizedDictionarySorting : MonoBehaviour
{
    // For frequent iterations, cache the sorted result
    private Dictionary<int, List<string>> dataDict;
    private List<KeyValuePair<int, List<string>>> cachedSorted;
    private bool isDirty = true;

    void Start()
    {
        dataDict = new Dictionary<int, List<string>>
        {
            { 3, new List<string> { "c" } },
            { 1, new List<string> { "a" } },
            { 2, new List<string> { "b" } }
        };
    }

    void Update()
    {
        // Only re-sort when data changes
        if (isDirty)
        {
            cachedSorted = dataDict.SortByKey().ToList();
            isDirty = false;
        }

        // Use cached sorted data for rendering
        foreach (var kvp in cachedSorted)
        {
            // Render UI elements in order
        }
    }

    public void AddItem(int key, string value)
    {
        if (!dataDict.ContainsKey(key))
            dataDict[key] = new List<string>();
        
        dataDict[key].Add(value);
        isDirty = true; // Mark for re-sort
    }
}

// ============================================
// LINQ ALTERNATIVES (One-liners)
// ============================================

public class LinqSortingExamples : MonoBehaviour
{
    void Start()
    {
        var dict = new Dictionary<int, List<string>>
        {
            { 3, new List<string> { "c" } },
            { 1, new List<string> { "a" } },
            { 2, new List<string> { "b" } }
        };

        // One-liner approaches
        
        // Approach 1: OrderBy
        var sorted1 = dict.OrderBy(kvp => kvp.Key);
        
        // Approach 2: ToList then Sort
        var sorted2 = dict.ToList();
        sorted2.Sort((a, b) => a.Key.CompareTo(b.Key));
        
        // Approach 3: SortedDictionary
        var sorted3 = new SortedDictionary<int, List<string>>(dict);
        
        Debug.Log("All approaches produce the same result!");
    }
}