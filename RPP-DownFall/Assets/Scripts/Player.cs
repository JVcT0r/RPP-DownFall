using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Movimento e Combate")]
    public float moveSpeed = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 500f;

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
    private int currentHealth;
    public GameObject deathScreen;

    private Vector2 moveInput;
    private Vector2 mousePos;
    private Camera mainCam;

    private bool isDashing = false;
    private float dashTime;
    private float lastDashTime;

    void Awake()
    {
        mainCam = Camera.main;
        currentHealth = maxHealth;
        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    void Update()
    {
        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown && moveInput != Vector2.zero)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.F) && flashlight != null)
        {
            var light2D = flashlight.GetComponent<Light2D>();
            if (light2D != null)
                light2D.enabled = !light2D.enabled;
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            transform.position += (Vector3)(moveInput * moveSpeed * Time.fixedDeltaTime);
        }

        Vector2 aimDir = mousePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (bullet.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = firePoint.right * bulletSpeed;
        }

        camShake.ShakeCamera(shakeIntensity, shakeTime);
        Destroy(bullet, 3f);
    }

    void TryInteract()
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
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (deathScreen != null)
            deathScreen.SetActive(true);

        Time.timeScale = 0f;
    }

    // Chamado pelo botão "Tentar novamente"
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Chamado pelo botão "Sair"
    public void QuitGame()
    {
        Application.Quit();
    }
}

