using UnityEngine;

namespace FirstOverseer.World.Player.Movement
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] private Transform _targetTransform;

        [Header("Detection Settings")]
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private Vector3 _boxOffset = new Vector3(0f, -0.05f, 0f);
        [SerializeField] private Vector3 _boxSize = new Vector3(0.45f, 0.05f, 0.45f);

        [Header("Optimization")]
        [SerializeField] private float _checkInterval = 0.02f;

        public bool IsGrounded { get; private set; }

        private Transform _cachedTargetTransform;
        private Transform _cachedSelfTransform;
        private float _nextCheckTime;

        private void Awake()
        {
            _cachedSelfTransform = transform;
            _cachedTargetTransform = _targetTransform != null ? _targetTransform : _cachedSelfTransform;
        }

        public void SetTarget(Transform target)
        {
            _targetTransform = target;
            _cachedTargetTransform = target != null ? target : _cachedSelfTransform;
        }

        private void FixedUpdate()
        {
            if (Time.time < _nextCheckTime)
                return;

            _nextCheckTime = Time.time + _checkInterval;

            Vector3 center = _cachedTargetTransform.position + _boxOffset;

            IsGrounded = Physics.CheckBox(
                center,
                _boxSize * 0.5f,
                _cachedTargetTransform.rotation,
                _groundMask,
                QueryTriggerInteraction.Ignore
            );
        }

        private void OnDrawGizmosSelected()
        {
            Transform drawTransform = _cachedTargetTransform != null ? _cachedTargetTransform : transform;

            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Matrix4x4 currentMatrix = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(drawTransform.position + _boxOffset, drawTransform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, _boxSize);

            Gizmos.matrix = currentMatrix;
        }
    }
}