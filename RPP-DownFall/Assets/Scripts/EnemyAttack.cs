using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject damageArea;
    private PolygonCollider2D _collider2D;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        damageArea = GameObject.Find("DamageArea");
        _collider2D = damageArea.GetComponent<PolygonCollider2D>();
        _collider2D.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    void EnableDamageArea()
    {
        damageArea.SetActive(true);
        _collider2D.enabled = true;
        
    }

    void DisableDamageArea()
    {
        damageArea.SetActive(false);
        _collider2D.enabled = false;
    }
    
}
