using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject damageArea;
    private PolygonCollider2D _collider2D;
    
    [Header("Audios")]
    private AudioSource _audioSource;
    public AudioClip attack_Sfx;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        damageArea = transform.Find("DamageArea").gameObject;
        damageArea.SetActive(false);
        _collider2D = damageArea.GetComponent<PolygonCollider2D>();
        //_collider2D.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    void EnableDamageArea()
    {
        damageArea.SetActive(true);
        _audioSource.pitch = Random.Range(1f, 1.5f);
        _audioSource.PlayOneShot(attack_Sfx);
        //_collider2D.enabled = true;
        
    }

    void DisableDamageArea()
    {
        damageArea.SetActive(false);
        //_collider2D.enabled = false;
    }
    
}
