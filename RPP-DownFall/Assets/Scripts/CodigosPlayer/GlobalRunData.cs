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

    // CHECKPOINT ==========================================================

    private bool cp_pistolUnlocked;
    private bool cp_shotgunUnlocked;

    private WeaponType cp_currentWeapon;

    private int cp_pistolBullets;
    private int cp_pistolMagazine;
    private int cp_shotgunBullets;
    private int cp_shotgunMagazine;

    private int cp_potionCount;

    private bool checkpointSavedThisScene = false;
    private string lastSceneName = "";

    // =====================================================================


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CaptureStatics();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    // CAPTURAR / APLICAR ================================================
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


    // CHECKPOINT SAVE ====================================================
    private void SaveCheckpointState()
    {
        cp_pistolUnlocked = pistolUnlocked;
        cp_shotgunUnlocked = shotgunUnlocked;

        cp_currentWeapon = currentWeapon;

        cp_pistolBullets = pistolBullets;
        cp_pistolMagazine = pistolMagazine;
        cp_shotgunBullets = shotgunBullets;
        cp_shotgunMagazine = shotgunMagazine;

        cp_potionCount = potionCount;

        Debug.Log("CHECKPOINT SALVO.");
    }

    // CHECKPOINT LOAD ====================================================
    public void LoadCheckpointState()
    {
        pistolUnlocked  = cp_pistolUnlocked;
        shotgunUnlocked = cp_shotgunUnlocked;

        if (!pistolUnlocked && !shotgunUnlocked)
            currentWeapon = WeaponType.None;
        else
            currentWeapon = cp_currentWeapon;

        pistolBullets    = cp_pistolBullets;
        pistolMagazine   = cp_pistolMagazine;
        shotgunBullets   = cp_shotgunBullets;
        shotgunMagazine  = cp_shotgunMagazine;

        potionCount = cp_potionCount;

        ApplyStatics();
        SaveRunAsCheckpointState();

        Debug.Log("CHECKPOINT CARREGADO.");
    }

    private void SaveRunAsCheckpointState()
    {
        Instance.pistolUnlocked = cp_pistolUnlocked;
        Instance.shotgunUnlocked = cp_shotgunUnlocked;

        Instance.currentWeapon = cp_currentWeapon;

        Instance.pistolBullets = cp_pistolBullets;
        Instance.pistolMagazine = cp_pistolMagazine;

        Instance.shotgunBullets = cp_shotgunBullets;
        Instance.shotgunMagazine = cp_shotgunMagazine;

        Instance.potionCount = cp_potionCount;
    }


    // APLICAR AO WEAPON MANAGER =========================================
    public void ApplyToWeaponManager(WeaponManager wm)
    {
        if (wm == null) return;

        wm.pistolUnlocked = pistolUnlocked;
        wm.shotgunUnlocked = shotgunUnlocked;

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


    // QUANDO A CENA É CARREGADA =========================================
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        // Reset checkpoint flag se mudou de cena
        if (sceneName != lastSceneName)
        {
            checkpointSavedThisScene = false;
            lastSceneName = sceneName;
        }

        // GAMBIARRAS COM NOMES REAIS DE CENA ============================

        // 🚫 Fase1 sempre sem pistola
        if (sceneName == "Fase1")
        {
            pistolUnlocked = false;
            currentWeapon = WeaponType.None;

            AmmoManager.pistolBullets = 0;
            AmmoManager.pistolMagazine = 0;
        }

        if (sceneName == "Fase2")
        {
            // 🔥 shotgun SEMPRE bloqueada ao iniciar/resetar fase 2
            shotgunUnlocked = false;

            // se estava com ela equipada, tira
            if (currentWeapon == WeaponType.Shotgun)
                currentWeapon = WeaponType.None;

            // resetar a munição da shotgun
            AmmoManager.shotgunBullets = 0;
            AmmoManager.shotgunMagazine = 0;

            
        }

        // 1) salvar checkpoint
        if (!checkpointSavedThisScene)
        {
            SaveCheckpointState();
            checkpointSavedThisScene = true;
        }

        // 2) aplicar estado da RUN
        ApplyStatics();

        // 3) aplicar armas
        WeaponManager wm = FindAnyObjectByType<WeaponManager>();
        ApplyToWeaponManager(wm);
    }


    // RESET TOTAL DA RUN ================================================
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
