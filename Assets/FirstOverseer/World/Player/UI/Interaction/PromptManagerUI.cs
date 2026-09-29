using System.Collections.Generic;
using FirstOverseer.Global.Localization;
using UnityEngine;

namespace FirstOverseer.UI.Interaction
{
    public class PromptManagerUI : MonoBehaviour
    {
        public static PromptManagerUI Instance { get; private set; }

        [Header("Локализация")]
        [SerializeField] private LocalizationSO _localization;

        [Header("Настройки Пула")]
        [SerializeField] private InteractionPromptUI _promptPrefab;
        [SerializeField] private Transform _containerTransform;
        [SerializeField] private int _poolCapacity = 4;

        private readonly List<InteractionPromptUI> _pool = new List<InteractionPromptUI>();
        private readonly Dictionary<object, ActivePromptData> _activePrompts = new Dictionary<object, ActivePromptData>();

        private struct ActivePromptData
        {
            public InteractionPromptUI UI;
            public string KeyName;
            public string ActionTypeKey;
            public string DescriptionKey;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            InitializePool();
        }

        private void OnEnable()
        {
            if (_localization != null)
                _localization.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnDisable()
        {
            if (_localization != null)
                _localization.OnLanguageChanged -= OnLanguageChanged;
        }

        private void InitializePool()
        {
            for (int i = 0; i < _poolCapacity; i++)
            {
                InteractionPromptUI instance = Instantiate(_promptPrefab, _containerTransform);
                instance.Hide();
                _pool.Add(instance);
            }
        }

        private void OnLanguageChanged()
        {
            foreach (var kvp in _activePrompts)
            {
                ActivePromptData data = kvp.Value;

                string actionType = Translate(data.ActionTypeKey);
                string description = Translate(data.DescriptionKey);

                data.UI.Show(data.KeyName, actionType, description);
            }
        }

        public void ShowPrompt(object source, string keyName, string actionTypeKey, string descriptionKey)
        {
            if (source == null)
                return;

            string actionType = Translate(actionTypeKey);
            string description = Translate(descriptionKey);

            if (_activePrompts.TryGetValue(source, out ActivePromptData activePrompt))
            {
                activePrompt.KeyName = keyName;
                activePrompt.ActionTypeKey = actionTypeKey;
                activePrompt.DescriptionKey = descriptionKey;
                _activePrompts[source] = activePrompt;

                activePrompt.UI.Show(keyName, actionType, description);
                return;
            }

            InteractionPromptUI freePrompt = GetFreePrompt();

            if (freePrompt == null)
                return;

            freePrompt.Show(keyName, actionType, description);

            _activePrompts.Add(source, new ActivePromptData
            {
                UI = freePrompt,
                KeyName = keyName,
                ActionTypeKey = actionTypeKey,
                DescriptionKey = descriptionKey
            });
        }

        public void HidePrompt(object source)
        {
            if (source == null)
                return;

            if (_activePrompts.TryGetValue(source, out ActivePromptData activePrompt))
            {
                activePrompt.UI.Hide();
                _activePrompts.Remove(source);
            }
        }

        private string Translate(string key) => _localization != null ? _localization.GetText(key) : key;

        private InteractionPromptUI GetFreePrompt()
        {
            for (int i = 0; i < _pool.Count; i++)
                if (_pool[i].IsActive == false)
                    return _pool[i];

            return null;
        }
    }
}