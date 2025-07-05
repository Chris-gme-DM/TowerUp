using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [Header("References")]
    // I think using rb for the checks like i used in other places would confuse
    public Transform player;
    // The StateController works together with the InteractionManager to find Interactables
    public StateController sc;
    // I switched to a HashSet to un-/register Interactable Objects dynamically for performance and scalability
    private List<InteractableBase> interactables;
    private HashSet<InteractableBase> registeredInteractables = new();
    private InteractableBase currentInteractable = null;

    [Header("Checks")]
    // should move this to the stateController probably
    // Set a bubble around the player
    public float checkRadius = 10f;
    // walls were not enough, and obstacles can be turned into something players can maybe jump over, more parcour
    public LayerMask whatIsObstacle;
    public LayerMask whatIsInteractable;
    public LayerMask whatIsWall;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(player == null)
        // Find the player object automatically since only one player is supposed to be in any given scene
            player = GameObject.FindWithTag("Player").transform;
        if(sc == null)
            sc = FindAnyObjectByType<StateController>();

        // Find Interactables
        interactables = new List<InteractableBase>(FindObjectsByType<InteractableBase>(FindObjectsSortMode.None));
        // Initially existing interactables register to the Manager
        foreach(InteractableBase interactable in interactables)
        {
            RegisterInteractable(interactable);
        }

        // Find Layer Mask whatIsWall in statecontroller
        whatIsWall = sc.whatIsWall;
    }

    // Update is called once per frame
    void Update()
    {
        // For each instance of any Interactable, they all have a raycast to detect the player
        // I considered if i will need to check for the next closest interactable at some point
        // After consideration need to research how to do it dynamically
        CheckInteractables();
    }
    private void CheckInteractables()
    {
        InteractableBase closestInteractable = null;
        float closestDistance = float.MaxValue;

        //Find Interactables in the player bubble, like a broadcast that registers interactables in checkRadius
        Collider[] hitColliders = Physics.OverlapSphere(player.position, checkRadius, whatIsInteractable);
        // Need a new list for potential interactables in player radius
        List<InteractableBase> potentialInteractables = new();
        foreach(Collider col in hitColliders)
        {
            InteractableBase interactable = col.GetComponent<InteractableBase>();
            potentialInteractables.Add(interactable);
        }

        // Checks the distance to all potential Interactables in checkRadius and narrow down to the intended interactable
        foreach (var interactable in potentialInteractables)
        {
            // Check distance to Interactables in reach
            float distance = Vector3.Distance(player.position, interactable.transform.position);

            // If the Interaction Manager has the player in the specified range of an object
            if (interactable.InInteractionRange(player.position))
            {
                // Raycast from camera to check if player looks at interactable
                // Player should look at interactable to interact with it
                Vector3 rayOrigin = sc.cameraTransform.position;
                Vector3 rayToInteractable = (interactable.transform.position - rayOrigin).normalized;
                RaycastHit hit;

                // if the player looks to the interactable and the ray hits it and NOT a f wall
                // If the raycast hits a wall or an obstacle it can't reach the interactable
                if (Physics.Raycast(rayOrigin, rayToInteractable, out hit, interactable.GetInteractionRange(), (whatIsObstacle ^ whatIsWall) | whatIsInteractable ))
                {
                    // if the raycast hits the interactable itself
                    if (hit.collider.GetComponent<InteractableBase>() == interactable)
                    {
                        // Check which one is closest, if a fringe case will ever lead to confusion
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestInteractable = interactable;
                        }
                    }
                }
            }
        }

        // Need to be careful if i put my plan to make a vault motion for crates or the like in close proximity to switches
        // probably unnecessary

        // if no current Interactable is assigned and subscribed, or another got closer to the player
        if (currentInteractable != closestInteractable)
        {
            currentInteractable?.onUnsubscribe();
            // set current Interactable as the interactable in question
            currentInteractable = closestInteractable;
            // Call current Interactable to subscribe to Interaction
            currentInteractable.onSubscribe();
        }
    }
    public void RegisterInteractable(InteractableBase interactable)
    {
        if (interactable != null) registeredInteractables.Add(interactable);
    }
    public void UnregisterInteractable(InteractableBase interactable)
    {
        //If Interactable is destroyed it needs to be taken out of the registry
        // Thinking about obstacles that can be destroyed
        // Currently kinda useless
    }
    // Player can Trigger the InteractAction when/if an Interactable is in range to be interacted with
    public void TriggerInteract()
    {
        // This is called as soon as the player is in interaction range and presses the interaction key
        currentInteractable?.Interact();
    }
}
