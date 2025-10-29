using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;
    public bool IsDead { get; private set; }

    [Header("Partículas e Efeitos")]
    public SpawnDamageParticles Particles;
    public Bullet bulletScript;

    [Header("Knockback")]
    [SerializeField] private float _knockbackTime = 0.25f;
    [SerializeField] private float _knockbackForce = 50f;
    private Rigidbody2D _rb;
    private bool _isKnockingBack;
    private float _timer;

    [Header("Identificador Único (para salvar estado)")]
    public string enemyID; 

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        
        if (string.IsNullOrEmpty(enemyID))
            enemyID = Guid.NewGuid().ToString();
    }

    void Start()
    {
        currentHealth = maxHealth;
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

    // -------------------- DANO --------------------
    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        currentHealth -= damage;

        if (bulletScript != null)
            StartKnockback(bulletScript.transform.right);

        Particles?.PlayBloodVFX();

        if (currentHealth <= 0)
            Die();
    }

    // -------------------- KNOCKBACK --------------------
    public void StartKnockback(Vector2 dir)
    {
        _isKnockingBack = true;
        _timer = 0f;
        _rb.AddForce(dir * _knockbackForce, ForceMode2D.Impulse);
    }

    // -------------------- MORTE --------------------
    private void Die()
    {
        IsDead = true;
        currentHealth = 0;
        gameObject.SetActive(false); 
        Debug.Log($"[Enemy] {gameObject.name} ({enemyID}) morreu.");
    }

    // -------------------- DANO NO PLAYER --------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(1);
            }
        }
    }
}
