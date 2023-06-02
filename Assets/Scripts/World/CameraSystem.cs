using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
   [SerializeField] SecurityCamera[] cameras;
    EnemySpawnPoints esp;


    private void Start()
    {
        cameras= GetComponentsInChildren<SecurityCamera>();
        esp = FindObjectOfType<EnemySpawnPoints>();
    }

    public void SetOffAlarm()
    {
        foreach (var item in cameras)
        {
            item.breachConfirmed = true;
            item.target = item.player;



        }

        int number = -1;
        foreach (var item in esp.spawnpoints)
        {

            print(esp.spawnpoints[number += 1].transform.position);

        }
    }

    public void TurnOffCameras()
    {
        foreach (var item in cameras) 
        {
            item.isOn = false;
        }
        print("cameras have been turned off");
    }
    
}
