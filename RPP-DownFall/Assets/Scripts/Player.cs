using Mono.Cecil;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class Player : MonoBehaviour
{
    [Header("Movimento e Combate")]
    public float moveSpeed = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 500f;
    public SpawnDamageParticles Particles;

    [Header("Interação")]
    public float interactRange = 1.5f;
    public LayerMask interactableLayer;

    [Header("Lanterna")]
    public Transform flashlight;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Tremor da Cam")] 
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
        mainCam = Camera.main;
        CurrentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>(); 

        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    void Update()
    {
        if (!dead)
        {
            Movement();
            Shoot();
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
    
    void Shoot()
    {
        if (Input.GetMouseButtonDown(0) && AmmoManager.Bullets > 0 && !isReloading)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Particles.PlayFireVFX();

            if (bullet.TryGetComponent<Rigidbody2D>(out var bulletRb))
            {
                bulletRb.linearVelocity = firePoint.right * bulletSpeed;
            }
            AmmoManager.Bullets--;

            camShake.ShakeCamera(shakeIntensity, shakeTime);
            Destroy(bullet, 3f);
        }
       
    }
    
    void ShootShotgun()
    {
        if (Input.GetMouseButtonDown(1) && AmmoManager.Bullets > 0 && !isReloading)
        {
            AmmoManager.Bullets--;
            
            for (int i = 8; i >=0;)
            {
                camShake.ShakeCamera(shakeIntensity, shakeTime);
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position,
                    firePoint.rotation * Quaternion.Euler(0, 0, Random.Range(-15, 15))); //tentativa de fazer os tiro espalhar, funciona, mas com a bala usando constant force ao inves da função abaixo.
                
                if (bullet.TryGetComponent<Rigidbody2D>(out var bulletRb))
                {
                    bulletRb.linearVelocity = firePoint.right * bulletSpeed; //comentando essa funçao e ativando o constant force no bullet prefab, as balas vao espalhar
                }

                i--;                
                Destroy(bullet, 3f);
            }
          
        }
       
    }
    

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
    
    private void Flashlight()
    {
        if (Input.GetKeyDown(KeyCode.F) && flashlight != null)
        {
            var light2D = flashlight.GetComponent<Light2D>();
            if (light2D != null)
                light2D.enabled = !light2D.enabled;
        }  
    }

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
                    var interactable = hit.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        interactable.Interact();
                        break;
                    }
                }
            }
        }
        
    }

    private System.Collections.IEnumerator Dash()
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
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown && moveInput != Vector2.zero)
            {
                StartCoroutine(Dash());
            }
        }
    }

    

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactRange);

        float coneAngle = 80f;
        Vector3 rightDir = transform.right;

        Quaternion leftRot = Quaternion.Euler(0, 0, -coneAngle);
        Quaternion rightRot = Quaternion.Euler(0, 0, coneAngle);

        Vector3 leftDir = leftRot * rightDir * interactRange;
        Vector3 rightDirFinal = rightRot * rightDir * interactRange;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, leftDir);
        Gizmos.DrawRay(transform.position, rightDirFinal);
    }
    

    public void TakeDamage(int amount)
    {
        camShake.ShakeCamera(3f, 0.5f);
        CurrentHealth -= amount;
        

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (deathScreen != null)
            deathScreen.SetActive(true);

        Time.timeScale = 0f;
        gameObject.SetActive(false);
        dead =  true;
    }
}
