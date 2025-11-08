using UnityEngine;
using UnityEngine.AI;

public class EnemyShooter : MonoBehaviour
{
    private static readonly int IsShooting = Animator.StringToHash("IsShooting");

    [Header("Referências")]
    [SerializeField] private Transform firePoint;        // ponto de onde as balas saem
    [SerializeField] private GameObject bulletPrefab;    // prefab da bala
    [SerializeField] private Transform viewPivot;        // define a direção do cone (opcional)
    private Transform target;
    private NavMeshAgent agent;
    
    private SpriteRenderer spriteRenderer;
    
    private Vector2 facingDir = Vector2.right;
    private Animator _animator;


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

        if (firePoint == null)
            Debug.LogWarning($"[EnemyShooter] FirePoint não atribuído em {gameObject.name}");
    }

    
    private void Update()
    {
        if (target == null) return;

        // Atualiza a frente com base na movimentação e no player
        UpdateFacingDirection();

        // Faz checagem de visão
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
            _animator.SetBool("IsShooting", true);
            TryShoot();
        }

        else
        {
            agent.ResetPath();
            _animator.SetBool("IsShooting", false);
        }

            

        // Flip visual
        if (spriteRenderer != null)
        {
            if (facingDir.x > 0.05f) spriteRenderer.flipX = false;
            else if (facingDir.x < -0.05f) spriteRenderer.flipX = true;
        }
    }

    private void UpdateFacingDirection()
    {
        Vector2 dirToPlayer = ((Vector2)target.position - (Vector2)transform.position).normalized;

        // Se o player estiver visível, olhe diretamente para ele
        if (canSeePlayer)
        {
            facingDir = Vector2.Lerp(facingDir, dirToPlayer, Time.deltaTime * 10f);
        }
        else if (agent.desiredVelocity.sqrMagnitude > 0.001f)
        {
            // Caso contrário, siga a direção do movimento do NavMesh
            facingDir = Vector2.Lerp(facingDir, agent.desiredVelocity.normalized, Time.deltaTime * 5f);
        }
    }

    private bool CheckPlayerInVision()
    {
        Vector2 origin = transform.position;
        Vector2 toPlayer = ((Vector2)target.position - origin);
        float dist = toPlayer.magnitude;

        if (dist > viewDistance) return false;

        Vector2 dirToPlayer = toPlayer / (dist > 0.001f ? dist : 1f);
        float angle = Vector2.Angle(facingDir, dirToPlayer);
        if (angle > viewAngle * 0.5f) return false;

        // Bloqueio de obstáculos
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
            rb.linearVelocity = firePoint.transform.up * bulletSpeed;
        }

        Destroy(bullet, 3f);
    }


    private void OnDrawGizmos()
    {
        // Mostra alcance
        Gizmos.color = canSeePlayer ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        // Direção atual do inimigo
        Vector2 dir = facingDir.sqrMagnitude < 0.001f ? Vector2.right : facingDir.normalized;

        // Desenha cone
        Vector3 origin = transform.position;
        Quaternion qRight = Quaternion.Euler(0, 0, viewAngle * 0.5f);
        Quaternion qLeft = Quaternion.Euler(0, 0, -viewAngle * 0.5f);

        Vector3 rightBound = qRight * (Vector3)dir;
        Vector3 leftBound = qLeft * (Vector3)dir;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
        Gizmos.DrawLine(origin, origin + rightBound * viewDistance);
        Gizmos.DrawLine(origin, origin + leftBound * viewDistance);
    }
}
