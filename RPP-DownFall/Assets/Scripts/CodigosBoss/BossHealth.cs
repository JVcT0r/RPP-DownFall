using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Vida real do Boss")]
    public int maxRealHealth = 50; 
    private int currentRealHealth;

    [Header("Sistema de Stagger")]
    public int pistolHitsNeeded = 10;
    public int shotgunHitsNeeded = 3;

    private int pistolCounter = 0;
    private int shotgunCounter = 0;

    public float staggerTime = 2f; 
    public bool isStaggered = false;

    [Header("Referências")]
    private BossAI bossAI;
    public SpawnDamageParticles Particles;

    [Header("Porta que abre ao morrer")]
    public string doorTagToOpen = "BossDoor";
    public float doorMoveDistance = 2f;
    public float doorMoveTime = 0.5f;

    private void Awake()
    {
        currentRealHealth = maxRealHealth;
        bossAI = GetComponent<BossAI>();
    }

    public void TakeHitFromGun(bool isShotgun)
    {
        if (isStaggered)
        {
            currentRealHealth--;
            Particles?.PlayBloodVFX();

            if (currentRealHealth <= 0)
                Die();

            return;
        }

        if (isShotgun) shotgunCounter++;
        else pistolCounter++;

        if (pistolCounter >= pistolHitsNeeded || shotgunCounter >= shotgunHitsNeeded)
            EnterStagger();
    }

    private void EnterStagger()
    {
        isStaggered = true;
        pistolCounter = 0;
        shotgunCounter = 0;

        bossAI?.EnterStaggerState();
        Invoke(nameof(ExitStagger), staggerTime);
    }

    private void ExitStagger()
    {
        isStaggered = false;
        bossAI?.ExitStaggerState();
    }

    private void Die()
    {
        bossAI?.OnBossDeath();
        OpenBossDoor();
        gameObject.SetActive(false);
    }

    // -------------------- ABERTURA LATERAL DA PORTA --------------------
    private void OpenBossDoor()
    {
        GameObject doorObj = GameObject.FindGameObjectWithTag(doorTagToOpen);
        if (doorObj == null)
        {
            Debug.LogWarning("[BossHealth] Porta com tag " + doorTagToOpen + " não encontrada!");
            return;
        }

        Transform door = doorObj.transform;
        Vector3 start = door.localPosition;
        Vector3 target = start + new Vector3(doorMoveDistance, 0, 0);

        doorObj.AddComponent<TempDoorMover>().Move(start, target, doorMoveTime);
    }
}

// -------------------------------------------------------------------
//       Faz a porta deslizar e depois se auto-destrói
// -------------------------------------------------------------------
public class TempDoorMover : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 targetPos;
    private float moveTime;

    public void Move(Vector3 start, Vector3 target, float time)
    {
        startPos = start;
        targetPos = target;
        moveTime = time;
        StartCoroutine(MoveRoutine());
    }

    private System.Collections.IEnumerator MoveRoutine()
    {
        float t = 0f;

        while (t < moveTime)
        {
            transform.localPosition = Vector3.Lerp(startPos, targetPos, t / moveTime);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPos;

        Destroy(this);
    }
}
