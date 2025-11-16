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
    public GameObject bulletShotgunPrefab;
    public Transform firePoint;
    public float bulletSpeed = 500f;
    public SpawnDamageParticles Particles;
    public SpawnDamageParticles ParticlesDmg;

    [Header("Interação")]
    public float interactRange = 1.5f;
    public LayerMask interactableLayer;

    [Header("UI de Interação")]
    public GameObject hintLerDocumento;  // <-- Texto "Aperte E para ler"

    [Header("Lanterna")]
    public Transform flashlight;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public LayerMask dashObstacleMask;
    public float dashSkin = 0.08f;

    [Header("Tremor da Câmera")] 
    [SerializeField] private CameraShakeController camShake;
    [SerializeField] private float shakeIntensity = 5f;
    [SerializeField] private float shakeTime = 0.2f;

    [Header("Vida")]
    public int maxHealth = 3;
    public int CurrentHealth { get; set; }  
    public FullScreenFXController fullScreenFX;
    public GameObject deathScreen;

    [Header("Cura")]
    public int healAmount = 1;
    public int potionCount = 3; 
    
    [Header("Audio")]
    public AudioClip sfxTiro;
    public AudioClip sfxReload;
    private AudioSource audioSource;

    [Header("Documentos")]
    public GameObject painelDocumento;
    public TMPro.TMP_Text textoDocumento;
    public bool isReadingDocument = false;

    private WeaponManager weaponManager;
    private Vector2 moveInput;
    private Vector2 mousePos;
    private Camera mainCam;

    private bool isDashing = false;
    private bool isReloading = false;
    private float lastDashTime;
    public bool dead = false;

    private Rigidbody2D rb;
    private CapsuleCollider2D capsule;
    private Animator anim;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>();
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();

        CurrentHealth = maxHealth;
        weaponManager = GetComponent<WeaponManager>() ?? GetComponentInParent<WeaponManager>();

        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // BLOQUEIO AO LER DOCUMENTO
        if (isReadingDocument)
        {
            if (hintLerDocumento != null) hintLerDocumento.SetActive(false);
            return;
        }

        if (gameManager != null && gameManager.GetPaused()) return;
        if (dead) return;

        if (CurrentHealth <= 2) fullScreenFX.Hurt();
        else fullScreenFX.NotHurt();

        Movement();
        moveSpeed = isReloading ? 2.5f : 5f;

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
        if (!isDashing && !isReadingDocument)
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
        if (isReloading || isReadingDocument) return;

        if (Input.GetMouseButtonDown(0) && AmmoManager.pistolBullets > 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Particles?.PlayFireVFX();
            audioSource.pitch = UnityEngine.Random.Range(1f, 1.1f);
            audioSource.PlayOneShot(sfxTiro);

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
        if (isReadingDocument) return;

        if (Input.GetMouseButtonDown(0) && AmmoManager.shotgunBullets > 0)
        {
            AmmoManager.shotgunBullets--;

            for (int i = 0; i < 6; i++)
            {
                GameObject bullet = Instantiate(
                    bulletShotgunPrefab,
                    firePoint.position,
                    firePoint.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-80, 80))
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
        if (isReadingDocument) return;

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

    public void SetIsReloadingToTrue()
    {
        isReloading = true;
    }
    public void SetIsReloadingToFalse()
    {
        isReloading = false;
    }

    void ReloadSFX()
    {
        audioSource.pitch = UnityEngine.Random.Range(1f, 1.1f);
        audioSource.PlayOneShot(sfxReload, 0.3f);
    }

    // -------------------- LANTERNA --------------------
    private void Flashlight()
    {
        if (isReadingDocument) return;

        if (Input.GetKeyDown(KeyCode.F) && flashlight != null)
        {
            var light2D = flashlight.GetComponent<Light2D>();
            if (light2D != null)
                light2D.enabled = !light2D.enabled;
        }
    }

    // -------------------- INTERAÇÃO + HINT + DOCUMENTO --------------------
    void TryInteract()
    {
        if (isReadingDocument) return;

        //----------------------------------------------------
        // MOSTRAR / ESCONDER "APERTE E PARA LER"
        //----------------------------------------------------
        bool encontrouDocumento = false;

        Collider2D[] hitsDocsHint = Physics2D.OverlapCircleAll(transform.position, interactRange);
        Vector2 forwardHint = transform.right;
        float coneHint = 70f;

        foreach (var hit in hitsDocsHint)
        {
            var doc = hit.GetComponent<DocumentObject>();
            if (doc == null) continue;

            Vector2 dir = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
            float ang = Vector2.Angle(forwardHint, dir);

            if (ang <= coneHint / 2f)
            {
                encontrouDocumento = true;
                break;
            }
        }

        if (hintLerDocumento != null)
            hintLerDocumento.SetActive(encontrouDocumento);

        //----------------------------------------------------
        // SE NÃO APERTOU E, NÃO INTERAGIR
        //----------------------------------------------------
        if (!Input.GetKeyDown(KeyCode.E)) return;

        //----------------------------------------------------
        // TENTAR ABRIR DOCUMENTO
        //----------------------------------------------------
        Collider2D[] hitsDocs = Physics2D.OverlapCircleAll(transform.position, interactRange);

        foreach (var hit in hitsDocs)
        {
            var doc = hit.GetComponent<DocumentObject>();
            if (doc == null) continue;

            Vector2 dir = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
            float ang = Vector2.Angle(transform.right, dir);

            if (ang <= 70f / 2f)
            {
                AbrirDocumento(doc);
                return;
            }
        }

        //----------------------------------------------------
        // INTERAÇÕES NORMAIS
        //----------------------------------------------------
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Vector2 dir = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
            float ang = Vector2.Angle(transform.right, dir);

            if (ang <= 70f / 2f && hit.CompareTag("Interagir"))
            {
                hit.SendMessage("OnInteracted", SendMessageOptions.DontRequireReceiver);
                break;
            }
        }
    }

    // -------------------- ABRIR / FECHAR DOCUMENTO --------------------
    void AbrirDocumento(DocumentObject doc)
    {
        isReadingDocument = true;
        Time.timeScale = 0f;

        if (hintLerDocumento != null) hintLerDocumento.SetActive(false);
        if (painelDocumento != null) painelDocumento.SetActive(true);

        textoDocumento.text = doc.documentText;
    }

    public void FecharDocumento()
    {
        isReadingDocument = false;
        Time.timeScale = 1f;

        if (painelDocumento != null)
            painelDocumento.SetActive(false);

        textoDocumento.text = "";
    }

    // -------------------- DASH --------------------
    private IEnumerator Dash()
    {
        if (isDashing || isReadingDocument) yield break;

        isDashing = true;

        Vector2 dashDir = moveInput.sqrMagnitude > 0.01f
            ? moveInput
            : (mousePos - (Vector2)transform.position).normalized;

        float desiredDistance = dashSpeed * dashDuration;
        float allowedDistance = desiredDistance;

        RaycastHit2D hit = Physics2D.CapsuleCast(
            rb.position,
            capsule.size,
            CapsuleDirection2D.Vertical,
            0f,
            dashDir,
            desiredDistance,
            dashObstacleMask
        );

        if (hit.collider != null)
            allowedDistance = Mathf.Max(0f, hit.distance - dashSkin);

        float elapsed = 0f;
        float speed = allowedDistance / dashDuration;

        while (elapsed < dashDuration)
        {
            rb.MovePosition(rb.position + dashDir * speed * Time.fixedDeltaTime);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
    }

    private void DashInput()
    {
        if (isReadingDocument) return;

        if (Input.GetKeyDown(KeyCode.LeftShift) &&
            Time.time >= lastDashTime + dashCooldown &&
            !isReloading)
        {
            lastDashTime = Time.time;
            StartCoroutine(Dash());
        }
    }

    // -------------------- CURA --------------------
    public void Heal()
    {
        if (dead || isReadingDocument) return;

        CurrentHealth += healAmount;
        if (CurrentHealth > maxHealth)
            CurrentHealth = maxHealth;
    }

    // -------------------- DANO --------------------
    public void TakeDamage(int amount)
    {
        if (dead || isReadingDocument) return;

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
        Time.timeScale = 0;
    }

    // -------------------- GIZMOS --------------------
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactRange);

        float coneAngle = 70f;
        int rayCount = 20;
        Vector2 forward = transform.right;

        for (int i = 0; i <= rayCount; i++)
        {
            float t = Mathf.Lerp(-coneAngle / 2, coneAngle / 2, (float)i / rayCount);
            Vector2 dir = Quaternion.Euler(0, 0, t) * forward;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + dir * interactRange);
        }
    }
}
