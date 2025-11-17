using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public void OnInteracted()
    {
        Player player = FindAnyObjectByType<Player>();
        if (player == null) return;

        player.temChave = true;
        Debug.Log("[KEY] Chave coletada!");

        Destroy(gameObject);
    }
}