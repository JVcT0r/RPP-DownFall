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
    private Animator anim;

    private Vector2 facingDir;

    [Header("Visão")]
    public float viewDistance = 10f;
    [Range(10f, 180f)] public float viewAngle = 90f;
    public LayerMask obstacleMask;

    [Header("Comportamento")]
    public float checkInterval = 0.1f;
    public float memorySeconds = 1.25f;
    public float fireRate = 1.2f;
    public float bulletSpeed = 10f;

    private float checkTimer;
    private float lastSeenTimer = Mathf.Infinity;
    private float nextFireTime;

    public bool canSeePlayer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

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
            facingDir = ((Vector2)target.position - (Vector2)transform.position).normalized;
            spriteRenderer.flipX = facingDir.x < 0;
        }
        else
        {
            facingDir = transform.right;
        }
    }

    private void Update()
    {
        if (target == null) return;

        UpdateFacingDirection(); // IGUAL AO MELEE

        // ---- visão ----
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;

            canSeePlayer = CheckPlayerInVision();
            if (canSeePlayer) lastSeenTimer = 0f;
        }

        if (!canSeePlayer)
            lastSeenTimer += Time.deltaTime;

        bool shouldChase = canSeePlayer || lastSeenTimer <= memorySeconds;

        if (shouldChase)
        {
            agent.SetDestination(target.position);
            anim.SetBool(IsShooting, true);
            TryShoot();
        }
        else
        {
            anim.SetBool(IsShooting, false);
            agent.ResetPath();
        }

        // Flip visual IGUAL MELEE
        spriteRenderer.flipX = facingDir.x < 0;
    }

    private void UpdateFacingDirection()
    {
        Vector2 dirToPlayer = ((Vector2)target.position - (Vector2)transform.position).normalized;

        // SE ENXERGA O PLAYER → olha direto
        if (canSeePlayer)
        {
            facingDir = Vector2.Lerp(facingDir, dirToPlayer, Time.deltaTime * 10f);
        }
        // SENÃO → olha para onde está andando
        else if (agent.desiredVelocity.sqrMagnitude > 0.001f)
        {
            facingDir = Vector2.Lerp(facingDir, agent.desiredVelocity.normalized, Time.deltaTime * 5f);
        }

        // ROTAÇÃO IGUAL AO MELEE
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

        if (dist > viewDistance)
            return false;

        Vector2 dirToPlayer = toPlayer.normalized;
        float angle = Vector2.Angle(facingDir, dirToPlayer);

        if (angle > viewAngle * 0.5f)
            return false;

        if (Physics2D.Raycast(origin, dirToPlayer, dist, obstacleMask))
            return false;

        return true;
    }

    private void TryShoot()
    {
        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + fireRate;

        if (bulletPrefab == null || firePoint == null)
            return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.gravityScale = 0;

            // 🔥 O tiro segue o firePoint, QUE SEGUE O facingDir (igual melee)
            rb.linearVelocity = firePoint.right * bulletSpeed;
        }

        Destroy(bullet, 3f);
    }

    // -------------------------------
    //             GIZMOS
    // -------------------------------
    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position;

        Gizmos.color = canSeePlayer ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(origin, viewDistance);

        Vector2 dir = Application.isPlaying
            ? (facingDir.sqrMagnitude < 0.01f ? (Vector2)transform.right : facingDir.normalized)
            : (Vector2)transform.right;

        Quaternion qRight = Quaternion.Euler(0, 0, viewAngle * 0.5f);
        Quaternion qLeft = Quaternion.Euler(0, 0, -viewAngle * 0.5f);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
        Gizmos.DrawLine(origin, origin + (Vector3)(qRight * dir) * viewDistance);
        Gizmos.DrawLine(origin, origin + (Vector3)(qLeft * dir) * viewDistance);
    }
}
