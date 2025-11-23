using UnityEngine;

public class SlashProjectile : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Player p = col.GetComponent<Player>();
            p?.TakeDamage(damage);
        }
        
        Destroy(gameObject);
    }
}