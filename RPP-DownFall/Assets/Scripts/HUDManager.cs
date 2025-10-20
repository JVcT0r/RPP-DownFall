using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Ammo UI (Círculo)")]
    public Image ammoCircle;
    public TMP_Text ammoText;

    [Header("Weapon Slot (Círculo Claro)")]
    public Image weaponSlotRing;
    public Image weaponSelectionGlow;
    [Range(0f, 1f)] public float selectionPulseSpeed = 6f;

    [Header("Ícone de Arma")]
    public Image weaponIcon;
    public Sprite pistolSprite;
    public Sprite shotgunSprite;
    public Sprite slot3Sprite;
    public Sprite slot4Sprite;

    [Header("Poção / Cura")]
    public Image potionIcon;
    public TMP_Text potionCountText;
    public int potionCount = 3; // 🔹 Quantidade inicial de poções

    private float pulseT;
    private bool doPulse;

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

        if (weaponSelectionGlow != null)
            weaponSelectionGlow.enabled = false;
    }

    void Update()
    {
        UpdateAmmoUI();

        
        if (Input.GetKeyDown(KeyCode.H))
        {
            UsePotion();
        }
        
    }

    // ------------------- ATUALIZAÇÃO DE MUNIÇÃO -------------------
    void UpdateAmmoUI()
    {
        if (ammoCircle != null)
        {
            float fill = AmmoManager.BulletsMax > 0
                ? (float)AmmoManager.Bullets / AmmoManager.BulletsMax
                : 0f;

            ammoCircle.fillAmount = Mathf.Clamp01(fill);
        }

        if (ammoText != null)
            ammoText.text = $"{AmmoManager.Bullets}/{AmmoManager.Magazine}";
    }

    // ------------------- ATUALIZAÇÃO DAS POÇÕES -------------------
    void UpdatePotionUI()
    {
        if (potionCountText != null)
            potionCountText.text = potionCount.ToString();
    }

    // ------------------- TROCA DE ARMA (para HUD) -------------------
    void HandleWeaponChanged(WeaponType wt)
    {
        if (weaponIcon != null)
        {
            switch (wt)
            {
                case WeaponType.Pistol: weaponIcon.sprite = pistolSprite; break;
                case WeaponType.Shotgun: weaponIcon.sprite = shotgunSprite; break;
                case WeaponType.Slot3: weaponIcon.sprite = slot3Sprite; break;
                case WeaponType.Slot4: weaponIcon.sprite = slot4Sprite; break;
            }
            weaponIcon.enabled = weaponIcon.sprite != null;
        }

        if (weaponSelectionGlow != null)
        {
            weaponSelectionGlow.enabled = true;
            pulseT = 0f;
            doPulse = true;
        }
    }

    // ------------------- USAR POÇÃO  -------------------
    public void UsePotion()
    {
        if (potionCount > 0)
        {
            potionCount--;
            UpdatePotionUI();

            
            Player player = FindAnyObjectByType<Player>();
            if (player != null)
            {
                player.Heal();
            }

            Debug.Log("[HUD] Poção usada! Cura aplicada.");
        }
        else
        {
            Debug.Log("[HUD] Sem poções restantes!");
        }
    }
}
