using System.Collections.Specialized;
using UnityEngine;

public class GeneratorSwitch : InteractableBase
{
    [Header("References")]
    // Reference to the door it controls the Powersurge to, until better system needs to be referenced manually
    public GameObject ThisDoor;
    private Animator animator;
    private bool switchON;
    private Door thisDoor;
    //Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switchON = false;
        //Get own components online
        animator = GetComponent<Animator>();
        thisDoor = ThisDoor.GetComponent<Door>();

        animator.SetBool("switchON", switchON);

    }
    public override void Interact()
    {
        // Do the stuff
        thisDoor.OnPowerSwitchInteract();
        switchON = !switchON;
        // Play Animation if any
        animator.SetBool("switchON", switchON);
    }
}
