using UnityEngine;
using TMPro;

public class AmmoManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text ammoText;

    // 🔹 Armas e munições
    public static int pistolBullets = 12;
    public static int pistolMagazine = 60;

    public static int shotgunBullets = 6;
    public static int shotgunMagazine = 30;

    // 🔹 Valores máximos
    public static int pistolBulletsMax = 12;
    public static int pistolMagazineMax = 60;

    public static int shotgunBulletsMax = 6;
    public static int shotgunMagazineMax = 30;

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