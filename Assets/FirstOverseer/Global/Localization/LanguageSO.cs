using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstOverseer.Global.Localization
{
    [System.Serializable]
    public struct LocalizationEntry
    {
        public string Key;
        [TextArea(1, 4)] public string Value;
    }

    [CreateAssetMenu(menuName = "Global/Localization/" + nameof(LanguageSO))]
    public class LanguageSO : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private string _languageCode = "RU";
        [SerializeField] private string _displayName = "Русский";
        [SerializeField] private List<LocalizationEntry> _entries = new List<LocalizationEntry>();

        private readonly Dictionary<string, string> _table = new Dictionary<string, string>();

        public string LanguageCode => _languageCode;
        public string DisplayName => _displayName;

        public bool TryGetValue(string key, out string value)
        {
            return _table.TryGetValue(key, out value);
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            _table.Clear();
            for (int i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i];
                if (!string.IsNullOrEmpty(entry.Key) && !_table.ContainsKey(entry.Key))
                {
                    _table.Add(entry.Key, entry.Value);
                }
            }
        }
    }
}