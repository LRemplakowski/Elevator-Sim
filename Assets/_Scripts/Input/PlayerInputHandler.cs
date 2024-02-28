using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SunsetSystems.Input
{
    public class PlayerInputHandler : SerializedMonoBehaviour
    {
        [Title("Rferences")]
        [SerializeField, Required]
        private CharacterController _playerCharacterController;
        [SerializeField, Required]
        private Transform _playerCameraTransform;

        [Title("Config")]
        [SerializeField, Min(0.01f)]
        private float _playerMoveSpeed = 2f;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private Vector2 _playerMoveVector = Vector2.zero;

        private void Update()
        {
            float timeDelta = Time.deltaTime;
            MovePlayer(InputMovementVectorToCharacterMovementVector(_playerMoveVector), _playerMoveSpeed, timeDelta);
        }

        private void MovePlayer(Vector3 movementDirection, float movementSpeed, float deltaTime)
        {
            _playerCharacterController.Move(deltaTime * movementSpeed * movementDirection);
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
