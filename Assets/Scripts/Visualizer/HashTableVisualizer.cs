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

    private ChainingHashTable<int, UiBucket> buckets = new ChainingHashTable<int, UiBucket>();

    public Transform bucketView;
    public TMP_Dropdown hashTableTypeDropDown;
    public TMP_Dropdown openAddressingStrategyDropDown;
    public TMP_InputField inputKey;
    public TMP_InputField inputValue;

    private HashTableType hashTableType;
    private OpenAddressingStrategy strategy;

    public IDictionary<string, string> hashTable;

    private void Start()
    {
        CreateHashTable();
    }

    public void Refresh<TKey, TValue>(IDictionary<TKey, TValue> dict)
    {
        foreach (var bucket in buckets)
        {
            Destroy(bucket.Value.gameObject);
        }
        buckets.Clear();

        var containerGetter = (IHashTableContainerGet<string, string>)dict;
        var containers = containerGetter.Containers;
        for (int i = 0; i < containers.Length; ++i)
        {
            if (containers[i] == null || containers[i].Count == 0)
            {
                continue;
            }
            var bucket = InstantiateBucket(i);
            bucket.InstantiateItem(containers[i]);
        }
    }

    private void Refresh()
    {
        Refresh(hashTable);
    }

    public UiBucket InstantiateBucket(int index)
    {
        var bucket = Instantiate(uiBucketPrefab, bucketView);
        bucket.SetIndex(index);
        buckets.Add(index, bucket);
        return bucket;
    }

    public void CreateHashTable()
    {
        switch (hashTableTypeDropDown.value)
        {
            case (int)HashTableType.Chaining:
                hashTable = new ChainingHashTable<string, string>();
                break;
            case (int)HashTableType.OpenAddressing:
                var strategy = (OpenAddressingStrategy)openAddressingStrategyDropDown.value;
                hashTable = new OpenAddressingHashTable<string, string>(strategy);
                break;
        }
        hashTableType = (HashTableType)hashTableTypeDropDown.value;
        strategy = (OpenAddressingStrategy)openAddressingStrategyDropDown.value;
        Refresh();
    }

    public void AddPair()
    {
        hashTable.Add(inputKey.text, inputValue.text);
        Refresh();
        return;
        //AddPair(inputKey.text, inputValue.text);
    }

    public void AddPair(string key, string value)
    {
        hashTable.Add(key, value);
        var indexGetter = (IHashTableIndexGetter<string, string>)hashTable;
        int arrayIndex = indexGetter.GetArrayIndex(key);

        if (arrayIndex < 0 || !buckets.ContainsKey(arrayIndex))
        {
            var bucket = InstantiateBucket(arrayIndex);
            bucket.InstantiateItem(key, value);
        }
        else
        {
            buckets[arrayIndex].InstantiateItem(key, value);
        }
    }

    public void RemovePair()
    {
        //hashTable.Remove(inputKey.text);
        //Refresh();

        RemovePair(inputKey.text);
    }

    public void RemovePair(string key)
    {
        if (!hashTable.ContainsKey(key))
        {
            return;
        }
        var indexGetter = (IHashTableIndexGetter<string, string>)hashTable;
        int arrayIndex = indexGetter.GetArrayIndex(key);
        hashTable.Remove(key);

        buckets[arrayIndex].RemoveItem(key);

        if (buckets[arrayIndex].Count == 0)
        {
            Destroy(buckets[arrayIndex].gameObject);
            buckets.Remove(arrayIndex);
        }
    }

    public void ClearTable()
    {
        foreach (var bucket in buckets)
        {
            Destroy(bucket.Value.gameObject);
        }
        buckets.Clear();
        hashTable.Clear();
    }

    public void RandomValueInsert()
    {
        if (hashTableType != (HashTableType)hashTableTypeDropDown.value
            || (hashTableType == HashTableType.OpenAddressing
                && strategy != (OpenAddressingStrategy)openAddressingStrategyDropDown.value))
        {
            CreateHashTable();
        }

        for (int i = 0; i < 100; ++i)
        {
            string randomKey = Random.Range(0, 100000).ToString();
            string randomValue = Random.Range(0, 100000).ToString();

            hashTable.Add(randomKey, randomValue);
            //AddPair(randomKey, randomValue);
        }

        Refresh();
    }
}
