using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //private Vector3 rotation;
    SpawnDamageParticles particles
    
    public int damage = 1;

    private void Start()
    {
        //rotation = transform.eulerAngles;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Inimigo"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                particles().SpawnDamageParticle(transform.position, transform.eulerAngles);
                enemy.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}



