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



    [SerializeField] GameObject gun;

    [SerializeField] Transform detectionCentre;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPoint = pointA.transform;
        FlipSprite();
      player = FindObjectOfType<PlayerController>().transform;
       
       gun.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (chasing) { return; }
     


      

        Vector2 point = currentPoint.position -transform.position;
        if(currentPoint == pointB.transform)
        {
            rb.velocity = new Vector2(speed, 0);
        }
        else
        {
            rb.velocity = new Vector2(-speed, 0);
        }
      
        if(Vector2.Distance(transform.position, currentPoint.position) < 1 && currentPoint == pointB.transform)
        {
            currentPoint = pointA.transform;
            FlipSprite();
        }
        if(Vector2.Distance(transform.position, currentPoint.position) < 1 && currentPoint == pointA.transform)
        {
            currentPoint = pointB.transform;
            FlipSprite();
        }
    }
  void FlipSprite()
    {
        Vector3 localScale = transform.localScale;
        localScale.x /= -1f;
        transform.localScale = localScale;
    }

    private void OnDrawGizmos()
    {
        // platform gizmos
        
        Gizmos.DrawWireSphere(new Vector3(pointA.transform.position.x, pointA.transform.position.y), 2);
        Gizmos.DrawWireSphere(new Vector3(pointB.transform.position.x, pointB.transform.position.y), 2);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);

        //detection boxcast

  
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(StartChase());
        }
    }
    bool chasing = false;
    IEnumerator StartChase()
    {
        yield return new WaitForSeconds(1);
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        chasing = true;
        PullOutGun();

        yield return new WaitForSeconds(1);
        StartCoroutine(StartShooting());
    }

    IEnumerator StartShooting()
    {
        yield return new WaitForSeconds(1);
       

    }
    void PullOutGun()
    {
        gun.SetActive(true);
        // animation here
        print("gun animation here");
    }
}
