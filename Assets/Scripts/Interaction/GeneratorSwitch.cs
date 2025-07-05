using System.Collections.Specialized;
using UnityEngine;

public class GeneratorSwitch : InteractableBase
{
    [Header("References")]
    // Reference to the door it controls the Powersurge to, until better system needs to be referenced manually
    public Door ThisDoor;
    private Animator animator;
    private bool switchON;

    //Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get own components online
        switchON = false; 
        animator = GetComponent<Animator>();
    }
    public override void Interact()
    {
        // Do the stuff
        ThisDoor.OnPowerSwitchInteract();
        switchON = !switchON;
        // Play Animation if any
    }
    public override void onSubscribe()
    {
        base.onSubscribe();
        // Subscribe to Interaction
    }
    public override void onUnsubscribe()
    {
        base.onUnsubscribe();
        // Unsubscribe from Interaction
    }
}
