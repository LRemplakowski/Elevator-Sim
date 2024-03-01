using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Core
{
    public class StatsManager : SerializedMonoBehaviour
    {
        public static StatsManager Instance { get; private set; }

        [SerializeField, ReadOnly]
        private GameStatsData _statsData = new();

        public GameStatsData CurrentStats => _statsData;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Update()
        {
            _statsData.SecondsSinceLevelStart += Time.deltaTime;
        }

        public void AddJump()
        {
            _statsData.NumberOfJumps++;
        }

        public void SetCurrentFloor(int floor)
        {
            _statsData.CurrentFloor = floor;
        }
    }

    public struct GameStatsData
    {
        public float SecondsSinceLevelStart;
        public int NumberOfJumps;
        public int CurrentFloor;
    }
}
