using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHashTableIndexGetter<TKey, TValue>
{
    int GetArrayIndex(TKey key);
}
