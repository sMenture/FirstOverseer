using FirstOverseer.World.Player.Movement.Settings;
using UnityEngine;

namespace FirstOverseer.World.Player.Movement
{
    public class PlayerJumper
    {
        private PlayerMovementSettings Settings;
        private float _verticalVelocity;

        public void SetSettings(PlayerMovementSettings settings) => Settings = settings;

        public void RequestJump(bool isGrounded)
        {
            if (isGrounded && Settings != null)
                _verticalVelocity = Settings.JumpHeight;
        }

        public float CalculateVerticalVelocity(bool isGrounded)
        {
            if (Settings == null)
                return -2f;

            if (isGrounded && _verticalVelocity <= 0f)
                _verticalVelocity = -2f;
            else
                _verticalVelocity -= Settings.Gravity * Time.fixedDeltaTime;

            return _verticalVelocity;
        }
    }
}