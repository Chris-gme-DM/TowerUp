using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public Transform player;
    public List<IInteractable> interactables;
    private IInteractable currentInteractable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var interactable in interactables)
            // If the Interaction Manager has the player in range of an object
            if (interactable.InInteractionRange(player.position))
            {
                // if no current Interactable is assigned and subscribed
                if (currentInteractable != interactable)
                {
                    currentInteractable?.Unsubscribe();
                    // set current Interactable as the interactable in question
                    currentInteractable = interactable;
                    // Call current Interactable to subscribe to Interaction
                    currentInteractable.Subscribe();
                }

                else if(currentInteractable == interactable)
                {
                    currentInteractable.Unsubscribe();
                    currentInteractable = null;
                }
            }
    }

    public void TriggerInteract()
    {
        currentInteractable?.Interact();
    }
}
