using System;
using UnityEngine;

namespace FirstOverseer.World.Objects
{
    [Serializable]
    public abstract class JointStateData
    {
        public string StateKey;
    }

    [Serializable]
    public class LinearJointStateData : JointStateData
    {
        [Header("Linear Movement")]
        public float TargetDistance;
        public float LinearSpring = 500f;
        public float LinearDamper = 50f;

        [Header("Linear Limits")]
        public bool UseLimits = true;
        public float MaxDistance = 1f;
    }

    [Serializable]
    public class AngularJointStateData : JointStateData
    {
        [Header("Angular Movement")]
        public float TargetAngle;
        public float AngularSpring = 500f;
        public float AngularDamper = 50f;

        [Header("Angular Limits")]
        public bool UseLimits = true;
        public float AngleLimit = 90f;
    }
}