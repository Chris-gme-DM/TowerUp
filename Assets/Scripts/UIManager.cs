using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Assign these, please
    [Header("References")]
    [SerializeField]InteractionManager interactionManager;
    [SerializeField]PlayerController playerController;
    [SerializeField] GameObject creditsUI;
    [SerializeField] Canvas canvas;

    public int credits;
    public float uiDisplayDuration;
    private Coroutine deactivateUI;
    private TextMeshProUGUI creditValueText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactionManager = FindAnyObjectByType<InteractionManager>();
        playerController = FindAnyObjectByType<PlayerController>();
        // Set credits UI in inspector
        creditValueText = creditsUI.GetComponentInChildren<TextMeshProUGUI>();
        creditsUI.SetActive(false);
        UpdateCreditUI();
    }
    private void ToggleUI()
    {
        // Press Esc to show entire MenuUI
        // Turn on the entire Canvas Panels to show the player information
    }
    private void ActivateCreditUI()
    {
        // Only show the credits UI for a short time, nobody cares about money
        if (deactivateUI != null)
        {
            StopCoroutine(deactivateUI);
        }
        creditsUI.SetActive(true);
        deactivateUI = StartCoroutine(DeactivateUI() );
    }
    IEnumerator DeactivateUI()
    {
        yield return new WaitForSeconds(uiDisplayDuration);
        creditsUI.SetActive(false);
        deactivateUI=null;
    }
    public void AddCredits(int value)
    {
        credits += value;
        UpdateCreditUI();
        ActivateCreditUI();
    }
    private void UpdateCreditUI()
    {
        creditValueText.text = "Credits: " + credits.ToString();
    }

}
