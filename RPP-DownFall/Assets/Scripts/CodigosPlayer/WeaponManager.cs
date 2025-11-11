using UnityEngine;
using System;

public enum WeaponType { Pistol, Shotgun }

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }
    public static event Action<WeaponType> OnWeaponChanged;
    public Animator animator;

    public WeaponType Current { get; private set; } = WeaponType.Pistol;

    void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.SetBool("Shotgun", false);
            SetWeapon(WeaponType.Pistol);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.SetBool("Shotgun", true);
            SetWeapon(WeaponType.Shotgun);
        }
    }

    public void SetWeapon(WeaponType type)
    {
        if (type == Current) return;

        Current = type;
        OnWeaponChanged?.Invoke(Current);
        
    }
}