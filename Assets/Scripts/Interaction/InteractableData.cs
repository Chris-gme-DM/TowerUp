using Unity.VisualScripting;
using UnityEngine;

// Concenpt of the screen has fallen flat so this is not in use
// 


[CreateAssetMenu(fileName = "InteractableData", menuName = "Scriptable Objects/InteractableData")]
public class InteractableData : ScriptableObject, IInteractable
{
    [Header("Objectdata")]
    [SerializeField] string interactableName;
    [SerializeField] GameObject interactablePrefab;
    // Scalable Interaction Range to determine Interactibility
    [Range(0f, 5f)] float interactionRange;

    public void Interact()
    { 
        Debug.Log("Do stuff");
    }
    public void Subscribe() 
    {
        // When the player hits the raycast requirements of the given Interactable it subscribes itself to to the interaction Manager that can call upon its Interaction
        // The UI displays Interactibility
    }
    public void Unsubscribe()
    { 
        // when the player leaves the interaction range the UI prompt for interaction should disappear
        // Disable Interactability
    }
    // The range for the interactions need to be set for each type of interactable
    public bool InInteractionRange(Vector3 playerPosition)
    {
        Vector3 interactablePosition = interactablePrefab.transform.position;
        return Vector3.Distance(playerPosition, interactablePosition) <= interactionRange;
    }
}
public struct Interactable
{
    InteractableType type;
    public Interactable(InteractableType type) { this.type = type; }
}
public enum InteractableType
{
    none,
    interactableObject,
    NPC,
    item,
}