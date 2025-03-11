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

    public void Set<TKey, TValue>(KeyValuePair<TKey,TValue> kvp)
    {
        keyText.text = string.Format(keyFormat, kvp.Key);
        valueText.text = string.Format(valueFormat, kvp.Value);
    }
}
