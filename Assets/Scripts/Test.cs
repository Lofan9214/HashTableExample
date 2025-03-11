using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour
{
    public enum HashTableType
    {
        Chaining,
        OpenAddressing
    }

    public HashTableType hashTableType;
    public OpenAddressingStrategy openAddressingStrategy;


    void Start()
    {
        TestHashTable();
    }

    [ContextMenu("Test HashTable")]
    public void TestHashTable()
    {
        ClearConsole();

        IDictionary<int, int> hashtable = null;

        Queue<int> randomQueue = new Queue<int>();

        if (hashTableType == HashTableType.Chaining)
        {
            hashtable = new ChainingHashTable<int, int>();
        }
        else if (hashTableType == HashTableType.OpenAddressing)
        {
            hashtable = new OpenAddressingHashTable<int, int>(openAddressingStrategy);
        }


        for (int i = 0; i < 100; ++i)
        {
            int random = Random.Range(0, 100000);
            if (!hashtable.ContainsKey(random))
            {
                hashtable.Add(random, random);
                if (Random.value < 0.15f)
                {
                    randomQueue.Enqueue(random);
                }
            }
            else
            {
                Debug.Log("Key Duplication");
            }
        }
        int initCount = hashtable.Count;

        int removeCount = randomQueue.Count;

        while (randomQueue.Count > 0)
        {
            int index = randomQueue.Dequeue();
            if (!hashtable.ContainsKey(index))
            {
                Debug.LogError("KeyNotFound");
            }
            if (hashtable[index] != index)
            {
                Debug.LogError("KeyValuePairError");
            }

            hashtable.Remove(index);
        }

        Debug.Log($"StartCount : {initCount}, toRemove: {removeCount}, All: {hashtable.Count}");

        if (initCount - removeCount != hashtable.Count)
        {
            Debug.LogError($"{removeCount}, {hashtable.Count}");
        }

        foreach (var kvp in hashtable)
        {
            Debug.Log(kvp.Key);
        }


        while (randomQueue.Count > 0)
        {
            int index = randomQueue.Dequeue();
            if (!hashtable.ContainsKey(index))
            {
                Debug.LogError("KeyNotFound");
            }
            if (hashtable[index] != index)
            {
                Debug.LogError("KeyValuePairError");
            }

            hashtable.Remove(index);
        }

        //for (int i = 0; i < 100; ++i)
        //{
        //    int random = Random.Range(0, 100000);
        //    if (!hashtable.ContainsKey(random))
        //    {
        //        hashtable.Add(random, random);
        //        if (Random.value < 0.15f)
        //        {
        //            randomQueue.Enqueue(random);
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("Key Duplication");
        //    }
        //}

        //while (randomQueue.Count > 0)
        //{
        //    int index = randomQueue.Dequeue();
        //    hashtable.Remove(index);
        //}

        //foreach (var kvp in hashtable)
        //{
        //    Debug.Log(kvp.Key);
        //}
    }

    private void ClearConsole()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        Type logEntries = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
        clearConsoleMethod.Invoke(new object(), null);
    }
}
