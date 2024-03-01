using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Elevator
{
    public class ElevatorCallToFloorUI : SerializedMonoBehaviour
    {
        public static ElevatorCallToFloorUI Instance { get; private set; }

        [Title("References")]
        [SerializeField, Required]
        private CanvasGroup _canvasGroup;
        [SerializeField, Required]
        private Button _callElevatorButton;

        [Title("Config")]
        [SerializeField]
        private float _uiAlphaLerpTime = .5f;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private ElevatorCallPanel _currentControlPanel = null;

        private IEnumerator _lerpUIAplhaCoroutine = null;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            _canvasGroup.alpha = 0f;
        }

        public void ShowLevelSelection(ElevatorCallPanel controlPanel)
        {
            _currentControlPanel = controlPanel;
            _callElevatorButton.Select();
            if (_lerpUIAplhaCoroutine != null)
                StopCoroutine(_lerpUIAplhaCoroutine);
            _lerpUIAplhaCoroutine = LerpUIAlphaOverSeconds(1, _uiAlphaLerpTime);
            StartCoroutine(_lerpUIAplhaCoroutine);
        }

        private IEnumerator LerpUIAlphaOverSeconds(float targetAlpha, float seconds, Action onCoroutineComplete = null)
        {
            float time = 0f;
            float startAlpha = _canvasGroup.alpha;
            while (time < seconds)
            {
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / seconds);
                time += Time.deltaTime;
                yield return null;
            }
            _canvasGroup.alpha = targetAlpha;
            _lerpUIAplhaCoroutine = null;
            onCoroutineComplete?.Invoke();
        }

        public void OnCallElevator()
        {
            if (_currentControlPanel != null)
                _currentControlPanel.Interact();
            HideLevelSelection();
        }

        public void HideLevelSelection()
        {
            if (_lerpUIAplhaCoroutine != null)
                StopCoroutine(_lerpUIAplhaCoroutine);
            _lerpUIAplhaCoroutine = LerpUIAlphaOverSeconds(0, _uiAlphaLerpTime);
            StartCoroutine(_lerpUIAplhaCoroutine);
            _currentControlPanel = null;
        }
    }
}
