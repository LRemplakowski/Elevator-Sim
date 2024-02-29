using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.Player
{
    public class PlayerCharacterController : SerializedMonoBehaviour
    {
        [Title("Rferences")]
        [SerializeField, Required]
        private CharacterController _playerCharacterController;
        [SerializeField, Required]
        private Transform _playerCameraTransform;

        [Title("Config")]
        [SerializeField, Min(0.01f)]
        private float _playerMoveSpeed = 2f;
        [SerializeField]
        private LayerMask _autoParentToLayers;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private Vector2 _playerMoveVector = Vector2.zero;

        private void Update()
        {
            float timeDelta = Time.deltaTime;
            MovePlayer(timeDelta);
            ParentPlayerToGround();
        }

        private void ParentPlayerToGround()
        {
            if (Physics.Raycast(_playerCameraTransform.position, Vector3.down, out RaycastHit hit, _autoParentToLayers))
            {
                _playerCharacterController.transform.SetParent(hit.collider.transform);
            }
            else
            {
                _playerCharacterController.transform.SetParent(null);
            }    
        }

        private void MovePlayer(float deltaTime)
        {
            Vector3 movementThisFrame = InputMovementVectorToCharacterMovementVector(_playerMoveVector);
            movementThisFrame *= _playerMoveSpeed;
            movementThisFrame += Physics.gravity;
            _playerCharacterController.Move(deltaTime * movementThisFrame);
        }

        private Vector3 InputMovementVectorToCharacterMovementVector(Vector2 moveInput)
        {
            Vector3 resultMoveVector = new(moveInput.x, 0, moveInput.y);
            resultMoveVector = _playerCharacterController.transform.InverseTransformDirection(_playerCameraTransform.TransformDirection(resultMoveVector));
            resultMoveVector.y = 0;
            return resultMoveVector.normalized;
        }

        public void OnMoveAction(InputAction.CallbackContext context)
        {
            _playerMoveVector = context.ReadValue<Vector2>().normalized;
        }
    }
}
