using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractableData", menuName = "Scriptable Objects/InteractableData")]
public class InteractableData : ScriptableObject, IInteractable
{
    [Header("Objectdata")]
    [SerializeField] string interactableName;
    [SerializeField] GameObject interactablePrefab;
    [Range(0f, 5f)] float interactionRange;

    public void Interact()
    { 
        Debug.Log("Do stuff");
    }
    public  void OnFocus()
    { 
        Debug.Log("Now you can do stuff");
        // Enable Interactability
    }
    public void Subscribe() { }
    public void OnLoseFocus()
    {
        Debug.Log("Now you can't"); 
    }
    public void Unsubscribe()
    { 
        // Disable Interactability
    }
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