using UnityEngine;
using TMPro;

public class HealthManager : MonoBehaviour
{
    public static int potionCount = 0; 
    public static int maxPotions = 9;

    [Header("UI")]
    public TMP_Text potionText;

    void Update()
    {
        if (potionText != null)
            potionText.text = potionCount.ToString();
    }

    public static bool TryUsePotion()
    {
        if (potionCount <= 0) return false;

        potionCount--;
        return true;
    }
}