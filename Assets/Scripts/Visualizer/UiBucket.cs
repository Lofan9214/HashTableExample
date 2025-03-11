using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiBucket : MonoBehaviour
{
    private const string indexFormat = "I: {0}";
    public TextMeshProUGUI indexText;
    public UiItem uiItemPrefab;

    public void Set<TKey, TValue>(int index, KeyValuePair<TKey, TValue> kvp)
    {
        indexText.text = string.Format(indexFormat, index);

        InstantiateItem(kvp);
    }

    public void Set<TKey, TValue>(int index, LinkedList<KeyValuePair<TKey, TValue>> bucket)
    {
        indexText.text = string.Format(indexFormat, index);

        foreach (var kvp in bucket)
        {
            InstantiateItem(kvp);
        }
    }

    public void InstantiateItem<TKey, TValue>(KeyValuePair<TKey, TValue> kvp)
    {
        var item = Instantiate(uiItemPrefab, transform);
        item.Set(kvp);
    }
}
