using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    public static CinemachineShake instance { get; private set; }

    float shakeTimer;
    private void Awake()
    {
       virtualCamera =  GetComponent<CinemachineVirtualCamera>();
        instance= this;
    }

   public void ShakeCamera(float intensity, float timer)
    {
       CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain= intensity;
        shakeTimer= timer;
    }
    private void Update()
    {
        if(shakeTimer >0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {

                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                     virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            }
        }
       
    }
}
