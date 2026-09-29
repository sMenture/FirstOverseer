using UnityEngine;

namespace FirstOverseer.World.Player.Camera.Settings
{
    [CreateAssetMenu(fileName = "PlayerCameraSettings", menuName = "Settings/Player Camera Settings")]
    public class PlayerCameraSettings : ScriptableObject
    {
        [field: SerializeField] public RotateSettings Rotation { get; private set; }
        [field: SerializeField] public TiltSettings Tilt { get; private set; }
        [field: SerializeField] public BobSettings Bob { get; private set; }

        [System.Serializable]
        public struct RotateSettings
        {
            [Range(1f, 100f)] public float SensitivityX;
            [Range(1f, 100f)] public float SensitivityY;
            public float MinPitch;
            public float MaxPitch;
        }

        [System.Serializable]
        public struct TiltSettings
        {
            [Header("Side Tilt (Roll - Z)")]
            public float TiltAmountZ;
            public float MaxRoll;
            public float TiltSpeedZ;
            public float ReturnSpeedZ;

            [Header("Forward/Backward Tilt (Pitch - X)")]
            public float ForwardPitchAmount;
            public float MaxForwardPitch;
            public float TiltSpeedX;
            public float ReturnSpeedX;

            [Header("Vertical Tilt")]
            public float VerticalTiltAmount;
            public float MaxVerticalPitch;
        }

        [System.Serializable]
        public struct BobSettings
        {
            public float MaxAmplitude;
            public float MinAmplitude;
            public float MinFrequency;
            public float MaxFrequency;
            public float ReturnSpeed;
        }
    }
}