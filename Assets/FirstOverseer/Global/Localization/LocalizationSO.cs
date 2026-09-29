using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstOverseer.Global.Localization
{
    [CreateAssetMenu(menuName = "Global/Localization/" + nameof(LocalizationSO))]
    public class LocalizationSO : ScriptableObject
    {
        public event Action OnLanguageChanged;

        [Header("Доступные языки")]
        [SerializeField] private List<LanguageSO> _availableLanguages = new List<LanguageSO>();

        [Header("Текущий язык")]
        [SerializeField] private LanguageSO _currentLanguage;

        public LanguageSO CurrentLanguage => _currentLanguage;
        public IReadOnlyList<LanguageSO> AvailableLanguages => _availableLanguages;

        public string GetText(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            if (_currentLanguage != null && _currentLanguage.TryGetValue(key, out string translatedText))
                return translatedText;

            return $"[{key}]";
        }

        public void SetLanguage(LanguageSO language)
        {
            if (_currentLanguage == language || language == null)
                return;

            _currentLanguage = language;
            OnLanguageChanged?.Invoke();
        }

        public void SetLanguageByCode(string languageCode)
        {
            for (int i = 0; i < _availableLanguages.Count; i++)
                if (_availableLanguages[i] != null && _availableLanguages[i].LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase))
                {
                    SetLanguage(_availableLanguages[i]);
                    return;
                }
        }
    }
}