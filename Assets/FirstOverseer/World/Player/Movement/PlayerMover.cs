using UnityEngine;

namespace FirstOverseer.World.Player.Movement
{
    public class PlayerMover
    {
        private Transform CameraTransform;

        public void SetCameraTransform(Transform camera) => CameraTransform = camera;

        public Vector3 CalculateVelocity(Vector2 input)
        {
            if (CameraTransform == null || input == Vector2.zero)
                return Vector3.zero;

            Quaternion cameraRotationY = Quaternion.Euler(0, CameraTransform.eulerAngles.y, 0);
            Vector3 inputDirection = new Vector3(input.x, 0, input.y).normalized;
            Vector3 moveDirection = cameraRotationY * inputDirection;

            return moveDirection;
        }
    }
}