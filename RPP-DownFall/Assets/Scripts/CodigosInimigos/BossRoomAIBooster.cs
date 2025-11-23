using UnityEngine;
using UnityEngine.SceneManagement;

public class BossRoomAIBooster : MonoBehaviour
{
    [Header("Nome da cena onde o boost será aplicado")]
    public string bossSceneName = "Fase3";   

    [Header("Valores especiais para inimigos da fase 3")]
    public float boostedViewDistance = 20f;
    public float boostedViewAngle = 160f;

    public float boostedMoveSpeed = 5.5f; 

    private void Start()
    {
       
        if (SceneManager.GetActiveScene().name != bossSceneName)
            return;

        
        EnemyAI[] meleeEnemies = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        foreach (var enemy in meleeEnemies)
        {
            enemy.viewDistance = boostedViewDistance;
            enemy.viewAngle = boostedViewAngle;

            enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = boostedMoveSpeed;
        }

        
        EnemyShooter[] shooters = FindObjectsByType<EnemyShooter>(FindObjectsSortMode.None);
        foreach (var enemy in shooters)
        {
            enemy.viewDistance = boostedViewDistance;
            enemy.viewAngle = boostedViewAngle;

            enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = boostedMoveSpeed;
        }

        Debug.Log("Boost de visão aplicado nos inimigos da Fase 3!");
    }
}