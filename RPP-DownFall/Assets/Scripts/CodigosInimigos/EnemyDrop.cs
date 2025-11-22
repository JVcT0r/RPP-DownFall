using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [Header("Chances de Drop")]
    [Range(0f, 1f)] public float chanceDropPistola = 0.30f;   
    [Range(0f, 1f)] public float chanceDropShotgun = 0.30f;   
    [Range(0f, 1f)] public float chanceDropPocao = 0.10f;     

    [Header("Prefabs")]
    public GameObject dropPistola;
    public GameObject dropShotgun;
    public GameObject dropPocao;

    [Header("Ajustes")]
    public float alturaDrop = 0.2f;

    public void DroparItens()
    {
        Vector3 pos = transform.position + Vector3.up * alturaDrop;

        
        if (Random.value <= chanceDropPistola && dropPistola != null)
            Instantiate(dropPistola, pos, Quaternion.identity);

        
        if (Player.hasShotgun && Random.value <= chanceDropShotgun && dropShotgun != null)
            Instantiate(dropShotgun, pos, Quaternion.identity);

        
        if (Random.value <= chanceDropPocao && dropPocao != null)
            Instantiate(dropPocao, pos, Quaternion.identity);
    }
}