using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraSwitch : MonoBehaviour
{
    Electricity electricity;
    BoxCollider2D box;

    private void Start()
    {
        electricity = GetComponentInParent<Electricity>();
        box = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Array.Resize(ref electricity.camSwitch, electricity.camSwitch.Length - 1);
            electricity.CheckSwitches();

           // anim.SetBool("TorchOn", true);
            
            Destroy(box);
             gameObject.SetActive(false);

        }
    }
}
