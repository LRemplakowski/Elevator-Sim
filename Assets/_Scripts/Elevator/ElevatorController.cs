using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Elevators
{
    public class ElevatorController : SerializedMonoBehaviour
    {
        private const float ELEVATOR_MOVE_COMPLETION_MARGIN = 0.01f;

        [Title("References")]
        [SerializeField, Required]
        private Rigidbody _elevatorPlatform;
        [SerializeField, Required]
        private GameObject _invisibleWalls;

        [Title("Config")]
        [SerializeField, Min(0.1f)]
        private float _elevatorSpeed = 3f;
        [SerializeField, DictionaryDrawerSettings(KeyLabel = "Level", ValueLabel = "Position")]
        private Dictionary<int, Vector3> _elevatorLevels = new();

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private bool _elevatorInMove = false;

        private IEnumerator _moveElevatorCoroutine = null;

        [Title("Editor Utility")]
        [Button]
        public void MoveToLevel(int level)
        {
            if (_elevatorInMove)
                return;
            if (_elevatorLevels.TryGetValue(level, out Vector3 value))
            {
                if (_moveElevatorCoroutine != null)
                    StopCoroutine(_moveElevatorCoroutine);
                _moveElevatorCoroutine = MoveElevatorPlatformToPosition(value);
                StartCoroutine(_moveElevatorCoroutine);
            }
        }

        private IEnumerator MoveElevatorPlatformToPosition(Vector3 position)
        {
            if (_elevatorPlatform.transform.localPosition.Equals(position))
            {
                yield break;
            }
            _elevatorInMove = true;
            _invisibleWalls.SetActive(true);
            while (_elevatorInMove)
            {
                Vector3 positionDelta = position - _elevatorPlatform.transform.localPosition;
                Vector3 movementDelta = _elevatorSpeed * Time.deltaTime * positionDelta.normalized;
                movementDelta = movementDelta.magnitude > positionDelta.magnitude ? positionDelta : movementDelta;
                _elevatorPlatform.MovePosition(_elevatorPlatform.position + movementDelta);
                if (_elevatorPlatform.transform.localPosition.Equals(position))
                    _elevatorInMove = false;
                yield return null;
            }
            _invisibleWalls.SetActive(false);
        }
    }
}
