using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
  [SerializeField]  RobotPatrol[] rob;

    private void Start()
    {
       
    }


    public void AllRobotsAwake()
    {
        foreach (var item in rob)
        {
            
        }
    }
}
