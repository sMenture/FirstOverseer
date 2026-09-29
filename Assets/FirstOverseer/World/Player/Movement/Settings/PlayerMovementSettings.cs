using UnityEngine;

namespace FirstOverseer.World.Player.Movement.Settings
{
    [CreateAssetMenu(fileName = "PlayerMovementSettings", menuName = "Settings/Player Movement Settings")]
    public class PlayerMovementSettings : ScriptableObject
    {
        [field: Header("Speed")]
        [field: SerializeField] public float WalkSpeed { get; private set; } = 4f;
        [field: SerializeField] public float SprintSpeed { get; private set; } = 7f;
        [field: SerializeField] public float Acceleration { get; private set; } = 12f;
        [field: SerializeField] public float Deceleration { get; private set; } = 16f;

        [field: Header("Dynamic Slope Modifiers (per degree)")]
        [field: SerializeField] public float UphillSlowdownPerDegree { get; private set; } = 0.05f;
        [field: SerializeField] public float DownhillSpeedupPerDegree { get; private set; } = 0.05f;

        [field: Header("Physics")]
        [field: SerializeField] public float JumpHeight { get; private set; } = 1.5f;
        [field: SerializeField] public float Gravity { get; private set; } = -19.62f;

        [field: Header("Step Climbing")]
        [field: SerializeField] public float StepHeight { get; private set; } = 0.35f;
        [field: SerializeField] public float StepRayDistance { get; private set; } = 0.5f;
        [field: SerializeField] public float StepSmoothness { get; private set; } = 12f;

        public float MaxPossibleSpeed => SprintSpeed * 1.5f;
    }
}