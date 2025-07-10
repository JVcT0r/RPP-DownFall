using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerMovement : MonoBehaviour
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

    private Vector2 moveInput;
    private Vector2 mousePos;
    private Camera mainCam;

    private bool isDashing = false;
    private float dashTime;
    private float lastDashTime;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // Entrada de movimento
        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        // Posição do mouse
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // Tiro
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        // Interação
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown && moveInput != Vector2.zero)
        {
            StartCoroutine(Dash());
        }

       /* // Rotação da lanterna para o mouse
        if (flashlight != null)
        {
            Vector2 dir = mousePos - (Vector2)transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            flashlight.rotation = Quaternion.Euler(0f, 0f, angle);
        }*/

        // Ativar/desativar lanterna com F
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

        // Rotação do jogador
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

            Debug.Log($"[INTERACT] {hit.name}: angle = {angle}");

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
}





