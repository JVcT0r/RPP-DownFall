using UnityEngine;
using System;

public enum WeaponType { Pistol, Shotgun }

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }
    public static event Action<WeaponType> OnWeaponChanged;

    public WeaponType Current { get; private set; } = WeaponType.Pistol;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetWeapon(WeaponType.Pistol);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SetWeapon(WeaponType.Shotgun);
    }

    public void SetWeapon(WeaponType type)
    {
        if (type == Current) return;

        Current = type;
        OnWeaponChanged?.Invoke(Current);
        
    }
}