using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UltEvents;
using UnityEngine;

namespace SunsetSystems.Core
{
    public class FloorDetector : MonoBehaviour
    {
        [Title("References")]
        [SerializeField]
        private Transform _playerTransform;
        [Title("Config")]
        [SerializeField]
        private List<FloorDefinition> _playableFloors = new();
        [Title("Events")]
        public UltEvent<int> OnPlayerFloorChanged = new();
        [Title("Runtime")]
        [ShowInInspector]
        private int _floorLastFrame = -1;

        private void Update()
        {
            for (int i = 0; i < _playableFloors.Count; i++)
            {
                if (_playableFloors[i].IsPositionOnFloor(_playerTransform.position) && _floorLastFrame != i)
                {
                    OnPlayerFloorChanged?.InvokeSafe(i);
                    _floorLastFrame = i;
                }
            }
        }

        [Serializable]
        private struct FloorDefinition
        {
            public float FloorStartY;
            public float FloorHeight;
            public Color FloorGizmoColor;

            public bool IsPositionOnFloor(Vector3 position)
            {
                return position.y >= FloorStartY && position.y <= FloorStartY + FloorHeight;
            }
        }

        private void OnDrawGizmosSelected()
        {
            foreach(var floorDef in _playableFloors)
            {
                Gizmos.color = floorDef.FloorGizmoColor;
                Vector3 gizmoCenter = transform.position;
                gizmoCenter.y += floorDef.FloorStartY + (floorDef.FloorHeight / 2);
                Vector3 gizmoSize = new(1, floorDef.FloorHeight, 1);
                Gizmos.DrawCube(gizmoCenter, gizmoSize);
            }
        }
    }
}
