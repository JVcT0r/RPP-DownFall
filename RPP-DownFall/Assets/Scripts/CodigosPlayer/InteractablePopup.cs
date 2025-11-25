using UnityEngine;

public class InteractablePopup : MonoBehaviour
{
    [Header("UI")]
    public GameObject popup; 

    public void ShowPopup(bool show)
    {
        if (popup != null)
            popup.SetActive(show);
    }
}