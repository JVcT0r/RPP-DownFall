using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Vida real do Boss")]
    public int maxRealHealth = 50;
    private int currentRealHealth;

    [Header("Sistema de Stagger (desmaio)")]
    public int pistolHitsNeeded = 10;
    public int shotgunHitsNeeded = 3;

    private int pistolCounter = 0;
    private int shotgunCounter = 0;

    public float staggerTime = 2f;
    public bool isStaggered = false;

    private BossAI bossAI;

    [Header("Referências de VFX (opcional)")]
    public SpawnDamageParticles Particles;

    [Header("Porta do Boss")]
    public Transform bossDoor;          
    public float doorMoveDistance = 30f; 
    public float doorMoveTime = 0.5f;   
    public bool doorMovesRight = true;  

    private void Awake()
    {
        currentRealHealth = maxRealHealth;
        bossAI = GetComponent<BossAI>();
    }

    // --------------------------------------------------------------------
    //   RECEBE DANO (DE PISTOLA OU SHOTGUN)
    // --------------------------------------------------------------------
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

    // --------------------------------------------------------------------
    //   ENTRA NO STUN
    // --------------------------------------------------------------------
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

    // --------------------------------------------------------------------
    //   MORRE
    // --------------------------------------------------------------------
    private void Die()
    {
        bossAI?.OnBossDeath();

        OpenDoor(); 
        
        Invoke(nameof(DisableBoss), 0.2f);
    }

    private void DisableBoss()
    {
        gameObject.SetActive(false);
    }

    // --------------------------------------------------------------------
    //   ABRE A PORTA
    // --------------------------------------------------------------------
    private void OpenDoor()
    {
        if (bossDoor == null)
        {
            Debug.LogWarning("BossHealth: Nenhuma porta atribuída no inspector!");
            return;
        }

        Vector3 start = bossDoor.position;
        Vector3 target;

        if (doorMovesRight)
            target = start + new Vector3(doorMoveDistance, 0, 0);
        else
            target = start + new Vector3(-doorMoveDistance, 0, 0);

        StartCoroutine(OpenDoorRoutine(start, target));
    }

    private System.Collections.IEnumerator OpenDoorRoutine(Vector3 start, Vector3 target)
    {
        float t = 0;

        while (t < doorMoveTime)
        {
            bossDoor.position = Vector3.Lerp(start, target, t / doorMoveTime);
            t += Time.deltaTime;
            yield return null;
        }

        bossDoor.position = target;
    }
}
