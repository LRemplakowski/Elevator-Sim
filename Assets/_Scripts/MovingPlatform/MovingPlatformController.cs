using System.Collections;
using Sirenix.OdinInspector;
using UltEvents;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace SunsetSystems.MovingPlatform
{
    public class MovingPlatformController : SerializedMonoBehaviour
    {
        [Title("References")]
        [SerializeField, Required]
        private SplineContainer _movementSpline = null;
        [SerializeField, ShowIf("@this._movementSpline != null"), ValueDropdown("GetSplineIndexes")]
        private int _splineIndex = 0;
        [SerializeField, Required]
        private Transform _movingPlatform = null;

        [Title("Config")]
        [SerializeField, Min(0.01f)]
        private float _platformSpeed = 10f;
        [SerializeField]
        private bool _holdAtEndOfSpline = true;
        [SerializeField, Min(0f), ShowIf("@this._holdAtEndOfSpline == true")]
        private float _holdTime = 3f;

        [Title("Events")]
        public UltEvent OnSplineMovementStart = new();
        public UltEvent OnSplineMovementEnd = new();

        [Title("Runtime - Not Serialized")]
        [ShowInInspector, Range(0f, 1f), OnValueChanged("OnProgressChangedInEditor")]
        private float _splineProgress = 0f;
        public float SplineProgress => _splineProgress;
        [ShowInInspector]
        private bool _splineMovementActive;
        [ShowInInspector]
        private Vector3 _platformPositionOffset = Vector3.zero;
        [ShowInInspector]
        private Quaternion _platformRotationOffset = Quaternion.identity;
        [ShowInInspector]
        private Spline SelectedSpline => _movementSpline.Splines[_splineIndex];


        private IEnumerator _splineMovementCoroutine = null;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used in ValueDropdown Attribute.")]
        private int[] GetSplineIndexes()
        {
            int numberOfSplines = _movementSpline.Splines.Count;
            int[] result = new int[numberOfSplines];
            for (int i = 0; i < result.Length; i++)
                result[i] = i;
            return result;
        }

        private void Start()
        {
            Vector3 splineStartPosition = SelectedSpline.EvaluatePosition(0f);
            _movingPlatform.transform.position = splineStartPosition;
            _platformPositionOffset = splineStartPosition - _movingPlatform.position;
            Quaternion rotationInFirstSplinePoint = Quaternion.LookRotation(SelectedSpline.EvaluateTangent(0f), SelectedSpline.EvaluateUpVector(0f));
            _platformRotationOffset = rotationInFirstSplinePoint * _movingPlatform.rotation;
            StartSplineMovement();
        }

        [Title("Editor Utility")]
        [Button]
        private void StartSplineMovement()
        {
            if (_splineMovementCoroutine != null)
                StopCoroutine(_splineMovementCoroutine);
            _splineMovementCoroutine = MoveAlongSpline();
            _splineMovementActive = true;
            StartCoroutine(_splineMovementCoroutine);
        }

        private IEnumerator MoveAlongSpline()
        {
            float movementDirectionModifier = 1f;
            OnSplineMovementStart?.InvokeSafe();
            yield return new WaitForFixedUpdate();
            while (_splineMovementActive)
            {
                if (SelectedSpline.Evaluate(_splineProgress, out float3 position, out float3 tangent, out float3 upVector))
                {
                    Quaternion rotation = Quaternion.LookRotation(tangent, upVector) * _platformRotationOffset;
                    _movingPlatform.SetPositionAndRotation((Vector3)position - _platformPositionOffset, rotation);
                }
                float splineLength = SelectedSpline.GetLength();
                float progressThisFrame = (_platformSpeed / splineLength) * Time.fixedDeltaTime;
                _splineProgress += progressThisFrame * movementDirectionModifier;
                if (_splineProgress >= 1f || _splineProgress <= 0f)
                {
                    OnSplineMovementEnd?.InvokeSafe();
                    _splineProgress = Mathf.Clamp01(_splineProgress);
                    movementDirectionModifier = -movementDirectionModifier;
                    if (_holdAtEndOfSpline)
                        yield return new WaitForSeconds(_holdTime);
                    OnSplineMovementStart?.InvokeSafe();
                }
                yield return new WaitForFixedUpdate();
            }
            OnSplineMovementEnd?.InvokeSafe();
            _splineMovementCoroutine = null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used in OnValueChanged Attribute.")]
        private void OnProgressChangedInEditor()
        {
            if (SelectedSpline.Evaluate(_splineProgress, out float3 position, out float3 tangent, out float3 upVector))
            {
                Quaternion rotation = Quaternion.LookRotation(tangent, upVector) * _platformRotationOffset;
                _movingPlatform.transform.SetPositionAndRotation((Vector3)position - _platformPositionOffset, rotation);
            }
        }
    }
}
