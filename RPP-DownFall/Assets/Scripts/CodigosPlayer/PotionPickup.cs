using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    [Header("Configuração da Poção")]
    public int potionAmount = 1;

    public void OnInteracted()
    {
        // adiciona ao HealthManager
        HealthManager.potionCount = Mathf.Clamp(
            HealthManager.potionCount + potionAmount,
            0,
            HealthManager.maxPotions
        );

        // atualiza HUD
        HUDManager hud = FindAnyObjectByType<HUDManager>();
        if (hud != null)
            hud.SendMessage("UpdatePotionUI", SendMessageOptions.DontRequireReceiver);

        Debug.Log($"[Pickup] Coletou {potionAmount} poção(ões). Total: {HealthManager.potionCount}");

        // destrói item
        Destroy(gameObject);
    }
}