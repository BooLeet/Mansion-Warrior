using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Localization/Language Pack")]
public class LanguagePack : ScriptableObject
{

    [System.Serializable]
    public struct KeyValue
    {
        public string key;
        public string value;
        public KeyValue(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }

    [System.Serializable]
    public struct KeyValuePack
    {
        public KeyValue[] keyValues;
    }

    public KeyValuePack[] keyValuePacks;

    public string GetString(string key)
    {
        foreach (KeyValuePack pack in keyValuePacks)
            foreach (KeyValue keyValue in pack.keyValues)
            {
                if (keyValue.key == key)
                    return keyValue.value;
            }

        return key;
    }

}
