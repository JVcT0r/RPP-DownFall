using UnityEngine;

public class InteractablePopup : MonoBehaviour
{
    [Header("UI")]
    public GameObject popup;

    private float hideTimer = 0f;
    private float hideDelay = 2f;  

    private bool isVisible = false;

    void Awake()
    {
        if (popup != null)
            popup.SetActive(false);
    }

    void Update()
    {
        if (isVisible)
        {
            hideTimer += Time.deltaTime;

            if (hideTimer >= hideDelay)
            {
                popup.SetActive(false);
                isVisible = false;
            }
        }
    }

    public void ShowPopup(bool show)
    {
        if (popup == null) return;

        if (show)
        {
            popup.SetActive(true);
            hideTimer = 0f;
            isVisible = true;
        }
    }
}