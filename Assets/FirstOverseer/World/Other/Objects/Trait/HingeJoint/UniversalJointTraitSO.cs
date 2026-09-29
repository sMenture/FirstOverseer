using FirstOverseer.World.Objects.Traits;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstOverseer.World.Objects
{
    [CreateAssetMenu(menuName = "World/Objects/Traits/" + nameof(UniversalJointTraitSO))]
    public class UniversalJointTraitSO : ObjectTraitSO
    {
        [SerializeReference]
        public List<JointStateData> States = new List<JointStateData>();

        [field: SerializeField] public int DefaultStateIndex { get; private set; } = 0;

        public override ObjectTraitInstance CreateInstance() => new UniversalJointTraitInstance(this);
    }

    [Serializable]
    public class UniversalJointTraitInstance : ObjectTraitInstance
    {
        private readonly UniversalJointTraitSO _data;
        private int _currentStateIndex;

        public ConfigurableJoint Joint { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        public JointStateData CurrentState => GetCurrentState();
        public event Action<JointStateData> OnStateChanged;

        public UniversalJointTraitInstance(UniversalJointTraitSO data) => _data = data;

        protected override void OnItialize()
        {
            if (_data.States != null && _data.States.Count > 0)
                _currentStateIndex = Mathf.Clamp(_data.DefaultStateIndex, 0, _data.States.Count - 1);
        }

        public override void OnEnter()
        {
            Owner.OnInteraction += StepNextState;
        }

        public override void OnExit()
        {
            Owner.OnInteraction -= StepNextState;
        }

        public void BindPhysics(ConfigurableJoint joint, Rigidbody rigidbody)
        {
            Joint = joint;
            Rigidbody = rigidbody;
            ApplyCurrentState();
        }

        public void StepNextState()
        {
            if (_data.States == null || _data.States.Count == 0)
                return;

            _currentStateIndex = (_currentStateIndex + 1) % _data.States.Count;
            ApplyCurrentState();
        }

        public void ApplyCurrentState()
        {
            if (Joint == null || _data.States == null || _data.States.Count == 0)
                return;

            JointStateData state = GetCurrentState();
            ApplyStateToJoint(state);
            OnStateChanged?.Invoke(state);
        }

        private void ApplyStateToJoint(JointStateData state)
        {
            if (Joint == null || state == null)
                return;

            Joint.xMotion = ConfigurableJointMotion.Locked;
            Joint.yMotion = ConfigurableJointMotion.Locked;
            Joint.zMotion = ConfigurableJointMotion.Locked;
            Joint.angularXMotion = ConfigurableJointMotion.Locked;
            Joint.angularYMotion = ConfigurableJointMotion.Locked;
            Joint.angularZMotion = ConfigurableJointMotion.Locked;

            if (state is LinearJointStateData linearState)
            {
                if (linearState.UseLimits)
                {
                    Joint.xMotion = ConfigurableJointMotion.Limited;
                    SoftJointLimit limit = Joint.linearLimit;
                    limit.limit = linearState.MaxDistance;
                    Joint.linearLimit = limit;
                }
                else
                {
                    Joint.xMotion = ConfigurableJointMotion.Free;
                }

                JointDrive xDrive = Joint.xDrive;
                xDrive.positionSpring = linearState.LinearSpring;
                xDrive.positionDamper = linearState.LinearDamper;
                xDrive.maximumForce = float.MaxValue;
                Joint.xDrive = xDrive;

                Joint.targetPosition = new Vector3(-linearState.TargetDistance, 0, 0);
            }
            else if (state is AngularJointStateData angularState)
            {
                Joint.rotationDriveMode = RotationDriveMode.XYAndZ;

                if (angularState.UseLimits)
                {
                    Joint.angularYMotion = ConfigurableJointMotion.Limited;
                    SoftJointLimit yLimit = Joint.angularYLimit;
                    yLimit.limit = angularState.AngleLimit;
                    Joint.angularYLimit = yLimit;
                }
                else
                {
                    Joint.angularYMotion = ConfigurableJointMotion.Free;
                }

                JointDrive yDrive = Joint.angularYZDrive;
                yDrive.positionSpring = angularState.AngularSpring;
                yDrive.positionDamper = angularState.AngularDamper;
                yDrive.maximumForce = float.MaxValue;
                Joint.angularYZDrive = yDrive;

                Joint.targetRotation = Quaternion.Euler(0, angularState.TargetAngle, 0);
            }
        }

        public JointStateData GetCurrentState()
        {
            if (_data.States == null || _data.States.Count == 0)
                return null;

            return _data.States[_currentStateIndex];
        }
    }
}