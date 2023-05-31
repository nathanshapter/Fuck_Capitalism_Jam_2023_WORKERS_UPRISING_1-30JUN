using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
 
    public float waitBeforeRespawn = 3;
    Animator animator;
   
    bool deathInProgress;
  

    [SerializeField] AudioClip deathClip;

    CanvasScript cs;
   
    PlayerController pc;
   public Vector2 respawnPosition;
    private void Start()
    {
        
        animator = GetComponentInChildren<Animator>();
      
       
        pc = GetComponent<PlayerController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {

            ProcessDeath();



        }
    }

    private IEnumerator ReturnPlayerToStart()
    {

        yield return new WaitForSeconds(waitBeforeRespawn);


       
        animator.SetTrigger("Alive");
      this.transform.position = respawnPosition;

        deathInProgress = false;
        isDying = false;

    }
    bool isDying = false;
    public void ProcessDeath()
    {
        if (!isDying)
        {
            StartCoroutine(pc.DisableControls());

            isDying = true;
            AudioManager.instance.PlaySound(deathClip);

            transform.SetParent(null);
            if (deathInProgress)
            {
                return;
            }
            deathInProgress = true;
            DeathCounter.instance.amountOfDeaths++;
         //   cs.UpdateDeathText();

            animator.SetTrigger("isDead");
            StartCoroutine(ReturnPlayerToStart());
        }

    }
    private void Update()
    {
        if (transform.position.y < -100)
        {
            ProcessDeath();
        }
    }
}
