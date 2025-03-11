using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiItem : MonoBehaviour
{
    private const string keyFormat = "K: {0}";
    private const string valueFormat = "V: {0}";
    public TextMeshProUGUI keyText;
    public TextMeshProUGUI valueText;

    private string key;

    public void Set<TKey, TValue>(KeyValuePair<TKey, TValue> kvp)
    {
        key = kvp.Key.ToString();
        keyText.text = string.Format(keyFormat, kvp.Key);
        valueText.text = string.Format(valueFormat, kvp.Value);
    }

    public void Set<TKey, TValue>(TKey key, TValue value)
    {
        Set(new KeyValuePair<TKey, TValue>(key, value));
    }

    public bool RemoveIf<TKey>(TKey key)
    {
        if (this.key == key.ToString())
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
