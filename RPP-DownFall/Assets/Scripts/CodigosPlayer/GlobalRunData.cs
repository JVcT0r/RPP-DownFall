using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalRunData : MonoBehaviour
{
    public static GlobalRunData Instance { get; private set; }

    [Header("Armas desbloqueadas")]
    public bool pistolUnlocked;
    public bool shotgunUnlocked;

    [Header("Arma atual")]
    public WeaponType currentWeapon = WeaponType.None;

    [Header("Munições")]
    public int pistolBullets;
    public int pistolMagazine;
    public int shotgunBullets;
    public int shotgunMagazine;

    [Header("Poções")]
    public int potionCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // pega os valores iniciais das statics
        CaptureStatics();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void CaptureStatics()
    {
        pistolBullets   = AmmoManager.pistolBullets;
        pistolMagazine  = AmmoManager.pistolMagazine;
        shotgunBullets  = AmmoManager.shotgunBullets;
        shotgunMagazine = AmmoManager.shotgunMagazine;

        potionCount = HealthManager.potionCount;
    }

    public void ApplyStatics()
    {
        AmmoManager.pistolBullets   = pistolBullets;
        AmmoManager.pistolMagazine  = pistolMagazine;
        AmmoManager.shotgunBullets  = shotgunBullets;
        AmmoManager.shotgunMagazine = shotgunMagazine;

        HealthManager.potionCount = potionCount;
    }

    public void ApplyToWeaponManager(WeaponManager wm)
    {
        if (wm == null) return;

        wm.pistolUnlocked = pistolUnlocked;
        wm.shotgunUnlocked = shotgunUnlocked;

        // força arma atual válida
        if (currentWeapon == WeaponType.Pistol && pistolUnlocked)
            wm.SetWeapon(WeaponType.Pistol);
        else if (currentWeapon == WeaponType.Shotgun && shotgunUnlocked)
            wm.SetWeapon(WeaponType.Shotgun);
        else if (pistolUnlocked)
            wm.SetWeapon(WeaponType.Pistol);
        else if (shotgunUnlocked)
            wm.SetWeapon(WeaponType.Shotgun);
        else
            wm.SetWeapon(WeaponType.None);
    }

    public void SaveFromWeaponManager(WeaponManager wm)
    {
        if (wm == null) return;

        pistolUnlocked = wm.pistolUnlocked;
        shotgunUnlocked = wm.shotgunUnlocked;
        currentWeapon = wm.Current;

        CaptureStatics();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // aplica municoes/poções na scene nova
        ApplyStatics();

        // aplica armas liberadas na scene nova
        WeaponManager wm = FindAnyObjectByType<WeaponManager>();
        ApplyToWeaponManager(wm);
    }

    // opcional: limpar tudo quando começar novo jogo
    public void ResetRun()
    {
        pistolUnlocked = false;
        shotgunUnlocked = false;
        currentWeapon = WeaponType.None;

        pistolBullets = 12;
        pistolMagazine = 60;
        shotgunBullets = 6;
        shotgunMagazine = 30;

        potionCount = 0;

        ApplyStatics();
    }
}
