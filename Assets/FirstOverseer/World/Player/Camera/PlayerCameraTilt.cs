using FirstOverseer.World.Player.Camera.Settings;
using FirstOverseer.World.Player.Movement.Settings;
using UnityEngine;

namespace FirstOverseer.World.Player.Camera
{
    public class PlayerCameraTilt : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform _handleTransform;

        [Header("Settings Configs")]
        [SerializeField] private PlayerCameraSettings _cameraSettings;
        [SerializeField] private PlayerMovementSettings _movementSettings;

        private float _currentRoll;
        private float _currentPitch;

        private void Update()
        {
            if (_rigidbody == null || _handleTransform == null || _cameraSettings == null || _movementSettings == null)
                return;

            CalculateTiltsFromVelocity();
            ApplyRotation();
        }

        private void CalculateTiltsFromVelocity()
        {
            var config = _cameraSettings.Tilt;
            float maxSpeed = _movementSettings.MaxPossibleSpeed;

            Vector3 worldVelocity = _rigidbody.linearVelocity;

            if (worldVelocity.sqrMagnitude < 0.01f)
                worldVelocity = Vector3.zero;

            Vector3 localVelocity = _handleTransform.InverseTransformDirection(worldVelocity);

            float sideSpeed = localVelocity.x;
            if (Mathf.Abs(sideSpeed) < 0.1f)
                sideSpeed = 0f;

            float sideSpeedPercent = Mathf.Clamp(sideSpeed / maxSpeed, -1f, 1f);

            float targetRoll = -sideSpeedPercent * config.MaxRoll * config.TiltAmountZ;

            float currentSpeedZ = (Mathf.Abs(targetRoll) < 0.01f) ? config.ReturnSpeedZ : config.TiltSpeedZ;
            _currentRoll = Mathf.Lerp(_currentRoll, targetRoll, Time.deltaTime * currentSpeedZ);

            float forwardSpeed = localVelocity.z;
            if (Mathf.Abs(forwardSpeed) < 0.1f)
                forwardSpeed = 0f;

            float forwardSpeedPercent = Mathf.Clamp(forwardSpeed / maxSpeed, -1f, 1f);

            float pitchFromForward = forwardSpeedPercent * config.MaxForwardPitch * config.ForwardPitchAmount;

            float pitchFromVertical = -localVelocity.y * config.VerticalTiltAmount;
            pitchFromVertical = Mathf.Clamp(pitchFromVertical, -config.MaxVerticalPitch, config.MaxVerticalPitch);

            float targetPitch = pitchFromForward + pitchFromVertical;

            float currentSpeedX = (Mathf.Abs(targetPitch) < 0.01f) ? config.ReturnSpeedX : config.TiltSpeedX;
            _currentPitch = Mathf.Lerp(_currentPitch, targetPitch, Time.deltaTime * currentSpeedX);
        }

        private void ApplyRotation() => _handleTransform.localRotation = Quaternion.Euler(_currentPitch, 0f, _currentRoll);
    }
}