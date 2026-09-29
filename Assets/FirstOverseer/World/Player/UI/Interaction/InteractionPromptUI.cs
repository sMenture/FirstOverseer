using TMPro;
using UnityEngine;

namespace FirstOverseer.UI.Interaction
{
    public class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private GameObject _container;
        [SerializeField] private TextMeshProUGUI _promptText;
        [SerializeField] private TextMeshProUGUI _buttonText;

        public bool IsActive => _container.activeSelf;

        public void Show(string keyName, string actionType, string description)
        {
            _container.SetActive(true);

            _buttonText.text = $"{keyName}";

            string formattedActionType = string.IsNullOrEmpty(actionType) ? string.Empty : char.ToUpper(actionType[0]) + actionType.Substring(1);
            string formattedDescription = description?.ToLower() ?? string.Empty;

            _promptText.text = $"{formattedActionType}, {formattedDescription}";
        }

        public void Hide()
        {
            _container.SetActive(false);
        }
    }
}