using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;

public class RobotPatrol : MonoBehaviour
{
    [SerializeField] GameObject pointA, pointB;
    Rigidbody2D rb;
    private Transform currentPoint;
    private Transform player;
    public float speed = 5;
    bool chasing = false;


    [SerializeField] GameObject gun;

    [SerializeField] Transform detectionCentre;

 [SerializeField]   EnemyGun enemyGun;

    [SerializeField] float enemyAggroDistance = 25;

    Animator anim;
 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPoint = pointA.transform;
        FlipSprite();
      player = FindObjectOfType<PlayerController>().transform;
       
       gun.SetActive(false);
        anim = GetComponentInChildren<Animator>();
      
    }

    // Update is called once per frame
    void Update()
    {
       // StopChase();

        Patrol();
       
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !chasing)
        {
            StartCoroutine(StartChase());
        }
    }

    private void Patrol()
    {
        if (!chasing)
        {
            Vector2 point = currentPoint.position - transform.position;
            if (currentPoint == pointB.transform)
            {
                rb.velocity = new Vector2(speed, 0);
            }
            else
            {
                rb.velocity = new Vector2(-speed, 0);
            }

            if (Vector2.Distance(transform.position, currentPoint.position) < 1 && currentPoint == pointB.transform)
            {
                currentPoint = pointA.transform;
                FlipSprite();
            }
            if (Vector2.Distance(transform.position, currentPoint.position) < 1 && currentPoint == pointA.transform)
            {
                currentPoint = pointB.transform;
                FlipSprite();
            }
        }
    }

    private void StopChase()
    {
        if (Vector2.Distance(this.transform.position, player.position) > enemyAggroDistance)
        {
            anim.SetTrigger("PlayerGone");
            chasing = false;
            gun.SetActive(false);
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            enemyGun.gunShowing = false;
        }
    }

   
  public  void FlipSprite()
    {
        Vector3 localScale = transform.localScale;
        localScale.x /= -1f;
        transform.localScale = localScale;
    }

 
    
 
   IEnumerator StartChase()
    {
        yield return new WaitForSeconds(0);
        anim.SetTrigger("PlayerDetected");
        chasing = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        PullOutGun();

        yield return new WaitForSeconds(1);
      
    }

   
    void PullOutGun()
    {
        gun.SetActive(true);
     
    }

    private void OnDrawGizmos()
    {
        // platform gizmos

        Gizmos.DrawWireSphere(new Vector3(pointA.transform.position.x, pointA.transform.position.y), 2);
        Gizmos.DrawWireSphere(new Vector3(pointB.transform.position.x, pointB.transform.position.y), 2);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);

        //detection boxcast

        Gizmos.DrawWireSphere(this.transform.position, enemyAggroDistance);
    }
}
