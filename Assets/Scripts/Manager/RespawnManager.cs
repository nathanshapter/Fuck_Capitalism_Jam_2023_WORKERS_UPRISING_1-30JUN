using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RespawnManager : MonoBehaviour
{
 
    public float waitBeforeRespawn = 3;
    Animator animator;   
    bool deathInProgress; 

    [SerializeField] AudioClip deathClip;

    CanvasScript cs;
   
    PlayerController pc;
   public Vector2 respawnPosition;
    PlayerHealth health;
    PlayerInput playerInput;

    bool isDying = false;
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
      
       health= GetComponent<PlayerHealth>();
        pc = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (transform.position.y < -100)
        {
            ProcessDeath();
        }
    }

    public IEnumerator ReturnPlayerToStart()
    {
        playerInput.enabled= false;
        yield return new WaitForSeconds(waitBeforeRespawn);       
        animator.SetTrigger("Alive");
      this.transform.position = respawnPosition;
        deathInProgress = false;
        if (isDying)
        {
            health.health = 100;
        }
        isDying = false;
        playerInput.enabled = true;

    }
  
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
   
}
