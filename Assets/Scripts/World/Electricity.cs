using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electricity : MonoBehaviour
{
    public CameraSwitch[] camSwitch;
    CameraSystem securityCamera;


    private void Start()
    {
        camSwitch = FindObjectsOfType<CameraSwitch>();
        securityCamera= GetComponentInChildren<CameraSystem>();
    }


    public void CheckSwitches()
    {
        if(camSwitch.Length == 0)
        {
            securityCamera.TurnOffCameras();
        }
    }
}
