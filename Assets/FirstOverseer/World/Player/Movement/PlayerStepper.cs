using FirstOverseer.World.Player.Movement.Settings;
using UnityEngine;

namespace FirstOverseer.World.Player.Movement
{
    public class PlayerStepper
    {
        private Rigidbody _rigidbody;
        private Transform _transform;
        private PlayerMovementSettings _settings;

        public void SetSettings(PlayerMovementSettings settings) => _settings = settings;

        public void SetData(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
            _transform = _rigidbody.transform;
        }

        public void Step(Vector3 moveDirection)
        {
            if (moveDirection.sqrMagnitude < 0.01f)
                return;

            Vector3 direction = moveDirection.normalized;
            Vector3 lowPosition = _transform.position;
            float stepCheckDistance = _settings.StepRayDistance * moveDirection.magnitude;

            if (Physics.Raycast(lowPosition + Vector3.up * 0.1f, direction, out RaycastHit hitLower, stepCheckDistance))
            {
                Vector3 upperOrigin = lowPosition + Vector3.up * _settings.StepHeight;
                if (!Physics.Raycast(upperOrigin, direction, stepCheckDistance * 2f))
                {
                    Vector3 dropOrigin = hitLower.point + Vector3.up * _settings.StepHeight + direction * 0.05f;

                    if (Physics.Raycast(dropOrigin, Vector3.down, out RaycastHit hitUpper, _settings.StepHeight))
                    {
                        Vector3 targetPosition = new Vector3(_rigidbody.position.x, hitUpper.point.y, _rigidbody.position.z);
                        _rigidbody.MovePosition(targetPosition);
                    }
                }
            }
        }
    }
}