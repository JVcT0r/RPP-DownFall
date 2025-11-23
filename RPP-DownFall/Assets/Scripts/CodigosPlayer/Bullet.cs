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

        
        GameObject[] slashes = GameObject.FindGameObjectsWithTag("BossProjectile");
        foreach (var slash in slashes)
        {
            foreach (var colSlash in slash.GetComponentsInChildren<Collider2D>())
            {
                Physics2D.IgnoreCollision(capsuleCollider, colSlash);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ------------------- ACERTOU ENEMY NORMAL -------------------
        if (collision.CompareTag("Inimigo"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                collision.GetComponent<EnemyAI>()?.OnHitByPlayer();
                enemy.TakeDamage(damage);
            }

            DestroyBullet();
            return;
        }

        // ------------------- ACERTOU O BOSS -------------------
        if (collision.CompareTag("Boss"))
        {
            BossHealth bh = collision.GetComponent<BossHealth>();
            if (bh != null)
            {
                // ▼ SE for a PISTOLA:
                bh.TakeHitFromGun(false);
            }

            DestroyBullet();
            return;
        }

        // ------------------- ACERTOU OBSTÁCULO -------------------
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            DestroyBullet();
            return;
        }
    }

    // ------------------- DESTRUIR EFEITOS -------------------
    private void DestroyBullet()
    {
        Particles?.PlayImpactVFX();

        if (light != null) light.enabled = false;
        if (trailRenderer != null) trailRenderer.enabled = false;
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (capsuleCollider != null) capsuleCollider.enabled = false;

        Destroy(gameObject, 0.1f);
    }
}
