using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class AmmoManager : MonoBehaviour
{
    [SerializeField]
    public TMP_Text ammoText;

    public static int ReloadTime = 1;
    
    public static int BulletsMax;
    public static int MagazineMax;
    
    public static int Bullets;
    public static int Magazine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BulletsMax = 12;
        MagazineMax = 60;
        
        Bullets = 12;
        Magazine = 60;
    }

    // Update is called once per frame
    void Update()
    {
      ammoText.text = "Ammo: " + Bullets + "/" + Magazine;  
    }
}
