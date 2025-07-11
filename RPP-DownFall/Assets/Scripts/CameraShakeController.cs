using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    private CinemachineCamera vCam;
    private CinemachineBasicMultiChannelPerlin perlinNoise;
    private float shakeTimer;
    private void Awake()
    {
        vCam = GetComponent<CinemachineCamera>();
        perlinNoise = vCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        
        
    }

    public void ShakeCamera(float intensity, float shakeTime)
    {
       perlinNoise.AmplitudeGain = intensity;
       shakeTimer = shakeTime;
    }
    
    public void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                perlinNoise.AmplitudeGain = 0f;
            }
        }
    }

   
}
