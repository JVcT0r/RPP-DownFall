using UnityEngine;

public class PistolPickup : MonoBehaviour
{
    public void OnInteracted()
    {
        WeaponManager.Instance.pistolUnlocked = true;
        WeaponManager.Instance.SetWeapon(WeaponType.Pistol);
        Debug.Log("[Pickup] Pistola coletada!");
        Destroy(gameObject);
    }
}