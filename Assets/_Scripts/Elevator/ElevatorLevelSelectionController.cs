using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SunsetSystems.Elevators
{
    public class ElevatorLevelSelectionController : SerializedMonoBehaviour
    {
        public static ElevatorLevelSelectionController Instance { get; private set; }

        [Title("References")]
        [SerializeField, Required]
        private CanvasGroup _canvasGroup;
        [SerializeField, Required]
        private RectTransform _buttonParent;
        [SerializeField, AssetsOnly, Required]
        private Button _levelSelectionButtonPrefab;

        [Title("Config")]
        [SerializeField]
        private float _uiAlphaLerpTime = .5f;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private ElevatorControlPanel _currentControlPanel = null;
        [ShowInInspector, ReadOnly]
        private List<Button> _buttonInstances = new();

        private IEnumerator _lerpUIAplhaCoroutine = null;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            _canvasGroup.alpha = 0f;
        }

        public void ShowLevelSelection(IEnumerable<int> levels, ElevatorControlPanel controlPanel)
        {
            if (_currentControlPanel != null)
                Cleanup();
            _currentControlPanel = controlPanel;
            foreach (int level in levels)
            {
                var buttonInstance = Instantiate(_levelSelectionButtonPrefab, _buttonParent);
                string levelString = $"Level {level}";
                buttonInstance.name = levelString;
                buttonInstance.GetComponentInChildren<TextMeshProUGUI>().text = levelString;
                buttonInstance.onClick.AddListener(() => OnLevelSelected(level));
                _buttonInstances.Add(buttonInstance);
            }
            _buttonInstances[0].Select();
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
            _lerpUIAplhaCoroutine = null;
            onCoroutineComplete?.Invoke();
        }

        private void OnLevelSelected(int level)
        {
            _currentControlPanel.SelectedLevel = level;
            _currentControlPanel.Interact();
            HideLevelSelection();
        }

        public void HideLevelSelection()
        {
            if (_lerpUIAplhaCoroutine != null)
                StopCoroutine(_lerpUIAplhaCoroutine);
            _lerpUIAplhaCoroutine = LerpUIAlphaOverSeconds(0, _uiAlphaLerpTime, Cleanup);
            StartCoroutine(_lerpUIAplhaCoroutine);
            _currentControlPanel = null;
        }

        public void Cleanup()
        {
            foreach (Button button in _buttonInstances)
            {
                button.onClick.RemoveAllListeners();
                Destroy(button.gameObject);
            }

            _buttonInstances.Clear();
        }
    }
}
