using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    private Ray2D[] rays;
    [SerializeField] float maxRayDistance = 10f;
    [SerializeField] float verticalOffset = 15f;
    [SerializeField] float offsetMultiplier = 5f;

    public Transform target;
    private Transform originalTarget;
   public Transform player;

    int numberOfRays = 5;

    [SerializeField] float alarmTime = 3;

   
    [SerializeField] float rotationOffset;

   public bool breachConfirmed = false;

    CameraSystem cameraSystem;

    public bool isOn = true;

    private void Start()
    {
        rays = new Ray2D[5];
        originalTarget = target;
        player = FindObjectOfType<PlayerController>().transform;
        cameraSystem= GetComponentInParent<CameraSystem>();
        Physics2D.queriesStartInColliders = false;

    }
    private void Update()
    {
        if(!isOn) return;

        ShootRays();
        LookAtTarget();
    }
  public  bool coroutineStarted;
    RaycastHit2D hit;
    bool playerSeen;
    private void ShootRays()
    {      




        Vector2 direction = (target.transform.position- transform.position);
        Vector2[] directions = new Vector2[]
        {
            direction,
            direction + Vector2.up * verticalOffset,
            direction - Vector2.up * verticalOffset,
            direction + Vector2.up * verticalOffset *offsetMultiplier,
            direction - Vector2.up * verticalOffset *offsetMultiplier
        };


        for (int i = 0; i < rays.Length; i++)
        {
            rays[i] = new Ray2D(transform.position, directions[i]);
          hit   = Physics2D.Raycast(rays[i].origin, rays[i].direction, maxRayDistance);
            if (hit.collider != null)
            {

                
                if (hit.collider.CompareTag("Player"))
                {
                   
                    
                    if (!coroutineStarted)
                    {
                        Debug.Log("Player has been seen starting coroutine");
                        StartCoroutine(PlayerSeen());
                    }
                    
                   
                }
               
            }

        }

      

      
    }



    private IEnumerator PlayerSeen()
    {
        target = player;
        coroutineStarted = true;
        yield return new WaitForSeconds(alarmTime);
        if (isOn)
        {
            if (Vector3.Distance(player.transform.position, this.transform.position) > maxRayDistance + 1)
            {
                coroutineStarted = false;
                target = originalTarget;
                print(Vector3.Distance(player.transform.position, this.transform.position));
            }
            else
            {
                print(Vector3.Distance(player.transform.position, this.transform.position));
                print("breach confirmed");
                cameraSystem.SetOffAlarm();

                // alarm code here
            }
        } 
       
     




    }
    void LookAtTarget()
    {
        var dir = target.position - this.transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + rotationOffset;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if(Application.isPlaying )
        {
            for (int i = 0; i < rays.Length; i++)
            {
                Gizmos.DrawRay(rays[i].origin, rays[i].direction * maxRayDistance);
            }
        }
       


    }
}