using UnityEngine;
using UnityEngine.VFX;

public class SpawnDamageParticles : MonoBehaviour
{
    [SerializeField] private VisualEffectAsset _damageParticle;
    [SerializeField] private float _particleOffsetDistance = 0.5f;
    
    
    
    private BoxCollider2D _boxCollider2D;
    
    void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public void SpawnDamageParticle(Vector3 spawnPosition, Vector3 dir)
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, dir);
        Instantiate(_damageParticle, spawnPosition + new Vector3(dir.x * (_boxCollider2D.bounds.extents.x * _particleOffsetDistance), 
            dir.y * (_boxCollider2D.bounds.extents.y * _particleOffsetDistance)), rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
