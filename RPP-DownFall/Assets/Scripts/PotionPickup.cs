using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    [Header("Configuração da Poção")]
    public int potionAmount = 1; 

    public void OnInteracted()
    {
        // pega o HUD
        HUDManager hud = FindAnyObjectByType<HUDManager>();
        if (hud != null)
        {
            hud.potionCount = Mathf.Clamp(hud.potionCount + potionAmount, 0, 9); 
            hud.SendMessage("UpdatePotionUI", SendMessageOptions.DontRequireReceiver);

            Debug.Log($"[Pickup] Coletou {potionAmount} poção(ões). Total: {hud.potionCount}");
        }

        
        Destroy(gameObject);
    }
}