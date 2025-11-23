using UnityEngine;
using UnityEngine.AI;

public class EnemyShooter : MonoBehaviour
{
    private static readonly int IsShooting = Animator.StringToHash("IsShooting");

    [Header("Referências")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform viewPivot;
    private Transform target;

    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;
    private Animator _animator;

    private Vector2 facingDir = Vector2.right;

    [Header("Comportamento")]
    public float moveSpeed = 3.5f;
    public float attackRange = 6f;
    public float fireRate = 1.2f;
    public float bulletSpeed = 10f;
    public float memorySeconds = 2f;
    public float checkInterval = 0.1f;

    private float checkTimer;

    [Header("Visão (Cone)")]
    public float viewDistance = 10f;
    [Range(10f, 180f)] public float viewAngle = 90f;
    public LayerMask obstacleMask;

    private bool canSeePlayer;
    private float nextFireTime;
    private float lastSeenTimer = Mathf.Infinity;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        Player p = FindAnyObjectByType<Player>();
        if (p != null)
            target = p.transform;

        
        if (target != null)
        {
            Vector2 startDir = ((Vector2)target.position - (Vector2)transform.position).normalized;
            facingDir = startDir;

            float angle = Mathf.Atan2(startDir.y, startDir.x) * Mathf.Rad2Deg;

            if (firePoint != null)
                firePoint.rotation = Quaternion.Euler(0, 0, angle);

            if (viewPivot != null)
                viewPivot.rotation = Quaternion.Euler(0, 0, angle);

            if (spriteRenderer != null)
                spriteRenderer.flipX = startDir.x < 0;
        }

        if (firePoint == null);
    }

    private void Update()
    {
        if (target == null) return;

        UpdateFacingDirection();

        
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            canSeePlayer = CheckPlayerInVision();
            if (canSeePlayer) lastSeenTimer = 0f;
        }

        if (!canSeePlayer) lastSeenTimer += Time.deltaTime;

        bool shouldChase = canSeePlayer || lastSeenTimer <= memorySeconds;

        if (shouldChase)
        {
            agent.SetDestination(target.position);
            _animator.SetBool(IsShooting, true);
            TryShoot();
        }
        else
        {
            agent.ResetPath();
            _animator.SetBool(IsShooting, false);
        }

        if (spriteRenderer != null)
            spriteRenderer.flipX = facingDir.x < 0;
    }

    private void UpdateFacingDirection()
    {
        Vector2 dirToPlayer = ((Vector2)target.position - (Vector2)transform.position).normalized;

        if (canSeePlayer)
            facingDir = Vector2.Lerp(facingDir, dirToPlayer, Time.deltaTime * 10f);
        else if (agent.desiredVelocity.sqrMagnitude > 0.001f)
            facingDir = Vector2.Lerp(facingDir, agent.desiredVelocity.normalized, Time.deltaTime * 5f);

        float angle = Mathf.Atan2(facingDir.y, facingDir.x) * Mathf.Rad2Deg;

        if (firePoint != null)
            firePoint.rotation = Quaternion.Euler(0, 0, angle);

        if (viewPivot != null)
            viewPivot.rotation = Quaternion.Euler(0, 0, angle);
    }

    private bool CheckPlayerInVision()
    {
        Vector2 origin = transform.position;
        Vector2 toPlayer = ((Vector2)target.position - origin);
        float dist = toPlayer.magnitude;

        if (dist > viewDistance) return false;

        Vector2 dirToPlayer = toPlayer.normalized;
        float angle = Vector2.Angle(facingDir, dirToPlayer);
        if (angle > viewAngle * 0.5f) return false;

        if (Physics2D.Raycast(origin, dirToPlayer, dist, obstacleMask))
            return false;

        return true;
    }

    private void TryShoot()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        if (firePoint == null || bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.gravityScale = 0;
            
            rb.linearVelocity = firePoint.transform.right * bulletSpeed;
        }

        Destroy(bullet, 3f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = canSeePlayer ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector2 dir = (facingDir.sqrMagnitude < 0.001f ? Vector2.right : facingDir.normalized);
        Vector3 origin = transform.position;

        Quaternion qRight = Quaternion.Euler(0, 0, viewAngle * 0.5f);
        Quaternion qLeft = Quaternion.Euler(0, 0, -viewAngle * 0.5f);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
        Gizmos.DrawLine(origin, origin + (Vector3)(qRight * dir) * viewDistance);
        Gizmos.DrawLine(origin, origin + (Vector3)(qLeft * dir) * viewDistance);
    }
}
