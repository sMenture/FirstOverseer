using FirstOverseer.Global.Player.Input.Action;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstOverseer.Global.Player.Input
{
    [CreateAssetMenu(menuName = "Global/" + nameof(InputReader))]
    public class InputReader : ScriptableObject, InputMapSystem.IPlayerActions, InputMapSystem.IUIActions
    {

        public event Action<Vector2> Move = delegate { };
        public event Action<Vector2> Look = delegate { };
        public event Action<bool> Sprint = delegate { };
        public event System.Action Jump = delegate { };
        public event Action<bool> JumpHold = delegate { };

        public event System.Action Interact = delegate { };
        public event System.Action Inventory = delegate { };
        public event System.Action Sonar = delegate { };

        public event System.Action Crouch = delegate { };
        public event System.Action Ultimate = delegate { };

        private InputMapSystem _inputActions;

        private void OnEnable()
        {
            _inputActions = new InputMapSystem();
            _inputActions.Player.SetCallbacks(this);
            _inputActions.UI.SetCallbacks(this);
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            if (_inputActions == null)
                throw new ArgumentNullException(nameof(InputMapSystem));

            _inputActions.Player.SetCallbacks(null);
            _inputActions.UI.SetCallbacks(null);
            _inputActions.Disable();
        }


        #region PlayerAction
        public void OnMove(InputAction.CallbackContext context) => Move.Invoke(context.ReadValue<Vector2>());
        public void OnLook(InputAction.CallbackContext context) => Look.Invoke(context.ReadValue<Vector2>());

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Jump?.Invoke();
                JumpHold?.Invoke(true);
            }
            else if (context.canceled)
            {
                JumpHold?.Invoke(false);
            }
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Sprint?.Invoke(true);
            }
            else if (context.canceled)
            {
                Sprint?.Invoke(false);
            }
        }

        public void OnInventory(InputAction.CallbackContext context)
        {
            if (context.started)
                Inventory?.Invoke();
        }

        public void OnSonar(InputAction.CallbackContext context)
        {
            if (context.started)
                Sonar?.Invoke();
        }

        public void OnAttack(InputAction.CallbackContext context) { }

        public void OnInteract(InputAction.CallbackContext context) => Interact?.Invoke();
        public string GetInteractBindingName() => GetActionKeyBindingName(_inputActions.Player.Interact);

        public string GetInteractActionTypeKey() => GetActionTypeKey(_inputActions.Player.Interact);

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.started)
                Crouch?.Invoke();
        }
        public void OnPrevious(InputAction.CallbackContext context) { }
        public void OnNext(InputAction.CallbackContext context) { }
        #endregion

        #region UI
        public event Action<Vector2> Point = delegate { };
        public event System.Action MouseDown = delegate { };
        public event System.Action MouseUp = delegate { };


        public void OnNavigate(InputAction.CallbackContext context) { }

        public void OnSubmit(InputAction.CallbackContext context) { }

        public void OnCancel(InputAction.CallbackContext context) { }

        public void OnPoint(InputAction.CallbackContext context) => Point?.Invoke(context.ReadValue<Vector2>());

        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.started)
                MouseDown?.Invoke();

            else if (context.canceled)
                MouseUp?.Invoke();
        }


        public void OnRightClick(InputAction.CallbackContext context) { }

        public void OnMiddleClick(InputAction.CallbackContext context) { }

        public void OnScrollWheel(InputAction.CallbackContext context) { }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }
        #endregion

        private string GetActionKeyBindingName(InputAction action)
        {
            if (action == null || action.bindings.Count == 0)
                return string.Empty;

            string path = action.bindings[0].effectivePath;

            int lastSlashIndex = path.LastIndexOf('/');

            if (lastSlashIndex >= 0 && lastSlashIndex < path.Length - 1)
                return path.Substring(lastSlashIndex + 1).ToUpper();

            return action.GetBindingDisplayString(0, InputBinding.DisplayStringOptions.DontIncludeInteractions);
        }
        private string GetActionTypeKey(InputAction action)
        {
            if (_inputActions == null)
                return "prompt.press";

            string interactions = action.interactions;

            if (interactions.IndexOf("hold", System.StringComparison.OrdinalIgnoreCase) >= 0)
                return "prompt.hold";

            return "prompt.press";
        }
    }
}