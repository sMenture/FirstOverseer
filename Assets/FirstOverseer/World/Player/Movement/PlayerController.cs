using FirstOverseer.Global.Other.Architecture;
using FirstOverseer.Global.Player.Input;
using FirstOverseer.World.Player.Movement.Settings;
using UnityEngine;

namespace FirstOverseer.World.Player.Movement
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private GroundChecker _groundChecker;

        [Header("Settings")]
        [SerializeField] private InputReader _input;
        [SerializeField] private PlayerMovementSettings _settings;

        [Header(nameof(LockChannelSO))]
        [SerializeField] private LockChannelSO _lockControl;

        private readonly PlayerMover _mover = new PlayerMover();
        private readonly PlayerJumper _jumper = new PlayerJumper();
        private readonly PlayerStepper _stepper = new PlayerStepper();

        private Vector2 _currentInput;

        private void Awake()
        {
            _mover.SetCameraTransform(_cameraTransform);
            _jumper.SetSettings(_settings);

            _stepper.SetSettings(_settings);
            _stepper.SetData(_rigidbody);
        }

        private void OnEnable()
        {
            _input.Move += HandleMoveInput;
            _input.Jump += HandleJumpInput;
        }

        private void OnDisable()
        {
            _input.Move -= HandleMoveInput;
            _input.Jump -= HandleJumpInput;
        }

        private void HandleMoveInput(Vector2 input) => _currentInput = input;

        private void HandleJumpInput() => _jumper.RequestJump(_groundChecker.IsGrounded);

        private void FixedUpdate()
        {
            if (_lockControl.IsLocked)
                return;

            Vector3 moveDirection = _mover.CalculateVelocity(_currentInput);
            Vector3 targetVelocity = moveDirection * _settings.WalkSpeed;
            targetVelocity.y = _jumper.CalculateVerticalVelocity(_groundChecker.IsGrounded);

            _rigidbody.linearVelocity = targetVelocity;

            if (_groundChecker.IsGrounded)
                _stepper.Step(moveDirection);
        }
    }
}