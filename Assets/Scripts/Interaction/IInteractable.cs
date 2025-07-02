using UnityEngine;

public interface IInteractable
{
    void Interact();
    void Subscribe();
    void Unsubscribe();
    bool InInteractionRange(Vector3 playerPosition);
}
