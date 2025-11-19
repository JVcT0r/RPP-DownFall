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
    
    private Rigidbody2D _rb;
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
    

    // -------------------- DANO --------------------
    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        currentHealth -= damage;
        
        Particles?.PlayBloodVFX();

        if (currentHealth <= 0)
            Die();
    }
    

    // -------------------- MORTE --------------------
    private void Die()
    {
        IsDead = true;
        currentHealth = 0;

        
        EnemyDrop drop = GetComponent<EnemyDrop>();
        if (drop != null)
            drop.DroparItens();

        gameObject.SetActive(false);
        
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
