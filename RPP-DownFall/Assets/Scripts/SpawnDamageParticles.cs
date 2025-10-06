using System;
using UnityEngine;
using UnityEngine.VFX;

public class SpawnDamageParticles : MonoBehaviour
{
    [SerializeField] private VisualEffect vfx;
    private VFXEventAttribute eventAttribute;

    private void Start()
    {
        eventAttribute = vfx.CreateVFXEventAttribute();
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayVFX();
        }
    }*/

    public void PlayBloodVFX()
    {
        vfx.SendEvent("EnemyDamaged");
    }

    public void PlayImpactVFX()
    {
        vfx.SendEvent("BulletImpact");
    }
}
