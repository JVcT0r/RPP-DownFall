using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public SpawnDamageParticles Particles;
    private SpriteRenderer spriteRenderer;
    public Bullet bulletScript;
    
    [SerializeField] private float _knockbackTime = 0.25f;
    [SerializeField] private float _knockbackForce = 50f;
    private Rigidbody2D _rb;
    private bool _isKnockingBack;
    private float _timer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_isKnockingBack)
        {
            _timer += Time.deltaTime;
            if (_timer > _knockbackTime)
            {
                _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
                _rb.angularVelocity = 0f;
                _isKnockingBack = false;
            }
        }
    }

    public void StartKnockback(Vector2 dir)
    {
        _isKnockingBack = true;
        _timer = 0f;
        _rb.AddForce(dir * _knockbackForce, ForceMode2D.Impulse);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartKnockback(bulletScript.transform.forward);
        Particles.PlayBloodVFX();

        if (currentHealth <= 0)
        {
            Die();
        }
        
    }

    
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(1);
            }
        }
    }
    void Die()
    {
        Destroy(gameObject);
    }
}

