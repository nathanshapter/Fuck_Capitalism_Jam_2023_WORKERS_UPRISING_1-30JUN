using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
   public int health = 100;
   [SerializeField] int spikeDamage = 20;
    RespawnManager rm;
    Rigidbody2D rb;

    private void Start()
    {
        rm = GetComponent<RespawnManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            if(collision.gameObject.GetComponent<SpikeScript>().instaKill) 
            {
                rm.ProcessDeath();
            }
            else
            {
               

                if (!IsAlive())
                {
                    rm.ProcessDeath();
                }
            }

           
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            rb.AddForce(new Vector2(500,0));
        }
    }

    bool IsAlive()
    {
        if(health > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void TakeDamage(int damage)
    {
        transform.SetParent(null);
        health-= damage;
        print($"you have {health} now");
    
    }
}
