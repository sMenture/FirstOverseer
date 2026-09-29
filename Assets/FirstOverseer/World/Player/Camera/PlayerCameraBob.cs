using FirstOverseer.World.Player.Camera.Settings;
using FirstOverseer.World.Player.Movement.Settings;
using UnityEngine;

namespace FirstOverseer.World.Player.Camera
{
    public class PlayerCameraBob : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform _handleTransform;

        [Header("Settings Configs")]
        [SerializeField] private PlayerCameraSettings _cameraSettings;
        [SerializeField] private PlayerMovementSettings _movementSettings;

        private float _bobTimer;

        private void Update()
        {
            if (_rigidbody == null || _handleTransform == null || _cameraSettings == null || _movementSettings == null)
                return;

            var config = _cameraSettings.Bob;
            Vector3 velocity = _rigidbody.linearVelocity;
            float speed = new Vector2(velocity.x, velocity.z).magnitude;

            if (speed < 0.01f)
            {
                _handleTransform.localPosition = Vector3.Lerp(_handleTransform.localPosition, Vector3.zero, Time.deltaTime * config.ReturnSpeed);
                return;
            }

            float maxSpeed = _movementSettings.MaxPossibleSpeed;
            float t = Mathf.Clamp01(speed / maxSpeed);

            float amplitude = Mathf.Lerp(config.MaxAmplitude, config.MinAmplitude, t);
            float frequency = Mathf.Lerp(config.MinFrequency, config.MaxFrequency, t);

            _bobTimer += Time.deltaTime * frequency;

            float bobOffset = Mathf.Sin(_bobTimer) * amplitude;

            _handleTransform.localPosition = Vector3.up * bobOffset;
        }
    }
}