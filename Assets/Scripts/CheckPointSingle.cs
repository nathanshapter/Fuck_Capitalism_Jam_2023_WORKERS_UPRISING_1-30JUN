using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointSingle : MonoBehaviour
{

    RespawnManager respawnManager;

    private void Start()
    {
     respawnManager = FindObjectOfType<RespawnManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            respawnManager.respawnPosition = this.transform.position;
        }
    }
}
