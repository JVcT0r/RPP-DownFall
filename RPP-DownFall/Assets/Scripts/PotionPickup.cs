using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    [Header("Configuração da Poção")]
    public int potionAmount = 1; 

    private bool collected = false;

    public void OnInteracted()
    {
        if (collected) return;
        collected = true;

        
        HUDManager hud = FindAnyObjectByType<HUDManager>();
        if (hud != null)
        {
            hud.potionCount = Mathf.Clamp(hud.potionCount + potionAmount, 0, 9); 
            hud.SendMessage("UpdatePotionUI", SendMessageOptions.DontRequireReceiver);
        }

        Debug.Log($"[Pickup] Coletou {potionAmount} poção(ões). Total: {hud?.potionCount}");
        Destroy(gameObject);
    }
}