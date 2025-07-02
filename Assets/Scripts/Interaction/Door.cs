using UnityEngine;

public class Door : InteractableData
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        base.Interact();
        // Do the stuff
        // Plays animation if any exist
    }
    public override void OnFocus()
    {
        base.OnFocus();
        // Detect player
        // DisplayInteractability
    }
    public override void OnLoseFocus()
    {
        base.OnLoseFocus();
        // Don't display Interactability
    }
}
