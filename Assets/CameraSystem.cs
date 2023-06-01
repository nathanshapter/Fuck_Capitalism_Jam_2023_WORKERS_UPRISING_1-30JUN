using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
   [SerializeField] SecurityCamera[] cameras;

    private void Start()
    {
        cameras= GetComponentsInChildren<SecurityCamera>();
    }

    public void SetOffAlarm()
    {
        foreach (var item in cameras)
        {
            item.breachConfirmed = true;
            item.target = item.player;
        }
    }
}
