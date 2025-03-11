using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HashTableVisualizer : MonoBehaviour
{
    public enum HashTableType
    {
        Chaining,
        OpenAddressing
    }

    public UiBucket uiBucketPrefab;

    private List<UiBucket> buckets = new List<UiBucket>();

    public Transform bucketView;
    public TMP_Dropdown hashTableType;
    public TMP_Dropdown openAddressingStrategy;
    public TMP_InputField inputKey;
    public TMP_InputField inputValue;

    public IDictionary<string, string> hashTable;

    private void Start()
    {
        CreateHashTable();
    }

    public void Refresh<TKey, TValue>(IDictionary<TKey, TValue> dict)
    {
        foreach (var bucket in buckets)
        {
            Destroy(bucket.gameObject);
        }
        buckets.Clear();

        if (dict is ChainingHashTable<TKey, TValue> chain)
        {
            var containers = chain.Containers;

            for (int i = 0; i < containers.Length; ++i)
            {
                if (containers[i] == null || containers[i].Count == 0)
                {
                    continue;
                }
                var bucket = Instantiate(uiBucketPrefab, bucketView);
                bucket.Set(i, containers[i]);
                buckets.Add(bucket);
            }
        }
        else if (dict is OpenAddressingHashTable<TKey, TValue> open)
        {
            var containers = open.Containers;
            var occupied = open.Occupied;

            for (int i = 0; i < containers.Length; ++i)
            {
                if (!occupied[i])
                {
                    continue;
                }
                var bucket = Instantiate(uiBucketPrefab, bucketView);
                bucket.Set(i, containers[i]);
                buckets.Add(bucket);
            }
        }
    }
    

    private void Refresh()
    {
        Refresh(hashTable);
    }

    public void CreateHashTable()
    {
        switch (hashTableType.value)
        {
            case (int)HashTableType.Chaining:
                hashTable = new ChainingHashTable<string, string>();
                break;
            case (int)HashTableType.OpenAddressing:
                var strategy = (OpenAddressingStrategy)openAddressingStrategy.value;
                hashTable = new OpenAddressingHashTable<string, string>(strategy);
                break;
        }
        Refresh();
    }

    public void AddPair()
    {
        hashTable.Add(inputKey.text, inputValue.text);
        Refresh();
    }

    public void RemovePair()
    {
        hashTable.Remove(inputKey.text);
        Refresh();
    }

    public void ClearTable()
    {
        foreach (var bucket in buckets)
        {
            Destroy(bucket.gameObject);
        }
        buckets.Clear();
        hashTable.Clear();
        Refresh();
    }
}
