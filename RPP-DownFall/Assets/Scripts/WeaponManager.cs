using UnityEngine;
using System;

public enum WeaponType { Pistol, Shotgun, Slot3, Slot4 }

public class WeaponManager : MonoBehaviour
{
    public static event Action<WeaponType> OnWeaponChanged;

    public WeaponType Current { get; private set; } = WeaponType.Pistol;
    
    private int pistolBullets = 12;
    private int pistolMagazine = 60;

    private int shotgunBullets = 8;
    private int shotgunMagazine = 32;

    private int slot3Bullets = 30;
    private int slot3Magazine = 90;

    private int slot4Bullets = 5;
    private int slot4Magazine = 15;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetWeapon(WeaponType.Pistol);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SetWeapon(WeaponType.Shotgun);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SetWeapon(WeaponType.Slot3);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            SetWeapon(WeaponType.Slot4);
    }

    public void SetWeapon(WeaponType type)
    {
        if (type == Current) return;
        
        SaveCurrentAmmo();
        
        Current = type;
        
        switch (Current)
        {
            case WeaponType.Pistol:
                AmmoManager.BulletsMax = 12;
                AmmoManager.MagazineMax = 60;
                AmmoManager.Bullets = pistolBullets;
                AmmoManager.Magazine = pistolMagazine;
                break;

            case WeaponType.Shotgun:
                AmmoManager.BulletsMax = 8;
                AmmoManager.MagazineMax = 32;
                AmmoManager.Bullets = shotgunBullets;
                AmmoManager.Magazine = shotgunMagazine;
                break;

            case WeaponType.Slot3:
                AmmoManager.BulletsMax = 30;
                AmmoManager.MagazineMax = 90;
                AmmoManager.Bullets = slot3Bullets;
                AmmoManager.Magazine = slot3Magazine;
                break;

            case WeaponType.Slot4:
                AmmoManager.BulletsMax = 5;
                AmmoManager.MagazineMax = 15;
                AmmoManager.Bullets = slot4Bullets;
                AmmoManager.Magazine = slot4Magazine;
                break;
        }

        OnWeaponChanged?.Invoke(Current);
    }

    private void SaveCurrentAmmo()
    {
        switch (Current)
        {
            case WeaponType.Pistol:
                pistolBullets = AmmoManager.Bullets;
                pistolMagazine = AmmoManager.Magazine;
                break;

            case WeaponType.Shotgun:
                shotgunBullets = AmmoManager.Bullets;
                shotgunMagazine = AmmoManager.Magazine;
                break;

            case WeaponType.Slot3:
                slot3Bullets = AmmoManager.Bullets;
                slot3Magazine = AmmoManager.Magazine;
                break;

            case WeaponType.Slot4:
                slot4Bullets = AmmoManager.Bullets;
                slot4Magazine = AmmoManager.Magazine;
                break;
        }
    }
}
