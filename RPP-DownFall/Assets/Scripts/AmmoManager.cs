using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class AmmoManager : MonoBehaviour
{
    [SerializeField]
    public TMP_Text ammoText;
    
    
    public static int BulletsMax;
    public static int MagazineMax;
    
    public static int Bullets;
    public static int Magazine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BulletsMax = 12;
        MagazineMax = 64;
        
        Bullets = 12;
        Magazine = 64;
    }

    // Update is called once per frame
    void Update()
    {
      ammoText.text = "Ammo: " + Bullets + "/" + Magazine;  
    }
}
