using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private const int DefaultCapacity = 16;
    private const float LoadFactor = 0.75f;

    private LinkedList<KeyValuePair<TKey, TValue>>[] table;
    private bool[] occupied;
    private int size;
    private int count;

    public ChainingHashTable() : this(DefaultCapacity)
    {
    }

    public ChainingHashTable(int capacity)
    {
        size = capacity;
        table = new LinkedList<KeyValuePair<TKey, TValue>>[size];
        occupied = new bool[size];
        count = 0;
    }

    private int GetIndex(TKey key)
    {
        return GetIndex(key, size);
    }

    private int GetIndex(TKey key, int s)
    {
        if (key == null)
        {
            throw new ArgumentException(nameof(key));
        }

        int hash = key.GetHashCode();
        return Mathf.Abs(hash) % s;
    }

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }

            throw new KeyNotFoundException("키 없음");
        }
        set
        {
            if (key == null)
            {
                throw new ArgumentException(nameof(key));
            }

            if ((float)count / size > LoadFactor)
            {
                Resize();
            }

            int index = GetIndex(key);

            if (!occupied[index])
            {
                table[index] = new LinkedList<KeyValuePair<TKey, TValue>>();
                table[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
                occupied[index] = true;
                ++count;
                return;
            }
            foreach (var kvp in table[index])
            {
                if (kvp.Key.Equals(key))
                {
                    table[index].Remove(kvp);
                    table[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
                    return;
                }
            }
            table[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
            ++count;
        }
    }

    public LinkedList<KeyValuePair<TKey, TValue>>[] Containers
    {
        get
        {
            return table;
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>(count);

            for (int i = 0; i < table.Length; ++i)
            {
                if (!occupied[i])
                {
                    continue;
                }
                foreach (var kvp in table[i])
                {
                    keys.Add(kvp.Key);
                }
            }
            return keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var values = new List<TValue>(count);

            for (int i = 0; i < table.Length; ++i)
            {
                if (!occupied[i])
                {
                    continue;
                }
                foreach (var kvp in table[i])
                {
                    values.Add(kvp.Value);
                }
            }
            return values;
        }
    }

    public int Count => count;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        if (key == null)
        {
            throw new ArgumentException(nameof(key));
        }

        if ((float)count / size > LoadFactor)
        {
            Resize();
        }

        int index = GetIndex(key);

        if (!occupied[index])
        {
            table[index] = new LinkedList<KeyValuePair<TKey, TValue>>();
            table[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
            occupied[index] = true;
            ++count;
            return;
        }

        foreach (var kvp in table[index])
        {
            if (kvp.Key.Equals(key))
            {
                throw new ArgumentException("키 중복");
            }
        }

        table[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
        ++count;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        for (int i = 0; i < size; ++i)
        {
            if (table[i] != null)
            {
                table[i].Clear();
            }
        }
        count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ContainsKey(item.Key);
    }

    public bool ContainsKey(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentException(nameof(key));
        }

        int index = GetIndex(key);

        if (!occupied[index])
        {
            return false;
        }

        foreach (var kvp in table[index])
        {
            if (kvp.Key.Equals(key))
            {
                return true;
            }
        }

        return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (arrayIndex < 0 || array.Length <= arrayIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }

        if (array.Length < arrayIndex + count)
        {
            throw new ArgumentException("공간 부족");
        }

        int currentIndex = arrayIndex;

        foreach (var kvp in this)
        {
            array[currentIndex] = kvp;
            ++currentIndex;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < size; ++i)
        {
            if (!occupied[i])
            {
                continue;
            }
            foreach (var kvp in table[i])
            {
                yield return kvp;
            }
        }
    }

    public bool Remove(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentException(nameof(key));
        }

        int index = GetIndex(key);

        if (table[index] != null)
        {
            foreach (var kvp in table[index])
            {
                if (kvp.Key.Equals(key))
                {
                    table[index].Remove(kvp);
                    --count;
                    return true;
                }
            }
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
        {
            throw new ArgumentException(nameof(key));
        }

        int index = GetIndex(key);

        if (table[index] != null)
        {
            foreach (var kvp in table[index])
            {
                if (kvp.Key.Equals(key))
                {
                    value = kvp.Value;
                    return true;
                }
            }
        }

        value = default(TValue);
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void Resize()
    {
        int newSize = size * 2;
        var newTable = new LinkedList<KeyValuePair<TKey, TValue>>[newSize];
        bool[] newOccupied = new bool[newSize];
        int newLinkedListCount = 0;

        for (int i = 0; i < size; ++i)
        {
            if (table[i] != null)
            {
                foreach (var kvp in table[i])
                {
                    int newIndex = GetIndex(kvp.Key, newSize);

                    if (!newOccupied[newIndex])
                    {
                        newOccupied[newIndex] = true;
                        newTable[newIndex] = new LinkedList<KeyValuePair<TKey, TValue>>();
                        ++newLinkedListCount;
                    }
                    newTable[newIndex].AddLast(kvp);
                }
            }
        }

        size = newSize;
        table = newTable;
        occupied = newOccupied;
    }
}
