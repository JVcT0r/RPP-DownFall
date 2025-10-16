using UnityEngine;
using TMPro;

public class AmmoManager : MonoBehaviour
{
    [SerializeField] private TMP_Text ammoText;

    public static int ReloadTime = 1;
    public static int BulletsMax = 12;
    public static int MagazineMax = 60;

    public static int Bullets = 12;
    public static int Magazine = 60;

    private void Awake()
    {
        // Garante que sempre começa cheio
        Bullets = BulletsMax;
        Magazine = MagazineMax;
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (ammoText != null)
            ammoText.text = $"{Bullets}/{Magazine}";
    }

    public static void ResetAmmo()
    {
        Bullets = BulletsMax;
        Magazine = MagazineMax;
    }
}