using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Elevators;
using SunsetSystems.Interaction;
using UnityEngine;

namespace SunsetSystems.Elevator
{
    public class ElevatorCallPanel : SerializedMonoBehaviour, IInteractable
    {
        [Title("References")]
        [SerializeField, Required]
        private Renderer _panelRenderer;
        [SerializeField, Required]
        private ElevatorController _elevatorController;

        [Title("Config")]
        [SerializeField, ValueDropdown("GetElevatorLevels")]
        private int _correspondingElevatorLevel = 0;
        [SerializeField]
        private Color _panelHighlightedColor = Color.green;
        [SerializeField]
        private Color _panelDefaultColor = Color.red;
        [SerializeField]
        private float _panelColorChangeDuration = .5f;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private bool _isInteractionTarget = false;
        public bool IsInteractionTarget
        {
            get
            {
                return _isInteractionTarget;
            }
            set
            {
                _isInteractionTarget = value;
                ShowInteractionUI(_isInteractionTarget && _canBeInteractedWith);
                UpdatePanelColor(_isInteractionTarget && _canBeInteractedWith ? _panelHighlightedColor : _panelDefaultColor);
            }
        }
        [ShowInInspector, ReadOnly]
        private bool _canBeInteractedWith = true;
        public bool CanBeInteractedWith
        {
            get
            {
                return _canBeInteractedWith;
            }
            set
            {
                _canBeInteractedWith = value;
                if (value is false)
                    IsInteractionTarget = false;
            }
        }

        private IEnumerator _panelColorLerpCoroutine = null;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private int[] GetElevatorLevels()
        {
            return _elevatorController?.ElevatorLevels.ToArray() ?? new int[0];
        }

        private void ShowInteractionUI(bool show)
        {
            if (show)
                ElevatorCallToFloorUI.Instance.ShowLevelSelection(this);
            else
                ElevatorCallToFloorUI.Instance.HideLevelSelection();
        }

        private void UpdatePanelColor(Color newColor)
        {
            if (_panelColorLerpCoroutine != null)
                StopCoroutine(_panelColorLerpCoroutine);
            _panelColorLerpCoroutine = LerpPanelColorOverTime(newColor, _panelColorChangeDuration);
            StartCoroutine(_panelColorLerpCoroutine);

            IEnumerator LerpPanelColorOverTime(Color targetColor, float seconds)
            {
                float time = 0f;
                Color startColor = _panelRenderer.material.color;
                while (time < seconds)
                {
                    Color color = Color.Lerp(startColor, targetColor, time / seconds);
                    time += Time.deltaTime;
                    _panelRenderer.material.color = color;
                    yield return null;
                }
                _panelRenderer.material.color = targetColor;
            }
        }

        public bool Interact()
        {
            if (_elevatorController.ElevatorInMove is false)
            {
                _elevatorController.MoveToLevel(_correspondingElevatorLevel);
                return true;
            }
            return false;
        }
    }
}
