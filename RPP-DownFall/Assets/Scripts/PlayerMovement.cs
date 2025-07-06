using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float interactRange = 1f;
    public LayerMask interactableLayer;

    private Vector2 moveInput;
    private Vector2 mousePos;
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
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
    }

    void FixedUpdate()
    {
        transform.position += (Vector3)(moveInput * moveSpeed * Time.fixedDeltaTime);

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
        Vector2 origin = transform.position;
        Vector2 direction = transform.right; 

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, interactRange, interactableLayer);

        if (hit.collider != null)
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }

        
        Debug.DrawRay(origin, direction * interactRange, Color.yellow, 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * interactRange);
    }
}



