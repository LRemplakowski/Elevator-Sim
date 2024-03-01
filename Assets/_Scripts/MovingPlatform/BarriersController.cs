using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.MovingPlatform
{
    public class BarriersController : SerializedMonoBehaviour
    {
        [Title("References")]
        [SerializeField]
        private MovingPlatformController _movingPlatfromController;
        [SerializeField]
        private List<GameObject> _barriersToEnableAtMoveStart = new();
        [SerializeField]
        private List<GameObject> _barriersToDisableAtSplineStart = new();
        [SerializeField]
        private List<GameObject> _barriersToDisableAtSplineEnd = new();

        public void OnPlatformeMoveStart()
        {
            _barriersToEnableAtMoveStart.ForEach(go => go.SetActive(true));
        }

        public void OnPlatformMoveEnd()
        {
            if (_movingPlatfromController.SplineProgress <= 0f)
                _barriersToDisableAtSplineStart.ForEach(go => go.SetActive(false));
            else
                _barriersToDisableAtSplineEnd.ForEach(go => go.SetActive(false));
        }
    }
}
