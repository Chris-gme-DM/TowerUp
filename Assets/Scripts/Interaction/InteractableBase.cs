using UnityEngine;
using UnityEngine.InputSystem;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Objectdata")]
    [SerializeField] string interactableName;
    public abstract void Interact();
}