using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class MenuButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text text;
    public Color normalColor = Color.white;
    public Color highlightColor = new Color(1, 0.5f, 0f); 

    void Start()
    {
        if (text == null)
            text = GetComponentInChildren<TMP_Text>();

        text.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = normalColor;
    }
}