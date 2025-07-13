using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
// Refactored and modified this script with Gemini, too tired
public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] InteractionManager interactionManager;
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject creditsUI;
    [SerializeField] GameObject InsufficientCreditsUI;
    [SerializeField] Canvas canvas; // Typically the root canvas

    public int credits;
    public float uiDisplayDuration = 3f; // Default duration for timed UI panels

    // Example: Add a persistent UI panel reference here (e.g., for a pause menu)
    [SerializeField] GameObject pauseMenuPanel; // Assign this in the Inspector!

    private TextMeshProUGUI creditValueText;

    // A dictionary to keep track of active deactivation coroutines for each UI panel.
    // This allows multiple panels to have independent timers.
    private Dictionary<GameObject, Coroutine> activeUIDeactivationCoroutines = new Dictionary<GameObject, Coroutine>();

    void Start()
    {
        interactionManager = FindAnyObjectByType<InteractionManager>();
        playerController = FindAnyObjectByType<PlayerController>();

        creditValueText = creditsUI.GetComponentInChildren<TextMeshProUGUI>();

        // Ensure all UI panels start inactive
        creditsUI.SetActive(false);
        InsufficientCreditsUI.SetActive(false);
        if (pauseMenuPanel != null) // Ensure your new panel is also initially off
        {
            pauseMenuPanel.SetActive(false);
        }

        UpdateCreditUI();
    }

    // --- Core UI Panel Control Methods ---

    /// <summary>
    /// Activates a UI panel and ensures it stays visible indefinitely.
    /// Clears any ongoing deactivation timers for this panel.
    /// </summary>
    /// <param name="uiPanel">The GameObject representing the UI panel to show.</param>
    public void ShowUIPanel(GameObject uiPanel)
    {
        if (uiPanel == null)
        {
            Debug.LogWarning("UIManager: Attempted to show a null UI Panel.", this);
            return;
        }

        // Stop any active deactivation coroutine for this panel
        if (activeUIDeactivationCoroutines.ContainsKey(uiPanel) && activeUIDeactivationCoroutines[uiPanel] != null)
        {
            StopCoroutine(activeUIDeactivationCoroutines[uiPanel]);
            activeUIDeactivationCoroutines.Remove(uiPanel);
        }

        uiPanel.SetActive(true); // Activate the panel
    }

    /// <summary>
    /// Deactivates a UI panel immediately.
    /// Stops any ongoing deactivation timers for this panel.
    /// </summary>
    /// <param name="uiPanel">The GameObject representing the UI panel to hide.</param>
    public void HideUIPanel(GameObject uiPanel)
    {
        if (uiPanel == null)
        {
            Debug.LogWarning("UIManager: Attempted to hide a null UI Panel.", this);
            return;
        }

        // Stop any active deactivation coroutine for this panel
        if (activeUIDeactivationCoroutines.ContainsKey(uiPanel) && activeUIDeactivationCoroutines[uiPanel] != null)
        {
            StopCoroutine(activeUIDeactivationCoroutines[uiPanel]);
            activeUIDeactivationCoroutines.Remove(uiPanel);
        }

        uiPanel.SetActive(false); // Deactivate the panel
    }

    /// <summary>
    /// Toggles the active state of a UI panel (shows if hidden, hides if shown).
    /// Panels shown with this method will stay active indefinitely until toggled off again.
    /// </summary>
    /// <param name="uiPanel">The GameObject representing the UI panel to toggle.</param>
    public void ToggleUIPanel(GameObject uiPanel)
    {
        if (uiPanel == null)
        {
            Debug.LogWarning("UIManager: Attempted to toggle a null UI Panel.", this);
            return;
        }

        if (uiPanel.activeSelf)
        {
            HideUIPanel(uiPanel); // If active, hide it
        }
        else
        {
            ShowUIPanel(uiPanel); // If inactive, show it indefinitely
        }
    }

    /// <summary>
    /// Activates a UI panel for a specified duration.
    /// Stops any existing deactivation timer for that panel before reactivating.
    /// Suitable for temporary notifications.
    /// </summary>
    /// <param name="uiPanel">The GameObject representing the UI panel to activate.</param>
    /// <param name="duration">Optional: The duration (in seconds) for which the panel should be active. Uses uiDisplayDuration if null.</param>
    public void ShowTimedUIPanel(GameObject uiPanel, float? duration = null)
    {
        if (uiPanel == null)
        {
            Debug.LogWarning("UIManager: Attempted to show a timed null UI Panel.", this);
            return;
        }

        // Stop any active deactivation coroutine for this panel
        if (activeUIDeactivationCoroutines.ContainsKey(uiPanel) && activeUIDeactivationCoroutines[uiPanel] != null)
        {
            StopCoroutine(activeUIDeactivationCoroutines[uiPanel]);
            activeUIDeactivationCoroutines.Remove(uiPanel);
        }

        uiPanel.SetActive(true); // Activate the specified UI panel

        // Determine the actual display duration
        float displayDuration = duration ?? uiDisplayDuration; // Use provided duration or the default

        // Start a new deactivation coroutine specifically for this panel
        Coroutine newDeactivateCoroutine = StartCoroutine(DeactivateUIPanelAfterDelay(uiPanel, displayDuration));
        activeUIDeactivationCoroutines.Add(uiPanel, newDeactivateCoroutine); // Store the reference
    }

    /// <summary>
    /// Coroutine to deactivate a specific UI panel after a delay.
    /// </summary>
    private IEnumerator DeactivateUIPanelAfterDelay(GameObject uiPanel, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
        // Remove the coroutine reference from the dictionary after it finishes its job
        activeUIDeactivationCoroutines.Remove(uiPanel);
    }


    // --- Existing methods, now using ShowTimedUIPanel for notifications ---

    public void AddCredits(int value)
    {
        credits += value; //
        UpdateCreditUI(); //
        ShowTimedUIPanel(creditsUI); // Use the new timed method for the credit notification
    }

    public void InsufficentCredit()
    {
        ShowTimedUIPanel(InsufficientCreditsUI); // Use the new timed method for the insufficient credit notification
    }

    private void UpdateCreditUI()
    {
        creditValueText.text = "Credits: " + credits.ToString(); //
    }

}