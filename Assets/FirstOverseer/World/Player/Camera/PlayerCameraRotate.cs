using FirstOverseer.Global.Other.Architecture;
using FirstOverseer.Global.Player.Input;
using FirstOverseer.World.Player.Camera.Settings;
using UnityEngine;
using static UnityEngine.Rendering.STP;

namespace FirstOverseer.World.Player.Camera
{
    public class PlayerCameraRotate : MonoBehaviour
    {
        [SerializeField] private Transform _handleTransform;
        [SerializeField] private PlayerCameraSettings _settings;
        [SerializeField] private InputReader _inputReader;

        [Header(nameof(LockChannelSO))]
        [SerializeField] private LockChannelSO _cameraLockChannel;
        [SerializeField] private LockChannelSO _cursorUnlockChannel;

        private float _xRotation;
        private float _yRotation;

        private void Start()
        {
            SetCursorUnlocked(false);
        }

        private void OnEnable()
        {
            _inputReader.Look += HandleLookInput;

            if (_cursorUnlockChannel != null)
            {
                _cursorUnlockChannel.OnLockStateChanged += OnCursorUnlockStateChanged;
                SetCursorUnlocked(_cursorUnlockChannel.IsLocked);
            }
        }

        private void OnDisable()
        {
            _inputReader.Look -= HandleLookInput;

            if (_cursorUnlockChannel != null)
                _cursorUnlockChannel.OnLockStateChanged -= OnCursorUnlockStateChanged;
        }

        private void OnCursorUnlockStateChanged(bool isUnlocked)
        {
            SetCursorUnlocked(isUnlocked);
        }

        private void SetCursorUnlocked(bool unlocked)
        {
            if (unlocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void HandleLookInput(Vector2 lookDelta)
        {
            if (_cameraLockChannel != null && _cameraLockChannel.IsLocked)
                return;

            var config = _settings.Rotation;

            float mouseX = lookDelta.x * config.SensitivityX * Time.deltaTime;
            float mouseY = lookDelta.y * config.SensitivityY * Time.deltaTime;

            _yRotation += mouseX;
            _xRotation -= mouseY;

            _xRotation = Mathf.Clamp(_xRotation, config.MinPitch, config.MaxPitch);
            _handleTransform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
        }
    }
}