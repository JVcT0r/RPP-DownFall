using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Vida real do Boss")]
    public int maxRealHealth = 50;
    private int currentRealHealth;
    
    public int CurrentRealHealth => currentRealHealth;

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

        if (bossDoor != null)
            bossDoor.GetComponent<BossDoorController>()?.OpenDoor();

        Invoke(nameof(DisableBoss), 0.1f); 
    }

    private void DisableBoss()
    {
        gameObject.SetActive(false);
    }

}
