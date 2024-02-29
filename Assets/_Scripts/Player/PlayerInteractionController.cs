using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Interaction;
using UnityEngine;

namespace SunsetSystems.Player
{
    public class PlayerInteractionController : SerializedMonoBehaviour
    {
        [Title("References")]
        [SerializeField, Required]
        private Camera _playerCamera = null;

        [Title("Config")]
        [SerializeField]
        private LayerMask _interactionLayers;
        [SerializeField, Min(0.1f)]
        private float _interactionDistance = 1f;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private IInteractable _currentInteractable = null;

        private void Update()
        {
            Ray ray = _playerCamera.ViewportPointToRay(new(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, _interactionDistance, _interactionLayers))
            {
                IInteractable hitInteractable = hit.collider.GetComponent<IInteractable>();
                if (hitInteractable == null)
                {
                    if (_currentInteractable != null)
                    {
                        _currentInteractable.IsInteractionTarget = false;
                    }
                    _currentInteractable = null;
                }
                else
                {
                    if (hitInteractable != _currentInteractable)
                    {
                        if (_currentInteractable != null)
                            _currentInteractable.IsInteractionTarget = false;
                        hitInteractable.IsInteractionTarget = true;
                        _currentInteractable = hitInteractable;
                    }
                }
            }
            else
            {
                if (_currentInteractable != null)
                    _currentInteractable.IsInteractionTarget = false;
                _currentInteractable = null;
            }
        }
    }
}
