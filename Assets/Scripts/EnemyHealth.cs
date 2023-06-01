using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
  [SerializeField]  int health;

    public void TakeDamageEnemy(int damage)
    {
        health -= damage;
        print(health);
        if (!CheckIfAlive())
        {
            Destroy(gameObject);
        }
    }

    bool CheckIfAlive()
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

    
}
