using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;  

public class EnemyAI : MonoBehaviour
{
    private static readonly int IsFollowing = Animator.StringToHash("IsFollowing");

    [Header("Referências")]
    [SerializeField] private Transform target;
    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;

    [Header("Configurações de Visão")]
    public float viewDistance = 10f;
    [Range(10f, 180f)] public float viewAngle = 90f;
    public LayerMask obstacleMask;

    [Header("Comportamento")]
    public float checkInterval = 0.1f;
    public float memorySeconds = 1.25f;
    public float hitAwarenessTime = 1.5f;

    [Header("Debug")]
    public bool canSeePlayer;

    private float checkTimer;
    private float lastSeenTimer = Mathf.Infinity;
    private float hitTimer = Mathf.Infinity;

    private Vector2 facingDir;
    private Animator _animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        if (target == null)
        {
            Player p = FindAnyObjectByType<Player>();
            if (p != null)
                target = p.transform;
        }

        // ------------------------------------------
        // 🔥 ADICIONADO: olhar para o player SOMENTE na Fase 3
        // ------------------------------------------
        bool isPhase3 = SceneManager.GetActiveScene().name == "Fase3";

        if (isPhase3 && target != null)
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

        hitTimer += Time.deltaTime;

        // Intervalo de checagem
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            canSeePlayer = CheckPlayerInVision();
            if (canSeePlayer)
                lastSeenTimer = 0f;
        }

        if (!canSeePlayer)
            lastSeenTimer += Time.deltaTime;

        bool shouldChase = canSeePlayer || lastSeenTimer <= memorySeconds || hitTimer <= hitAwarenessTime;

        if (shouldChase)
        {
            agent.SetDestination(target.position);
            _animator.SetBool(IsFollowing, true);
        }
        else
        {
            agent.ResetPath();
            _animator.SetBool(IsFollowing, false);
        }

        UpdateFacingDirection();
    }

    private void UpdateFacingDirection()
    {
        Vector2 dirToPlayer = ((Vector2)target.position - (Vector2)transform.position).normalized;

        if (canSeePlayer || hitTimer <= hitAwarenessTime)
        {
            facingDir = Vector2.Lerp(facingDir, dirToPlayer, Time.deltaTime * 6f);
        }
        else if (agent.desiredVelocity.sqrMagnitude > 0.1f)
        {
            facingDir = Vector2.Lerp(facingDir, agent.desiredVelocity.normalized, Time.deltaTime * 4f);
        }

        spriteRenderer.flipX = facingDir.x < 0;
    }

    private bool CheckPlayerInVision()
    {
        Vector2 origin = transform.position;
        Vector2 toPlayer = (target.position - transform.position);
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

    // ---------------------------------
    // 🔥 Reagir ao tiro do player
    // ---------------------------------
    public void OnHitByPlayer()
    {
        hitTimer = 0f;
        lastSeenTimer = 0f;
        canSeePlayer = true;

        if (target != null)
        {
            Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
            facingDir = dir;
            spriteRenderer.flipX = facingDir.x < 0;
        }

        agent.SetDestination(target.position);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = canSeePlayer ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Quaternion rotRight = Quaternion.Euler(0, 0, viewAngle / 2);
        Quaternion rotLeft = Quaternion.Euler(0, 0, -viewAngle / 2);

        Vector2 dir = facingDir.normalized;

        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(rotRight * dir) * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(rotLeft * dir) * viewDistance);
    }
}
