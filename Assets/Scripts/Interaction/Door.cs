using UnityEngine;
using System.Collections.Generic;

public class Door : InteractableBase
{
    [Header("References")]
    Animator animator;
    // this script controls the doors opening if the player interacts with the panels attached to it
    // The door will need a reference to the panels that control it
    // need the animation controller
    // needs reference to which genereator switch enables the door to function
    // So the panels need to charge some money from the player
    // Doors need energy enabled before any function can be provided
    private bool isOpen;
    private bool powerEnabled;

    //Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public override void Interact()
    {
        // Do the stuff
        //Take credit amount from player
        //Open the door

        // Plays animation
    }
    // Do i even need that? Interfaces
    public override void onSubscribe()
    { base.onSubscribe(); }
    public override void onUnsubscribe()
    { base.onUnsubscribe(); }
    private void OnTriggerExit(Collider col)
    {
        // When the player leaves the area of the door it closes automatically if it is open, leaving it to the player to use the door at their own leizure
        if (isOpen)
        { 
            isOpen = false;
            // Play animation to close the door
        }
    }
    public void OnPowerSwitchInteract()
    {
        // If the connected powerSwitch of the Generator is used it switches the state of the door
        powerEnabled = !powerEnabled;
        // Change the Lights on the door and power up the Panel
    }
}
