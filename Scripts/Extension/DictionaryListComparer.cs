
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Comparer for Dictionary<T2, List<T1>> that checks both keys and list contents
/// </summary>
public class DictionaryListComparer<T1, T2> : IEqualityComparer<Dictionary<T2, List<T1>>>
{
    private readonly IEqualityComparer<T1> itemComparer;
    private readonly IEqualityComparer<T2> keyComparer;
    private readonly bool orderMatters;

    public DictionaryListComparer(
        IEqualityComparer<T1> itemComparer = null,
        IEqualityComparer<T2> keyComparer = null,
        bool orderMatters = false)
    {
        this.itemComparer = itemComparer ?? EqualityComparer<T1>.Default;
        this.keyComparer = keyComparer ?? EqualityComparer<T2>.Default;
        this.orderMatters = orderMatters;
    }

    public bool Equals(Dictionary<T2, List<T1>> x, Dictionary<T2, List<T1>> y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;
        if (x.Count != y.Count) return false;

        foreach (var kvp in x)
        {
            if (!y.TryGetValue(kvp.Key, out List<T1> yList))
                return false;

            if (!ListsEqual(kvp.Value, yList))
                return false;
        }

        return true;
    }

    private bool ListsEqual(List<T1> list1, List<T1> list2)
    {
        if (list1 == null && list2 == null) return true;
        if (list1 == null || list2 == null) return false;
        if (list1.Count != list2.Count) return false;

        if (orderMatters)
        {
            for (int i = 0; i < list1.Count; i++)
            {
                if (!itemComparer.Equals(list1[i], list2[i]))
                    return false;
            }
        }
        else
        {
            // Order doesn't matter - check if all items exist in both lists
            var list2Copy = new List<T1>(list2);
            foreach (var item in list1)
            {
                int index = list2Copy.FindIndex(x => itemComparer.Equals(x, item));
                if (index < 0) return false;
                list2Copy.RemoveAt(index);
            }
        }

        return true;
    }

    public int GetHashCode(Dictionary<T2, List<T1>> obj)
    {
        if (obj == null) return 0;

        int hash = 17;
        foreach (var kvp in obj)
        {
            int keyHash = keyComparer.GetHashCode(kvp.Key);
            int listHash = GetListHashCode(kvp.Value);
            
            // Use XOR for order-independent hash
            hash ^= keyHash ^ listHash;
        }

        return hash;
    }

    private int GetListHashCode(List<T1> list)
    {
        if (list == null) return 0;

        if (orderMatters)
        {
            int hash = 17;
            foreach (var item in list)
            {
                hash = hash * 31 + (item != null ? itemComparer.GetHashCode(item) : 0);
            }
            return hash;
        }
        else
        {
            // Order-independent hash
            int hash = 0;
            foreach (var item in list)
            {
                hash ^= (item != null ? itemComparer.GetHashCode(item) : 0);
            }
            return hash;
        }
    }
}