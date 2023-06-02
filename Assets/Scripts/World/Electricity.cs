using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class Electricity : MonoBehaviour
{
    public CameraSwitch[] camSwitch;
    CameraSystem securityCamera;
    EnemySpawnPoints esp;


    private void Start()
    {
        camSwitch = FindObjectsOfType<CameraSwitch>();
        securityCamera= GetComponentInChildren<CameraSystem>();
        esp = GetComponentInChildren<EnemySpawnPoints>();
    }


    public void CheckSwitches()
    {
        if(camSwitch.Length == 0)
        {
            securityCamera.TurnOffCameras();

            // spawn more enemies
            int number = -1;
            foreach (var item in esp.spawnpoints)
            {
                
                print(esp.spawnpoints[number +=1].transform.position);
               
            }
        }
    }
}
