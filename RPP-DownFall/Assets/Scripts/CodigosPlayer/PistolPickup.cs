using UnityEngine;

public class PistolPickup : MonoBehaviour
{
    public void OnInteracted()
    {
        WeaponManager.Instance.UnlockPistol();
        Debug.Log("[Pickup] Pistola coletada!");
        Destroy(gameObject);
    }
}