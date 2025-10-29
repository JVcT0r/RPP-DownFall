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
    public int CurrentHealth { get; set; }  
    public GameObject deathScreen;

    [Header("Cura")]
    public int healAmount = 1;
    public int potionCount = 3; 

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
    
    private Animator anim;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>();
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        CurrentHealth = maxHealth;

        weaponManager = GetComponent<WeaponManager>();
        if (weaponManager == null)
            weaponManager = GetComponentInParent<WeaponManager>();

        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        
    }

    void Update()
    {
        if (gameManager != null && gameManager.GetPaused()) return;
        if (dead) return;

        Movement();
        DashInput();
        Flashlight();
        TryInteract();
        Reload();

        if (weaponManager == null || weaponManager.Current == WeaponType.Pistol)
            ShootPistol();
        else if (weaponManager.Current == WeaponType.Shotgun)
            ShootShotgun();
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
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
    }

    // -------------------- TIRO PISTOLA --------------------
    void ShootPistol()
    {
        if(isReloading) return;
        if (Input.GetMouseButtonDown(0) && AmmoManager.pistolBullets > 0)
        {
            
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Particles?.PlayFireVFX();

            if (bullet.TryGetComponent<Rigidbody2D>(out var bulletRb))
                bulletRb.linearVelocity = firePoint.right * bulletSpeed;

            AmmoManager.pistolBullets--;
            camShake?.ShakeCamera(shakeIntensity, shakeTime);
            Destroy(bullet, 3f);
        }
    }

    // -------------------- TIRO SHOTGUN --------------------
    void ShootShotgun()
    {
        if (Input.GetMouseButtonDown(0) && AmmoManager.shotgunBullets > 0)
        {
            AmmoManager.shotgunBullets--;

            for (int i = 0; i < 6; i++)
            {
                GameObject bullet = Instantiate(
                    bulletPrefab,
                    firePoint.position,
                    firePoint.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-12, 12))
                );

                if (bullet.TryGetComponent<Rigidbody2D>(out var bulletRb))
                    bulletRb.linearVelocity = firePoint.right * bulletSpeed * UnityEngine.Random.Range(0.9f, 1.1f);

                camShake?.ShakeCamera(shakeIntensity, shakeTime);
                Destroy(bullet, 2f);
            }

            Particles?.PlayFireVFX();
        }
    }

    // -------------------- RELOAD --------------------
    void Reload()
    {
        //if (isReloading) return;

        if (Input.GetKey(KeyCode.R) && !isReloading)
        {
            switch (WeaponManager.Instance.Current)
            {
                case WeaponType.Pistol:
                    if (AmmoManager.pistolMagazine > 0 && AmmoManager.pistolBullets < AmmoManager.pistolBulletsMax)
                    {
                        isReloading = true;
                        
                        int bulletsNeeded = AmmoManager.pistolBulletsMax - AmmoManager.pistolBullets;
                        int bFromMagazine = Mathf.Min(bulletsNeeded, AmmoManager.pistolMagazine);
                        anim.Play("HandgunReload");
                        AmmoManager.pistolBullets += bFromMagazine;
                        AmmoManager.pistolMagazine -= bFromMagazine;
                        isReloading = false;
                       
                    }
                    break;

                case WeaponType.Shotgun:
                    if (AmmoManager.shotgunMagazine > 0 && AmmoManager.shotgunBullets < AmmoManager.shotgunBulletsMax)
                    {
                        isReloading = true;
                        int bulletsNeeded = AmmoManager.shotgunBulletsMax - AmmoManager.shotgunBullets;
                        int bFromMagazine = Mathf.Min(bulletsNeeded, AmmoManager.shotgunMagazine);

                        AmmoManager.shotgunBullets += bFromMagazine;
                        AmmoManager.shotgunMagazine -= bFromMagazine;
                        isReloading = false;
                    }
                    break;
            }
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
        if (!Input.GetKeyDown(KeyCode.E)) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);
        Vector2 forward = transform.right;
        float coneAngle = 70f;

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Vector2 dirToTarget = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
            float angle = Vector2.Angle(forward, dirToTarget);

            if (angle <= coneAngle / 2f && hit.CompareTag("Interagir"))
            {
                hit.SendMessage("OnInteracted", SendMessageOptions.DontRequireReceiver);
                break;
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
        ParticlesDmg?.PlayBloodVFX();
        camShake?.ShakeCamera(2f, 0.5f);

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

    // -------------------- GIZMOS --------------------
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactRange);

        float coneAngle = 70f;
        int rayCount = 20;
        Vector2 forward = transform.right;
        float halfAngle = coneAngle / 2f;

        for (int i = 0; i <= rayCount; i++)
        {
            float t = Mathf.Lerp(-halfAngle, halfAngle, (float)i / rayCount);
            Quaternion rot = Quaternion.Euler(0, 0, t);
            Vector2 dir = rot * forward;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + dir * interactRange);
        }
    }
}
