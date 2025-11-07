using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public SpawnDamageParticles Particles;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider;
    private TrailRenderer trailRenderer;
    public Light2D light;
    

    private void Start()
    {
      spriteRenderer = GetComponent<SpriteRenderer>();
      capsuleCollider = GetComponent<CapsuleCollider2D>();
      trailRenderer = GetComponent<TrailRenderer>();
      
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Inimigo"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                
                //Particles.PlayBloodVFX();
                enemy.TakeDamage(damage);
                
            }
        }
        
        Particles.PlayImpactVFX();
        light.enabled = false;
        trailRenderer.enabled = false;
        spriteRenderer.enabled = false;
        capsuleCollider.enabled = false;
        
    }

    
 
}



