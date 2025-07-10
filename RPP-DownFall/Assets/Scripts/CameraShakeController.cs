using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    private CinemachineCamera vCam;
    private CinemachineBasicMultiChannelPerlin perlinNoise;

    private void Awake()
    {
        vCam = GetComponent<CinemachineCamera>();
        perlinNoise = vCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        ResetIntensity();
        
    }

    public void ShakeCamera(float intensity, float shakeTime)
    {
        perlinNoise.AmplitudeGain = intensity;
        StartCoroutine(WaitTime(shakeTime));
    }

    IEnumerator WaitTime(float shakeTime)
    {
        yield return new WaitForSeconds(shakeTime);
        ResetIntensity();
    }

    void ResetIntensity()
    {
        perlinNoise.AmplitudeGain = 0f;
    }
}
