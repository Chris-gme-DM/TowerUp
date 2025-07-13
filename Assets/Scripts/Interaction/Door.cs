using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public class Door : InteractableBase
{
    [Header("References")]
    Animator animator;
    [SerializeField] private MeshRenderer lightMeshRenderer;
    [SerializeField] private Material defaultLight;
    [SerializeField] private Material greenLight;
    [SerializeField] private int priceTag;
    private UIManager manager;
    // this script controls the doors opening if the player interacts with the panels attached to it
    // The door will need a reference to the panels that control it
    // need the animation controller
    // So the panels need to charge some money from the player
    // Doors need energy enabled before any function can be provided
    private bool isOpen;
    private bool powerEnabled;
    private bool isUnlocked;
    //Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isOpen = false;
        powerEnabled = false;

        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("powerEnabled" , powerEnabled);
        }

        if (lightMeshRenderer.sharedMaterials.Length > 0)
        {
            defaultLight = lightMeshRenderer.sharedMaterials[0];
        }

        manager = FindAnyObjectByType<UIManager>();
        isUnlocked = false;
    }
    public override void Interact()
    {
        // Only opens if the player collected enough credits to pay the price
        // I should make the price Tag visible somehow
        if (isUnlocked)
        {
            animator.SetBool("isOpen", true);
            // Close door automatically
            Invoke(nameof(CloseDoor), 10f);
        }
        else if (powerEnabled && (manager.credits >= priceTag) && !isUnlocked)
        {
            isUnlocked= true;
            //Take credit amount from player
            manager.AddCredits(-priceTag);
            isOpen = !isOpen;
            //Open the door
            // Plays animation
            animator.SetBool("isOpen", true);
            // Close door automatically after 10s
            Invoke(nameof(CloseDoor), 10f);
        }
        else if(powerEnabled && (manager.credits < priceTag) && !isUnlocked)
        {
            manager.InsufficentCredit();
        }
    }
    private void CloseDoor()
    {
        animator.SetBool("isOpen", false);
    }
    // Do i even need that? Interfaces
    public void OnPowerSwitchInteract()
    {
        // If the connected powerSwitch of the Generator is used it switches the state of the door
        powerEnabled = !powerEnabled;
        if (powerEnabled)
            SetLightMaterial(greenLight, 0);
        if (!powerEnabled)
            SetLightMaterial(defaultLight, 0 );
        // Change the Lights on the door and power up the Panel
        animator.SetBool("powerEnabled", powerEnabled);

        //Force door closed if i missed a fringe case, or i can make NPCs that turn off the power
        if (!powerEnabled && isOpen)
        {
            isOpen = false;
            animator.SetBool("isOpen", isOpen);
        }
    }
    private void SetLightMaterial(Material materialToApply, int materialIndex = 0)
    {
        Material[] currentMaterials = lightMeshRenderer.materials;
        currentMaterials[materialIndex] = materialToApply;
        lightMeshRenderer.materials = currentMaterials;

    }
}
