using UnityEngine;

public class CursorController
{
    void Start()
    {
        // Hide the cursor at the start of the game
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void EnableCursor()
    { 
        // Cursor needs to be visible in UI Interactins like PauseMenu, if i make one that is
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void DisableCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}