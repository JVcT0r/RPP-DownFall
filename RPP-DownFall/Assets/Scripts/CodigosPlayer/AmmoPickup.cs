using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Configuração da Munição")]
    public WeaponType weaponType = WeaponType.Pistol;
    public int ammoAmount = 10;

    private bool collected = false;

    public void OnInteracted()
    {
        if (collected) return;
        collected = true;

        switch (weaponType)
        {
            case WeaponType.Pistol:
                AmmoManager.pistolMagazine = Mathf.Clamp(
                    AmmoManager.pistolMagazine + ammoAmount,
                    0,
                    AmmoManager.pistolMagazineMax
                );
                
                break;

            case WeaponType.Shotgun:
                AmmoManager.shotgunMagazine = Mathf.Clamp(
                    AmmoManager.shotgunMagazine + ammoAmount,
                    0,
                    AmmoManager.shotgunMagazineMax
                );
                
                break;
        }

        Destroy(gameObject);
    }
}