using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Elevators
{
    public class ElevatorController : SerializedMonoBehaviour
    {
        private const float ELEVATOR_MOVE_COMPLETION_MARGIN = 0.01f;

        [Title("References")]
        [SerializeField, Required]
        private Transform _elevatorPlatform;

        [Title("Config")]
        [SerializeField, Min(0.1f)]
        private float _elevatorSpeed = 3f;
        [SerializeField, DictionaryDrawerSettings(KeyLabel = "Level", ValueLabel = "Position")]
        private Dictionary<int, Vector3> _elevatorLevels = new();
        public IEnumerable<int> ElevatorLevels => _elevatorLevels.Keys;

        [Title("Events")]
        public UltEvent OnElevatorMoveStart = new();
        public UltEvent OnElevatorMoveEnd = new();

        [field: Title("Runtime")]
        [field: ShowInInspector, ReadOnly]
        public bool ElevatorInMove { get; private set; }

        private IEnumerator _moveElevatorCoroutine = null;

        [Title("Editor Utility")]
        [Button]
        public void MoveToLevel(int level)
        {
            if (ElevatorInMove)
                return;
            if (_elevatorLevels.TryGetValue(level, out Vector3 value))
            {
                if (_moveElevatorCoroutine != null)
                    StopCoroutine(_moveElevatorCoroutine);
                _moveElevatorCoroutine = MoveElevatorPlatformToPosition(transform.TransformPoint(value));
                StartCoroutine(_moveElevatorCoroutine);
            }
        }

        private IEnumerator MoveElevatorPlatformToPosition(Vector3 position)
        {
            if (Mathf.Approximately((position - _elevatorPlatform.position).magnitude, 0f))
            {
                ElevatorInMove = false;
                yield break;
            }
            ElevatorInMove = true;
            OnElevatorMoveStart?.InvokeSafe();
            yield return new WaitForFixedUpdate();
            while (ElevatorInMove)
            {
                Vector3 positionDelta = position - _elevatorPlatform.position;
                Vector3 movementDelta = _elevatorSpeed * Time.fixedDeltaTime * positionDelta.normalized;
                movementDelta = movementDelta.magnitude > positionDelta.magnitude ? positionDelta : movementDelta;
                _elevatorPlatform.transform.position = _elevatorPlatform.position + movementDelta;
                if (Mathf.Approximately(movementDelta.magnitude, 0f))
                    ElevatorInMove = false;
                yield return new WaitForFixedUpdate();
            }
            OnElevatorMoveEnd?.InvokeSafe();
        }
    }
}
