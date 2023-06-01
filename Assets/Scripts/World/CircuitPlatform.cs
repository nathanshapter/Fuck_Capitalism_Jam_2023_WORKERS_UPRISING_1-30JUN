using System.Collections;
using UnityEngine;



public class CircuitPlatform : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;



    [Header("Mandatory")]
    [Space(20)]
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float dockTimer;
    [SerializeField] float dockDecelerationSpeed = 10f;
    [SerializeField] float dockAccelerationSpeed = 5f;
   
    [Space(20)]
    [Header("Optional")]
    [SerializeField] float dockStoppingDistance = 1f;
    [SerializeField] float delayTime;
    [SerializeField] bool reverseDirection;




    // used as counters
    bool delay;
    Vector2 startPosition;
    float startingSpeed;
    int index = 0;
    int gizmoNumber = 0;
    bool coroutineStarted = false;


    [SerializeField] bool activatedByTouch;
    bool canMove;
    [SerializeField] bool stopsMovingWhenNotTouched;










    private void Start()
    {
        canMove = true;
        startPosition = transform.position;
        startingSpeed = movementSpeed;

        StartCoroutine(PausePlatform(delayTime));

        if (reverseDirection)
        {
            System.Array.Reverse(waypoints);
        }

        if (activatedByTouch) { canMove = false; }
    }
    private void Update()
    {
        this.transform.Rotate(0, 0, 0);


        if (delay || !canMove) { return; }

        CalculateNextWaypoint();

        float distance = Vector2.Distance(waypoints[index].position, transform.position);

        CalculateSpeed(distance);








    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canMove = true;
        }
        if (collision.gameObject.CompareTag("Box"))
        {
            transform.SetParent(collision.transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (stopsMovingWhenNotTouched)
        {
            canMove = false;
        }
        transform.SetParent(null);
        this.transform.Rotate(0, 0, 0);
    }

   


    private void CalculateSpeed(float distance)
    {
        if (distance <= dockStoppingDistance)
        {
            if (dockTimer >= dockStoppingDistance)
            {
                movementSpeed = distance * dockDecelerationSpeed;


            }
            if (!coroutineStarted)
                StartCoroutine(IncreaseIndex());


        }
        else if (distance > dockStoppingDistance)
        {
            movementSpeed += dockAccelerationSpeed * Time.deltaTime;

            if (movementSpeed >= startingSpeed)
            {
                movementSpeed = startingSpeed;
            }
        }
    }

    private void CalculateNextWaypoint()
    {
        if (index >= waypoints.Length)
        {
            index = 0;
        }
        else
        {
            Vector2 newPos = Vector2.MoveTowards(transform.position, waypoints[index].transform.position, movementSpeed * Time.deltaTime);
            transform.position = newPos;
        }
    }


    IEnumerator IncreaseIndex()
    {

        coroutineStarted = true;
        yield return new WaitForSeconds(dockTimer);
        index++;
        coroutineStarted = false;

    }

    IEnumerator PausePlatform(float delayTime)
    {
        delay = true;
        yield return new WaitForSeconds(delayTime);
        delay = false;
    }


    private void OnDrawGizmos()
    {
        if (index == 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, waypoints[0].transform.position);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(waypoints[waypoints.Length - 1].transform.position, waypoints[0].transform.position);

        foreach (var item in waypoints)
        {

            if (gizmoNumber + 1 >= waypoints.Length)
            {

                gizmoNumber = 0;

            }
            Gizmos.DrawLine(waypoints[gizmoNumber].position, waypoints[gizmoNumber + 1].transform.position);
            gizmoNumber++;
        }





    }



}
