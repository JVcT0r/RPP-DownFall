using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class EnemyBullet : MonoBehaviour
{
    public int damage = 1;
    public SpawnDamageParticles Particles;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider;
    private TrailRenderer trailRenderer;
    

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        trailRenderer = GetComponent<TrailRenderer>();

        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.gravityScale = 0;
        }

        Destroy(gameObject, 3f); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }

        
        if (collision.CompareTag("Inimigo"))
            return;

        
        if (Particles != null)
            Particles.PlayImpactVFX();

        if (trailRenderer != null)
            trailRenderer.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (capsuleCollider != null)
            capsuleCollider.enabled = false;

        Destroy(gameObject, 0.2f);
    }
}