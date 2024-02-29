using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Interaction
{
    public interface IInteractable
    {
        public bool IsInteractionTarget { get; set; }
        public bool CanBeInteractedWith { get; set; }

        /// <summary>
        /// Interact with given interactable object.
        /// </summary>
        /// <returns>Interaction successful</returns>
        public bool Interact();
    }
}
