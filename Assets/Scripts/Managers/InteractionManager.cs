using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [Header("References")]
    // I think using rb for the checks like i used in other places would confuse
    // The StateController works together with the InteractionManager to find Interactables
    public PlayerController pc;
    // I switched to a HashSet to un-/register Interactable Objects dynamically for performance and scalability
    public InteractableBase currentInteractable {  get; private set; }
    public GameObject interactionUI;

    [Header("Checks")]
    // Set a bubble around the player
    public float interactionDistance;

    private void Start()
    {
        pc = FindAnyObjectByType<PlayerController>();
    }
    void Update()
    {
        // Perform a Raycast from the center of the camera
        RaycastHit hit;
        if (Physics.Raycast(pc.cameraTransform.position, pc.cameraTransform.forward, out hit, interactionDistance))
        {
            // Check if the hit object has an InteractableBase component
            InteractableBase interactable = hit.collider.GetComponent<InteractableBase>();
            if (interactable != null)
            {
                // An interactable object is in range
                if (currentInteractable == null || currentInteractable != interactable)
                {
                    currentInteractable = interactable;
                    ToggleInteractionUI(true); // Show the UI
                }
            }
            else
            {
                // No interactable object hit, or hit something non-interactable
                ClearCurrentInteractable();
            }
        }
        else
        {
            // Nothing hit by the raycast
            ClearCurrentInteractable();
        }
    }
    // Player can Trigger the InteractAction when/if an Interactable is in range to be interacted with
    public void TriggerInteract()
    {
        // This is called as soon as the player is in interaction range and presses the interaction key
        currentInteractable.Interact();
    }
    public void ToggleInteractionUI(bool show)
    {
        if (interactionUI != null && interactionUI.activeSelf != show)
        {
            interactionUI.SetActive(show);
        }
    }
    private void ClearCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable = null;
            ToggleInteractionUI(false); // Hide the UI
        }
    }
}
