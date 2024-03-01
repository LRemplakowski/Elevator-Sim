using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace SunsetSystems.Core
{
    public class StatsDisplay : MonoBehaviour
    {
        private const string TIME_SINCE_LEVEL_START_TEXT = "Level Time:";
        private const string NUMBER_OF_JUMPS_TEXT = "Jumps:";
        private const string CURRENT_FLOOR_TEXT = "Floor:";

        [Title("References")]
        [SerializeField, Required]
        private TextMeshProUGUI _timeSinceLevelStart;
        [SerializeField, Required]
        private TextMeshProUGUI _numberOfJumps;
        [SerializeField, Required]
        private TextMeshProUGUI _currentFloor;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private StatsManager _statsManagerInstance = null;

        private void Start()
        {
            _statsManagerInstance = StatsManager.Instance;
        }

        private void Update()
        {
            var statsData = _statsManagerInstance.CurrentStats;
            _timeSinceLevelStart.text = $"{TIME_SINCE_LEVEL_START_TEXT} {statsData.SecondsSinceLevelStart:0.00}";
            _numberOfJumps.text = $"{NUMBER_OF_JUMPS_TEXT} {statsData.NumberOfJumps}";
            _currentFloor.text = $"{CURRENT_FLOOR_TEXT} {statsData.CurrentFloor}";
        }
    }
}
