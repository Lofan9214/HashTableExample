using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHashTableContainerGet<TKey, TValue>
{
    LinkedList<KeyValuePair<TKey, TValue>>[] Containers { get; }
}
