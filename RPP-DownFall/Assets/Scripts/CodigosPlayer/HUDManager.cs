using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Ammo UI (Círculo)")]
    public Image ammoCircle;
    public TMP_Text ammoText;
    

    [Header("Ícone de Arma")]
    public Image weaponIcon;
    public Sprite pistolSprite;
    public Sprite shotgunSprite;
    //public Sprite slot3Sprite;
    //public Sprite slot4Sprite;

    [Header("Poção / Cura")]
    public Image potionIcon;
    public TMP_Text potionCountText;

    void OnEnable()
    {
        WeaponManager.OnWeaponChanged += HandleWeaponChanged;
    }

    void OnDisable()
    {
        WeaponManager.OnWeaponChanged -= HandleWeaponChanged;
    }

    void Start()
    {
        UpdateAmmoUI();
        HandleWeaponChanged(WeaponType.Pistol);
        UpdatePotionUI();
    }

    void Update()
    {
        UpdateAmmoUI();

        
        if (Input.GetKeyDown(KeyCode.H))
            UsePotion();
    }

    // -------------------- ATUALIZAÇÃO DE MUNIÇÃO --------------------
    void UpdateAmmoUI()
    {
        if (WeaponManager.Instance == null) return;

        switch (WeaponManager.Instance.Current)
        {
            case WeaponType.Pistol:
                if (ammoCircle != null)
                {
                    float fill = AmmoManager.pistolBulletsMax > 0
                        ? (float)AmmoManager.pistolBullets / AmmoManager.pistolBulletsMax
                        : 0f;
                    ammoCircle.fillAmount = Mathf.Clamp01(fill);
                }

                if (ammoText != null)
                    ammoText.text = $"{AmmoManager.pistolBullets}/{AmmoManager.pistolMagazine}";
                break;

            case WeaponType.Shotgun:
                if (ammoCircle != null)
                {
                    float fill = AmmoManager.shotgunBulletsMax > 0
                        ? (float)AmmoManager.shotgunBullets / AmmoManager.shotgunBulletsMax
                        : 0f;
                    ammoCircle.fillAmount = Mathf.Clamp01(fill);
                }

                if (ammoText != null)
                    ammoText.text = $"{AmmoManager.shotgunBullets}/{AmmoManager.shotgunMagazine}";
                break;
        }
    }

    // ------------------- ATUALIZAÇÃO DAS POÇÕES -------------------
    void UpdatePotionUI()
    {
        if (potionCountText != null)
            potionCountText.text = HealthManager.potionCount.ToString();
    }

    // ------------------- USAR POÇÃO -------------------
    public void UsePotion()
    {
        var player = FindAnyObjectByType<Player>();

        
        if (HealthManager.potionCount <= 0)
        {
            Debug.Log("[HUD] Sem poções restantes!");
            return;
        }

       
        if (player.CurrentHealth >= player.maxHealth)
        {
            Debug.Log("[HUD] Vida já está cheia!");
            return;
        }

        
        HealthManager.potionCount--;
        UpdatePotionUI();

        
        if (player != null)
            player.Heal();

        Debug.Log("[HUD] Poção usada!");
    }

    // ------------------- TROCA DE ARMA (HUD) -------------------
    void HandleWeaponChanged(WeaponType wt)
    {
        if (weaponIcon != null)
        {
            switch (wt)
            {
                case WeaponType.Pistol:
                    weaponIcon.sprite = pistolSprite;
                    break;

                case WeaponType.Shotgun:
                    weaponIcon.sprite = shotgunSprite;
                    break;
            }

            weaponIcon.enabled = weaponIcon.sprite != null;
        }
        
    }
}
