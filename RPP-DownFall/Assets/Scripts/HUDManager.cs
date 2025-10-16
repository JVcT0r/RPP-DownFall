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
    public Image weaponMarker; 
    [Range(0f, 1f)] public float selectionPulseSpeed = 6f;

    [Header("Ícone de Arma")]
    public Image weaponIcon;          
    public Sprite pistolSprite;
    public Sprite shotgunSprite;
    public Sprite slot3Sprite;
    public Sprite slot4Sprite;

    [Header("Poção / Vida")]
    public Image potionIcon;
    public TMP_Text potionCountText;
    public int potionCount = 3;

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
        if (weaponMarker != null)
            weaponMarker.enabled = true;
    }

    void Update()
    {
        UpdateAmmoUI();

        if (doPulse && weaponSelectionGlow != null)
        {
            pulseT += Time.unscaledDeltaTime * selectionPulseSpeed;
            float a = Mathf.Abs(Mathf.Sin(pulseT)) * 0.6f + 0.2f; 
            var c = weaponSelectionGlow.color;
            c.a = a;
            weaponSelectionGlow.color = c;

            if (pulseT > Mathf.PI) 
            {
                doPulse = false;
                weaponSelectionGlow.enabled = false;
            }
        }
    }

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

    void UpdatePotionUI()
    {
        if (potionCountText != null)
            potionCountText.text = potionCount.ToString();
    }

    void HandleWeaponChanged(WeaponType wt)
    {
        if (weaponIcon != null)
        {
            switch (wt)
            {
                case WeaponType.Pistol:  weaponIcon.sprite = pistolSprite;  break;
                case WeaponType.Shotgun: weaponIcon.sprite = shotgunSprite; break;
                case WeaponType.Slot3:   weaponIcon.sprite = slot3Sprite;   break;
                case WeaponType.Slot4:   weaponIcon.sprite = slot4Sprite;   break;
            }
            weaponIcon.enabled = weaponIcon.sprite != null;
        }

        
        if (weaponSelectionGlow != null)
        {
            weaponSelectionGlow.enabled = true;
            pulseT = 0f;
            doPulse = true;
        }

        
        if (weaponMarker != null)
        {
            float targetRotation = (int)wt * 90f; 
            weaponMarker.rectTransform.localRotation = Quaternion.Euler(0, 0, -targetRotation);
        }
    }

    
    public void SetWeapon(int index)
    {
        HandleWeaponChanged((WeaponType)index);
    }

    public void UsePotion()
    {
        if (potionCount > 0)
        {
            potionCount--;
            UpdatePotionUI();
        }
    }
}
