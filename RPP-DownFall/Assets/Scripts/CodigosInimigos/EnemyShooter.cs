using UnityEngine;
using UnityEngine.AI;

public class EnemyShooter : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform firePoint;        // ponto de onde as balas saem
    [SerializeField] private GameObject bulletPrefab;    // prefab da bala
    [SerializeField] private Transform viewPivot;        // define a direção do cone (opcional)
    private Transform target;
    private NavMeshAgent agent;

    [Header("Comportamento")]
    public float moveSpeed = 3.5f;
    public float attackRange = 6f;
    public float fireRate = 1.2f;
    public float bulletSpeed = 10f;
    public float memorySeconds = 2f;

    [Header("Visão (Cone)")]
    public float viewDistance = 10f;
    [Range(10f, 180f)] public float viewAngle = 90f;
    public LayerMask obstacleMask;

    private bool canSeePlayer;
    private float nextFireTime;
    private float lastSeenTimer = Mathf.Infinity;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        Player p = FindAnyObjectByType<Player>();
        if (p != null)
            target = p.transform;

        if (firePoint == null)
            Debug.LogWarning($"[EnemyShooter] FirePoint não atribuído em {gameObject.name}");
    }

    private void Update()
    {
        if (target == null) return;

        canSeePlayer = CheckPlayerInVision();

        if (canSeePlayer)
        {
            lastSeenTimer = 0f;
            float distance = Vector2.Distance(transform.position, target.position);

            if (distance > attackRange)
            {
                agent.SetDestination(target.position);
            }
            else
            {
                agent.ResetPath();
                agent.velocity = Vector3.zero;
                TryShoot();
            }
        }
        else
        {
            lastSeenTimer += Time.deltaTime;

            if (lastSeenTimer <= memorySeconds)
            {
                agent.SetDestination(target.position);
            }
            else
            {
                agent.ResetPath();
                agent.velocity = Vector3.zero;
            }
        }

        // Mantém o inimigo no plano 2D
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;

        // --- Rotação suave do corpo na direção do movimento ---
        Vector3 velocity = agent.velocity;

        if (velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0, 0, angle),
                Time.deltaTime * 10f
            );
        }
        else if (canSeePlayer && target != null)
        {
            // se parado, gira o corpo pra mirar no player
            Vector2 dir = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0, 0, angle),
                Time.deltaTime * 10f
            );
        }
    }

    // -------------------- VISÃO EM CONE --------------------
    private bool CheckPlayerInVision()
    {
        if (target == null) return false;

        Vector2 origin = transform.position;
        Vector2 toPlayer = ((Vector2)target.position - origin);
        float dist = toPlayer.magnitude;
        if (dist > viewDistance) return false;

        Vector2 dirToPlayer = toPlayer.normalized;
        Vector2 forward = viewPivot ? (Vector2)viewPivot.right : (Vector2)transform.right;

        float angle = Vector2.Angle(forward, dirToPlayer);
        if (angle > viewAngle / 2f)
            return false;

        if (Physics2D.Raycast(origin, dirToPlayer, dist, obstacleMask))
            return false;

        return true;
    }

    // -------------------- TIRO --------------------
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
            rb.linearVelocity = firePoint.right * bulletSpeed;
        }

        Destroy(bullet, 3f);
    }

    // -------------------- GIZMOS --------------------
    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position;
        Gizmos.color = canSeePlayer ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(origin, viewDistance);

        Vector3 baseForward = viewPivot ? viewPivot.right : transform.right;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, viewAngle / 2f) * baseForward;
        Vector3 leftBoundary = Quaternion.Euler(0, 0, -viewAngle / 2f) * baseForward;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
        Gizmos.DrawLine(origin, origin + rightBoundary * viewDistance);
        Gizmos.DrawLine(origin, origin + leftBoundary * viewDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(origin, attackRange);

        if (viewPivot)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(origin, origin + viewPivot.right * viewDistance * 0.8f);
        }
    }
}
