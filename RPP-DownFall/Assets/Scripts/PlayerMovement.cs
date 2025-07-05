using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    private Vector2 moveInput;
    private Vector2 mousePos;

    void Update()
    {
        
        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        
        if (Input.GetMouseButtonDown(0)) 
        {
            Shoot();
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
}


