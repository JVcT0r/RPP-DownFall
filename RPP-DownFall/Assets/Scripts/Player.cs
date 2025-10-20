using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    private GameManager gameManager;

    [Header("Movimento e Combate")]
    public float moveSpeed = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 500f;
    public SpawnDamageParticles Particles;
    public SpawnDamageParticles ParticlesDmg;

    [Header("Interação")]
    public float interactRange = 1.5f;
    public LayerMask interactableLayer;

    [Header("Lanterna")]
    public Transform flashlight;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Tremor da Câmera")] 
    [SerializeField]
    private CameraShakeController camShake;
    [SerializeField]
    private float shakeIntensity = 5f;
    [SerializeField]
    private float shakeTime = 0.2f;

    [Header("Vida")]
    public int maxHealth = 3;
    public int CurrentHealth { get; private set; }
    public GameObject deathScreen;

    [Header("Cura")]
    public int healAmount = 1; // 🔹 Ajustável no Inspector

    private WeaponManager weaponManager;
    private Vector2 moveInput;
    private Vector2 mousePos;
    private Camera mainCam;
    private bool isDashing = false;
    private bool isReloading = false;
    private float dashTime;
    private float lastDashTime;
    public bool dead = false;

    private Rigidbody2D rb;

    void Awake()
    {
        gameManager =  GameObject.Find("GameManager").GetComponent<GameManager>();
        
        mainCam = Camera.main;
        CurrentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        weaponManager = GetComponent<WeaponManager>();
        if (weaponManager == null)
            weaponManager = GetComponentInParent<WeaponManager>();

        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    void Update()
    {
        if (gameManager.GetPaused()) return;
        
        if (!dead)
        {
            Movement();
            
            if (weaponManager == null || weaponManager.Current == WeaponType.Pistol)
                ShootPistol();
            else if (weaponManager.Current == WeaponType.Shotgun)
                ShootShotgun();

            Reload();
            TryInteract();
            DashInput();
            Flashlight();
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            Vector2 targetPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);
        }

        Vector2 aimDir = mousePos - rb.position;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Movement()
    {
        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
    }

    // -------------------- TIRO PISTOLA --------------------
    void ShootPistol()
    {
        if (Input.GetMouseButtonDown(0) && AmmoManager.Bullets > 0 && !isReloading)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Particles.PlayFireVFX();

            if (bullet.TryGetComponent<Rigidbody2D>(out var bulletRb))
                bulletRb.linearVelocity = firePoint.right * bulletSpeed;

            AmmoManager.Bullets--;
            camShake.ShakeCamera(shakeIntensity, shakeTime);
            Destroy(bullet, 3f);
        }
    }

    // -------------------- TIRO SHOTGUN --------------------
    void ShootShotgun()
    {
        if (Input.GetMouseButtonDown(0) && AmmoManager.Bullets > 0 && !isReloading)
        {
            AmmoManager.Bullets--;

            for (int i = 0; i < 6; i++)
            {
                GameObject bullet = Instantiate(
                    bulletPrefab,
                    firePoint.position,
                    firePoint.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-12, 12))
                );

                if (bullet.TryGetComponent<Rigidbody2D>(out var bulletRb))
                    bulletRb.linearVelocity = firePoint.right * bulletSpeed * UnityEngine.Random.Range(0.9f, 1.1f);

                camShake.ShakeCamera(shakeIntensity, shakeTime);
                Destroy(bullet, 2f);
            }

            Particles.PlayFireVFX();
        }
    }

    // -------------------- RELOAD --------------------
    void Reload()
    {
        int MaxBullets = AmmoManager.BulletsMax;
        int MaxMagazine = AmmoManager.MagazineMax;

        if (AmmoManager.Magazine > 0 && AmmoManager.Bullets < MaxBullets &&
            !isReloading && Input.GetKey(KeyCode.R))
        {
            int BulletsNeeded = MaxBullets - AmmoManager.Bullets;
            int bFromMagazine = Mathf.Min(BulletsNeeded, AmmoManager.Magazine);

            AmmoManager.Bullets += bFromMagazine;
            AmmoManager.Magazine -= bFromMagazine;
        }
    }

    // -------------------- LANTERNA --------------------
    private void Flashlight()
    {
        if (Input.GetKeyDown(KeyCode.F) && flashlight != null)
        {
            var light2D = flashlight.GetComponent<Light2D>();
            if (light2D != null)
                light2D.enabled = !light2D.enabled;
        }
    }

    // -------------------- INTERAÇÃO --------------------
    void TryInteract()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange, interactableLayer);
            float coneAngle = 80f;
            Vector2 forward = transform.right;

            foreach (var hit in hits)
            {
                Vector2 dirToTarget = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
                float angle = Vector2.Angle(forward, dirToTarget);

                if (angle <= coneAngle)
                {
                    
                    if (hit.CompareTag("Interagir"))
                    {
                        Debug.Log("Interagiu com: " + hit.name);

                        
                        hit.SendMessage("OnInteracted", SendMessageOptions.DontRequireReceiver);
                        break;
                    }
                }
            }
        }
    }


    // -------------------- DASH --------------------
    private IEnumerator Dash()
    {
        isDashing = true;
        dashTime = 0f;
        lastDashTime = Time.time;
        Vector2 dashDirection = moveInput;

        while (dashTime < dashDuration)
        {
            transform.position += (Vector3)(dashDirection * dashSpeed * Time.deltaTime);
            dashTime += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    private void DashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown && moveInput != Vector2.zero)
            StartCoroutine(Dash());
    }

    // -------------------- CURA --------------------
    public void Heal()
    {
        if (dead) return;

        CurrentHealth += healAmount;
        if (CurrentHealth > maxHealth)
            CurrentHealth = maxHealth;

        Debug.Log($"[Player] Curado em {healAmount} pontos. Vida atual: {CurrentHealth}");
    }

    // -------------------- DANO E MORTE --------------------
    public void TakeDamage(int amount)
    {
        if (dead) return;

        CurrentHealth -= amount;
        ParticlesDmg.PlayBloodVFX();
        camShake.ShakeCamera(2f, 0.5f);

        if (CurrentHealth <= 0)
            Die();
    }

    void Die()
    {
        dead = true;
        rb.linearVelocity = Vector2.zero;
        if (deathScreen != null)
            deathScreen.SetActive(true);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
