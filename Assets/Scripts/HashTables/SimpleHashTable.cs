using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private const int DefaultCapacity = 16;
    private const float LoadFactor = 0.99f;

    private KeyValuePair<TKey, TValue>[] table;
    private bool[] occupied;
    private int size;
    private int count;

    public SimpleHashTable() : this(DefaultCapacity)
    {
    }

    public SimpleHashTable(int capacity)
    {
        size = capacity;
        table = new KeyValuePair<TKey, TValue>[size];
        occupied = new bool[size];
        count = 0;
    }

    public int GetIndex(TKey key)
    {
        return GetIndex(key, size);
    }

    public int GetIndex(TKey key, int s)
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
                table[index] = new KeyValuePair<TKey, TValue>(key, value);
                occupied[index] = true;
                ++count;
            }
            else if (table[index].Key.Equals(key))
            {
                table[index] = new KeyValuePair<TKey, TValue>(key, value);
            }
            else
            {
                // Todo 충돌 시 과제
                throw new InvalidOperationException("해시 충돌");
            }

        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new TKey[count];
            for (int i = 0, count = 0; i < size; ++i)
            {
                if (occupied[i])
                {
                    keys[count] = table[i].Key;
                    ++count;
                }
            }
            return keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var keys = new TValue[count];
            for (int i = 0, count = 0; i < size; ++i)
            {
                if (occupied[i])
                {
                    keys[count] = table[i].Value;
                    ++count;
                }
            }
            return keys;
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
            table[index] = new KeyValuePair<TKey, TValue>(key, value);
            occupied[index] = true;
            ++count;
        }
        else if (table[index].Key.Equals(key))
        {
            throw new ArgumentException("키 중복");
        }
        else
        {
            // Todo 충돌 시 과제
            throw new InvalidOperationException("해시 충돌");
        }
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        for (int i = 0; i < size; ++i)
        {
            occupied[i] = false;
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
        return occupied[index] && table[index].Key.Equals(key);
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
            if (occupied[i])
            {
                yield return table[i];
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

        if (occupied[index] && table[index].Key.Equals(key))
        {
            occupied[index] = false;
            --count;
            return true;
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        //if (item.Key == null)
        //{
        //    throw new ArgumentException(nameof(item.Key));
        //}

        //int index = GetIndex(item.Key);

        //if (occupied[index] && table[index].Equals(item))
        //{
        //    occupied[index] = false;
        //    --count;
        //    return true;
        //}

        //return false;
        return Remove(item.Key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
        {
            throw new ArgumentException(nameof(key));
        }

        int index = GetIndex(key);

        if (occupied[index] && table[index].Key.Equals(key))
        {
            value = table[index].Value;
            return true;
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
        var newTable = new KeyValuePair<TKey, TValue>[newSize];
        var newOccupied = new bool[newSize];

        for (int i = 0; i < size; ++i)
        {
            if (occupied[i])
            {
                int newIndex = GetIndex(table[i].Key, newSize);

                if (newOccupied[newIndex])
                {
                    // Todo 충돌 시 과제
                    throw new InvalidOperationException("해시 충돌");
                }

                newTable[newIndex] = table[i];
                newOccupied[newIndex] = true;
            }
        }

        size = newSize;
        table = newTable;
        occupied = newOccupied;
    }
}
