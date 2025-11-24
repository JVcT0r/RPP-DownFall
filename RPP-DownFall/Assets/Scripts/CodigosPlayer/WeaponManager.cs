using UnityEngine;
using System;

public enum WeaponType { None, Pistol, Shotgun }

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }
    public static event Action<WeaponType> OnWeaponChanged;

    private Animator animator;

    public WeaponType Current { get; private set; } = WeaponType.None;

    [Header("Armas liberadas")]
    public bool pistolUnlocked = false;
    public bool shotgunUnlocked = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetBool("Desarmado", !pistolUnlocked && !shotgunUnlocked);

        if (Input.GetKeyDown(KeyCode.Alpha1) && pistolUnlocked)
        {
            animator.SetBool("Shotgun", false);
            SetWeapon(WeaponType.Pistol);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && shotgunUnlocked)
        {
            animator.SetBool("Shotgun", true);
            SetWeapon(WeaponType.Shotgun);
        }
    }

    public void SetWeapon(WeaponType type)
    {
        Current = type;
        OnWeaponChanged?.Invoke(Current);
    }
}