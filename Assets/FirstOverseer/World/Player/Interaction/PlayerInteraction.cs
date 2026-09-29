using FirstOverseer.Global.Player.Input;
using FirstOverseer.UI.Interaction;
using FirstOverseer.World.Opportunities;
using UnityEngine;

namespace FirstOverseer.World.Player.Interaction
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Зависимости")]
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private Transform _cameraTransform;

        [Header("Настройки Проверки")]
        [SerializeField] private float _interactDistance = 3f;
        [SerializeField] private LayerMask _interactableLayer;

        private IInteractable _currentInteractable;

        private void OnEnable() => _inputReader.Interact += OnInteractPressed;

        private void OnDisable() => _inputReader.Interact -= OnInteractPressed;

        private void Update()
        {
            Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, _interactDistance, _interactableLayer))
                if (hit.collider.TryGetComponent(out IInteractable interactable) && interactable.CanInteract)
                {
                    SetFocus(interactable);
                    return;
                }

            ClearFocus();
        }

        private void SetFocus(IInteractable interactable)
        {
            if (_currentInteractable == interactable)
                return;

            _currentInteractable = interactable;

            string keyName = _inputReader.GetInteractBindingName();
            string actionTypeKey = _inputReader.GetInteractActionTypeKey();
            string descriptionKey = _currentInteractable.InteractionDescriptionKey;

            PromptManagerUI.Instance?.ShowPrompt(this, keyName, actionTypeKey, descriptionKey);
        }

        private void ClearFocus()
        {
            if (_currentInteractable != null)
            {
                _currentInteractable = null;
                PromptManagerUI.Instance?.HidePrompt(this);
            }
        }

        private void OnInteractPressed()
        {
            if (_currentInteractable != null && _currentInteractable.CanInteract)
                _currentInteractable.Interact();
        }
    }
}