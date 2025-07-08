using UnityEngine;

public class CreditChip : InteractableBase, IInteractable
{
    private UIManager ui;
    private Vector3 rotationSpeed = new Vector3(0, 90, 0);
    private Space rotationSpace = Space.Self;
    public float destroyDelay = 0.5f;
    private bool collected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collected = false;
        ui = FindAnyObjectByType<UIManager>();
    }

    void Update()
    {
        // Rotate the creditChip
        transform.Rotate(rotationSpeed * Time.deltaTime, rotationSpace);
    }
    public override void Interact()
    {
        ui.AddCredits(100);
        collected = true;
        // Disable visiblity and collision, even before it is properly destroyed, maybe i find sound effects
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Destroy this GameObject after a short delay
        Destroy(gameObject, destroyDelay);
    }
}
