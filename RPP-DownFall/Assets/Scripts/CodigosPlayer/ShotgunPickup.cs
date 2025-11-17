using UnityEngine;

public class ShotgunPickup : MonoBehaviour
{
    public void OnInteracted()
    {
        WeaponManager.Instance.shotgunUnlocked = true;
        WeaponManager.Instance.SetWeapon(WeaponType.Shotgun);
        Debug.Log("[Pickup] Shotgun coletada!");
        Destroy(gameObject);
    }
}