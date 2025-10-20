using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Munição para o Player")]
    public int ammoAmount = 10; 

    private bool collected = false;

    public void OnInteracted()
    {
        if (collected) return;
        collected = true;

        
        AmmoManager.Magazine += ammoAmount;
        
        Destroy(gameObject);
    }
}