using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    [Header("Referências de Ataque")]
    public Transform firePointSlash;
    public GameObject slashProjectilePrefab;
    public float slashSpeed = 14f;

    [Header("Ataque Melee")]
    public float meleeRange = 2f;
    public float meleeDamage = 1f;

    private Transform target;
    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private BossHealth bossHealth;

    private Vector2 facingDir;

    [Header("Cooldown de Ataque")]
    public float attackCooldown = 2f;
    private float nextAttackTime;

    [Header("Movimento")]
    public float moveSpeed = 4f;

    private bool isStaggered = false;

    // ------------------------- VISÃO -------------------------------
    [Header("Configurações de Visão")]
    public float viewDistance = 8f;
    [Range(10f, 180f)] public float viewAngle = 90f;
    public LayerMask obstacleMask;

    [Header("Debug Visão")]
    public bool canSeePlayer;
    public float checkInterval = 0.1f;
    private float checkTimer = 0f;
    // ----------------------------------------------------------------

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        bossHealth = GetComponent<BossHealth>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        Player p = FindAnyObjectByType<Player>();
        if (p != null)
            target = p.transform;

        agent.speed = moveSpeed;
    }

    private void Update()
    {
        if (target == null) return;

        // -------------------- VISÃO --------------------
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            canSeePlayer = CheckPlayerInVision();
        }
        // ------------------------------------------------

        if (isStaggered)
        {
            agent.ResetPath();
            anim.SetBool("IsMoving", false);
            return;
        }

        
        Vector2 dir = (target.position - transform.position).normalized;
        facingDir = dir;

        
        if (facingDir.x > 0) spriteRenderer.flipX = false;
        else spriteRenderer.flipX = true;

        
        float angle = Mathf.Atan2(facingDir.y, facingDir.x) * Mathf.Rad2Deg;
        firePointSlash.rotation = Quaternion.Euler(0, 0, angle);

        
        if (canSeePlayer)
        {
            agent.SetDestination(target.position);
            anim.SetBool("IsFollowing", true);
        }
        else
        {
            agent.ResetPath();
            anim.SetBool("IsFollowing", false);
        }

        float dist = Vector2.Distance(transform.position, target.position);

        if (Time.time >= nextAttackTime && canSeePlayer)
        {
            if (dist <= meleeRange)
                MeleeAttack();
            else
                RangedSlash();

            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // -------------------- ATAQUE MELEE --------------------
    private void MeleeAttack()
    {
        anim.SetTrigger("Melee");

        Collider2D hit = Physics2D.OverlapCircle(firePointSlash.position, meleeRange);
        if (hit != null && hit.CompareTag("Player"))
        {
            Player p = hit.GetComponent<Player>();
            if (p != null)
                p.TakeDamage((int)meleeDamage);
        }
    }

    // -------------------- ATAQUE À DISTÂNCIA --------------------
    private void RangedSlash()
    {
        anim.SetTrigger("SlashShoot");

        if (slashProjectilePrefab != null)
        {
            GameObject proj = Instantiate(
                slashProjectilePrefab,
                firePointSlash.position,
                firePointSlash.rotation
            );

            
            foreach (var colBoss in GetComponentsInChildren<Collider2D>())
            {
                foreach (var colProj in proj.GetComponentsInChildren<Collider2D>())
                {
                    Physics2D.IgnoreCollision(colProj, colBoss);
                }
            }

            
            if (proj.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.linearVelocity = firePointSlash.right * slashSpeed;
            }

            
            proj.transform.position += firePointSlash.right * 0.25f;

            Destroy(proj, 4f);
        }
    }

    // -------------------- VISÃO --------------------
    private bool CheckPlayerInVision()
    {
        if (target == null) return false;

        Vector2 origin = transform.position;
        Vector2 toPlayer = (target.position - transform.position);
        float dist = toPlayer.magnitude;

        if (dist > viewDistance) return false;

        Vector2 dirToPlayer = toPlayer.normalized;
        float angle = Vector2.Angle(facingDir, dirToPlayer);
        if (angle > viewAngle * 0.5f) return false;

        if (Physics2D.Raycast(origin, dirToPlayer, dist, obstacleMask))
            return false;

        return true;
    }

    // -------------------- STAGGER --------------------
    public void EnterStaggerState()
    {
        isStaggered = true;
        anim.SetTrigger("Stunned");
        agent.ResetPath();
    }

    public void ExitStaggerState()
    {
        isStaggered = false;
    }

    // -------------------- MORTE --------------------
    public void OnBossDeath()
    {
        anim.SetTrigger("Die");
        agent.enabled = false;
    }

    // -------------------- GIZMOS --------------------
    private void OnDrawGizmos()
    {
        Vector3 origin = Application.isPlaying
            ? (Vector3)transform.position
            : transform.position;

        Gizmos.color = canSeePlayer ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(origin, viewDistance);

        Vector2 dir = Application.isPlaying
            ? (facingDir.sqrMagnitude < 0.01f ? (Vector2)transform.right : facingDir.normalized)
            : (Vector2)transform.right;

        Quaternion qRight = Quaternion.Euler(0, 0, viewAngle * 0.5f);
        Quaternion qLeft = Quaternion.Euler(0, 0, -viewAngle * 0.5f);

        Vector3 rightBound = qRight * dir;
        Vector3 leftBound = qLeft * dir;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
        Gizmos.DrawLine(origin, origin + rightBound * viewDistance);
        Gizmos.DrawLine(origin, origin + leftBound * viewDistance);
    }
}
