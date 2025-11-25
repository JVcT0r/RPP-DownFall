using UnityEngine;
using TMPro;

public class AmmoManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text ammoText;

    public static int pistolBullets = 0;
    public static int pistolMagazine = 0;

    public static int shotgunBullets = 0;
    public static int shotgunMagazine = 0;

    public static int pistolBulletsMax = 12;
    public static int pistolMagazineMax = 60;

    public static int shotgunBulletsMax = 1;
    public static int shotgunMagazineMax = 20;

    void Update()
    {
        if (WeaponManager.Instance == null || ammoText == null)
            return;

        switch (WeaponManager.Instance.Current)
        {
            case WeaponType.Pistol:
                ammoText.text = $"{pistolBullets}/{pistolMagazine}";
                break;

            case WeaponType.Shotgun:
                ammoText.text = $"{shotgunBullets}/{shotgunMagazine}";
                break;

            default:
                ammoText.text = "-/-";
                break;
        }
    }
}