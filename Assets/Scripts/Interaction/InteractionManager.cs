using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public Transform player;
    public List<IInteractable> interactables;
    private IInteractable currentInteractable;
    private PlayerInput playerInput;
    private PlayerController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerInput = player.GetComponent<PlayerInput>();
        playerInput.actions["Interact"].performed += onInteractPerformed;
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

    public void onInteractPerformed(InputAction.CallbackContext ctx)
    {
        currentInteractable?.Interact();
    }
}
