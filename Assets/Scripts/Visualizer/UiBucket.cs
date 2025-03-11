using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiBucket : MonoBehaviour
{
    private const string indexFormat = "I: {0}";
    public TextMeshProUGUI indexText;
    public UiItem uiItemPrefab;

    private List<UiItem> uiItems;

    public int Index { get; private set; }
    public int Count => uiItems.Count;

    private void Awake()
    {
        uiItems = new List<UiItem>();
    }

    public void Set<TKey, TValue>(int index, LinkedList<KeyValuePair<TKey, TValue>> bucket)
    {
        SetIndex(index);
        InstantiateItem(bucket);
    }

    public void Set<TKey, TValue>(int index, KeyValuePair<TKey, TValue> pair)
    {
        SetIndex(index);
        InstantiateItem(pair);
    }

    public void SetIndex(int index)
    {
        Index = index;
        indexText.text = string.Format(indexFormat, Index);
    }

    public void InstantiateItem<TKey, TValue>(LinkedList<KeyValuePair<TKey, TValue>> bucket)
    {
        foreach (var kvp in bucket)
        {
            InstantiateItem(kvp);
        }
    }

    public void InstantiateItem<TKey, TValue>(KeyValuePair<TKey, TValue> kvp)
    {
        var item = Instantiate(uiItemPrefab, transform);
        item.Set(kvp);
        uiItems.Add(item);
    }

    public void InstantiateItem<TKey, TValue>(TKey key, TValue value)
    {
        InstantiateItem(new KeyValuePair<TKey, TValue>(key, value));
    }

    public void RemoveItem<TKey>(TKey key)
    {
        foreach (var item in uiItems)
        {
            bool removed = item.RemoveIf(key);
            if (removed)
            {
                uiItems.Remove(item);
                break;
            }
        }
    }
}
