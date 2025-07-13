// Scripts/UI/UIPanelController.cs
using UnityEngine;

public class UIPanelController : MonoBehaviour
{
    [Header("UI Panel Settings")]
    [SerializeField] private GameObject panelToControl; // Assign the UI Panel GameObject here
    [SerializeField] private bool startVisible = false; // Whether the panel should be visible on start
    [SerializeField] private bool useAnimation = false; // Whether to use animation for show/hide
    [SerializeField] private float animationDuration = 0.3f; // Duration of show/hide animation

    // Animation components
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Start()
    {
        // Get animation components if they exist
        if (panelToControl != null)
        {
            canvasGroup = panelToControl.GetComponent<CanvasGroup>();
            rectTransform = panelToControl.GetComponent<RectTransform>();
        }

        // Set initial visibility
        if (panelToControl != null)
        {
            panelToControl.SetActive(startVisible);
        }
    }

    // Method to show the panel.
    public void ShowPanel()
    {
        if (panelToControl != null)
        {
            if (useAnimation && canvasGroup != null)
            {
                StartCoroutine(AnimateShow());
            }
            else
            {
                panelToControl.SetActive(true); // Activates the GameObject, making it visible [42, 43]
                Debug.Log($"{panelToControl.name} is now visible.");
            }
        }
    }

    // Method to hide the panel.
    public void HidePanel()
    {
        if (panelToControl != null)
        {
            if (useAnimation && canvasGroup != null)
            {
                StartCoroutine(AnimateHide());
            }
            else
            {
                panelToControl.SetActive(false); // Deactivates the GameObject, hiding it [42, 43]
                Debug.Log($"{panelToControl.name} is now hidden.");
            }
        }
    }

    // Optional: Toggle panel visibility.
    public void TogglePanel()
    {
        if (panelToControl != null)
        {
            if (panelToControl.activeSelf)
            {
                HidePanel();
            }
            else
            {
                ShowPanel();
            }
        }
    }

    // Animate panel showing
    private System.Collections.IEnumerator AnimateShow()
    {
        panelToControl.SetActive(true);
        canvasGroup.alpha = 0f;
        
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / animationDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        Debug.Log($"{panelToControl.name} is now visible (animated).");
    }

    // Animate panel hiding
    private System.Collections.IEnumerator AnimateHide()
    {
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / animationDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        panelToControl.SetActive(false);
        Debug.Log($"{panelToControl.name} is now hidden (animated).");
    }

    // Method to set panel visibility directly
    public void SetPanelVisible(bool visible)
    {
        if (panelToControl != null)
        {
            panelToControl.SetActive(visible);
        }
    }

    // Method to check if panel is currently visible
    public bool IsPanelVisible()
    {
        return panelToControl != null && panelToControl.activeSelf;
    }

    // Method to set the panel to control (useful for dynamic assignment)
    public void SetPanelToControl(GameObject panel)
    {
        panelToControl = panel;
        if (panel != null)
        {
            canvasGroup = panel.GetComponent<CanvasGroup>();
            rectTransform = panel.GetComponent<RectTransform>();
        }
    }
} 