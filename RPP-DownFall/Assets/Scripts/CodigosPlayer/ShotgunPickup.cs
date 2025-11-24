using UnityEngine;

public class ShotgunPickup : MonoBehaviour
{
    public void OnInteracted()
    {
        WeaponManager.Instance.UnlockShotgun();
        Debug.Log("[Pickup] Shotgun coletada!");
        Destroy(gameObject);
    }
}