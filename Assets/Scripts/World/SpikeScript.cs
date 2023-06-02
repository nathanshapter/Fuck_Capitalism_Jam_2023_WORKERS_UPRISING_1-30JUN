using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    public bool instaKill = false;
  

    
    [SerializeField] int damage = 20;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") )
        {
            if(!instaKill)
            {
                collision.gameObject.transform.SetParent(null);

                FindObjectOfType<PlayerHealth>().TakeDamage(damage);
                StartCoroutine(FindObjectOfType<RespawnManager>().ReturnPlayerToStart());
            }

           
        }
    }
}
