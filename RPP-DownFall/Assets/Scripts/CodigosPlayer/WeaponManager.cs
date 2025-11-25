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

    private void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();

        // garante que exista GlobalRunData
        if (GlobalRunData.Instance == null)
        {
            GameObject obj = new GameObject("GlobalRunData");
            obj.AddComponent<GlobalRunData>();
        }
        
    }

    private void OnDestroy()
    {
        if (GlobalRunData.Instance != null)
            GlobalRunData.Instance.SaveFromWeaponManager(this);
    }

    private void Update()
    {
        // animação se estiver sem arma nenhuma
        if (!shotgunUnlocked && !pistolUnlocked)
        {
            animator.SetBool("Desarmado", true);
        }
        else animator.SetBool("Desarmado", false);

        // troca para pistola
        if (Input.GetKeyDown(KeyCode.Alpha1) && pistolUnlocked)
        {
            animator.SetBool("Shotgun", false);
            SetWeapon(WeaponType.Pistol);
        }
        // troca para shotgun
        else if (Input.GetKeyDown(KeyCode.Alpha2) && shotgunUnlocked)
        {
            animator.SetBool("Shotgun", true);
            SetWeapon(WeaponType.Shotgun);
        }
    }

    public void UnlockPistol()
    {
        pistolUnlocked = true;
        SetWeapon(WeaponType.Pistol);

        GlobalRunData.Instance.pistolUnlocked = true;
        GlobalRunData.Instance.currentWeapon = WeaponType.Pistol;
        GlobalRunData.Instance.CaptureStatics();
    }

    public void UnlockShotgun()
    {
        shotgunUnlocked = true;
        SetWeapon(WeaponType.Shotgun);

        GlobalRunData.Instance.shotgunUnlocked = true;
        GlobalRunData.Instance.currentWeapon = WeaponType.Shotgun;
        GlobalRunData.Instance.CaptureStatics();
    }

    public void SetWeapon(WeaponType type)
    {
        Current = type;
        OnWeaponChanged?.Invoke(Current);

        if (GlobalRunData.Instance != null)
        {
            GlobalRunData.Instance.currentWeapon = type;
            GlobalRunData.Instance.SaveFromWeaponManager(this);
        }
    }
}
