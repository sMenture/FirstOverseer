using UnityEngine;

namespace FirstOverseer.World.Objects
{
    [RequireComponent(typeof(ConfigurableJoint))]
    [RequireComponent(typeof(Rigidbody))]
    public class ConfigurableJointController : MonoBehaviour
    {
        [SerializeField] private WorldObject _worldObject;

        private ConfigurableJoint _joint;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _joint = GetComponent<ConfigurableJoint>();
            _rigidbody = GetComponent<Rigidbody>();

            if (_worldObject == null)
                _worldObject = GetComponent<WorldObject>();
        }

        private void Start()
        {
            if (_worldObject != null && _worldObject.Instance != null)
            {
                var trait = _worldObject.Instance.GetTrait<UniversalJointTraitInstance>();
                trait?.BindPhysics(_joint, _rigidbody);
            }
        }
    }
}