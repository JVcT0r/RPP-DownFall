using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform target; // Jogador
    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;

    [Header("Configurações de Visão")]
    public float viewDistance = 10f;
    [Range(10f, 180f)] public float viewAngle = 90f;
    public LayerMask obstacleMask;

    [Header("Comportamento")]
    public float checkInterval = 0.1f;
    public float memorySeconds = 1.25f;

    [Header("Debug")]
    public bool canSeePlayer;
    private float checkTimer;
    private float lastSeenTimer = Mathf.Infinity;

    // Direção “virtual” da frente
    private Vector2 facingDir = Vector2.right;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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
            agent.SetDestination(target.position);
        else
            agent.ResetPath();

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
