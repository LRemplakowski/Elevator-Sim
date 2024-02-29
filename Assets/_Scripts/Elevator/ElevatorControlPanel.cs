using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Interaction;
using UnityEngine;

namespace SunsetSystems.Elevators
{
    public class ElevatorControlPanel : SerializedMonoBehaviour, IInteractable
    {
        [Title("References")]
        [SerializeField, Required]
        private Renderer _panelRenderer;
        [SerializeField, Required]
        private ElevatorController _elevatorController;

        [Title("Config")]
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
                ShowInteractionUI(value);
                UpdatePanelColor(value ? _panelHighlightedColor : _panelDefaultColor);
            }
        }
        [ShowInInspector, ReadOnly]
        private bool _canBeInteractedWith = false;
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
        [field: ShowInInspector, ReadOnly]
        public int SelectedLevel { get; set; }

        private IEnumerator _panelColorLerpCoroutine = null;

        private void ShowInteractionUI(bool show)
        {
            if (show)
                ElevatorLevelSelectionController.Instance.ShowLevelSelection(_elevatorController.ElevatorLevels, this);
            else
                ElevatorLevelSelectionController.Instance.HideLevelSelection();
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
                _elevatorController.MoveToLevel(SelectedLevel);
                return true;
            }
            return false;
        }
    }
}
