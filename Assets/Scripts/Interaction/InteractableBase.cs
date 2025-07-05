using UnityEngine;
using UnityEngine.InputSystem;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Objectdata")]
    [SerializeField] string interactableName;
    private GameObject interactionPanel;
    private PlayerController playerController;
    // Scalable Interaction Range to determine Interactibility
    [Range(0f, 5f)] float interactionRange;

    protected virtual void Awake()
    {
        // The Subscribe and Unsubscribe Method have the new Purpose to switch the interaction panel on and off
        interactionPanel = GameObject.Find("InteractionPanel");
        if (interactionPanel != null)
        {
            // Don't display on Start
            interactionPanel.SetActive(false);
        }
        // just find the player automatically
        playerController = FindAnyObjectByType<PlayerController>();
    }
    public abstract void Interact();
    public virtual void onSubscribe()
    {
        // When the player hits the raycast requirements of the given Interactable it subscribes itself to to the interaction Manager that can call upon its Interaction
        // The UI displays Interactibility
        if (interactionPanel != null) 
            interactionPanel.SetActive(true);
    }
    public virtual void onUnsubscribe()
    {
        // when the player leaves the interaction range the UI prompt for interaction should disappear
        // Disable Interactability
        if (interactionPanel != null)
            interactionPanel.SetActive(false);

    }
    // The range for the interactions need to be set for each type of interactable
    public bool InInteractionRange(Vector3 playerPosition)
    {
        return Vector3.Distance(playerPosition, transform.position) <= interactionRange;
    }
    public float GetInteractionRange()
    {
        return interactionRange;
    }
}