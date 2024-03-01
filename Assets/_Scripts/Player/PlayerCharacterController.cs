using Sirenix.OdinInspector;
using UltEvents;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.Player
{
    public class PlayerCharacterController : SerializedMonoBehaviour
    {
        [Title("Rferences")]
        [SerializeField, Required]
        private CharacterController _characterController;
        [SerializeField, Required]
        private Transform _playerCameraTransform;

        [Title("Config")]
        [SerializeField, Min(0.01f)]
        private float _playerMoveSpeed = 2f;
        [SerializeField]
        private float _playerJumpSpeed = 2f;
        [SerializeField]
        private LayerMask _autoParentToLayers;

        [Title("Events")]
        public UltEvent OnJump = new();

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private Vector2 _playerMoveVector = Vector2.zero;
        [ShowInInspector, ReadOnly]
        private bool _isJumping = false;
        [ShowInInspector, ReadOnly]
        private float _verticalSpeed = 0f;
        [ShowInInspector, ReadOnly]
        private float _cachedWorldGravityMagnitude = 0f;

        private void Start()
        {
            _cachedWorldGravityMagnitude = Physics.gravity.magnitude;
        }

        private void Update()
        {
            _characterController.transform.rotation = Quaternion.identity;
            RaycastForGroundParent();
            MovePlayer(Time.deltaTime);
        }

        private void RaycastForGroundParent()
        {
            if (Physics.Raycast(_characterController.transform.position, Vector3.down, out RaycastHit hit, _autoParentToLayers))
            {
                _characterController.transform.SetParent(hit.transform);
            }
            else
            {
                _characterController.transform.SetParent(null);
            }    
        }

        private void MovePlayer(float deltaTime)
        {
            Vector3 movementThisFrame = InputMovementVectorToCharacterMovementVector(_playerMoveVector);
            movementThisFrame *= _playerMoveSpeed;
            if (_isJumping && _characterController.isGrounded)
            {
                _verticalSpeed = _playerJumpSpeed;
            }
            movementThisFrame.y += _verticalSpeed;
            _verticalSpeed -= _cachedWorldGravityMagnitude * deltaTime;
            _verticalSpeed = Mathf.Max(-_cachedWorldGravityMagnitude, _verticalSpeed);
            if (_verticalSpeed <= 0f)
                _isJumping = false;
            movementThisFrame *= deltaTime;
            _characterController.enabled = true;
            _characterController.Move(movementThisFrame);
            _characterController.enabled = false;
        }

        private Vector3 InputMovementVectorToCharacterMovementVector(Vector2 moveInput)
        {
            Vector3 resultMoveVector = new(moveInput.x, 0, moveInput.y);
            resultMoveVector = _characterController.transform.InverseTransformDirection(_playerCameraTransform.TransformDirection(resultMoveVector));
            resultMoveVector.y = 0;
            return resultMoveVector.normalized;
        }

        public void OnMoveAction(InputAction.CallbackContext context)
        {
            _playerMoveVector = context.ReadValue<Vector2>().normalized;
        }

        public void OnJumpAction(InputAction.CallbackContext context)
        {
            if (context.performed && !_isJumping && _characterController.isGrounded)
            {
                _isJumping = true;
                OnJump?.InvokeSafe();
            }
        }
    }
}
