using System.Globalization;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    private Ray2D[] rays;
    [SerializeField] float maxRayDistance = 10f;
    [SerializeField] float verticalOffset = 15f;
    [SerializeField] float offsetMultiplier = 5f;

    [SerializeField] Transform target;

    int numberOfRays = 5;
    private void Start()
    {
        rays = new Ray2D[5];
        
    }
    private void Update()
    {
        ShootRays();
    }

    private void ShootRays()
    {

       




        Vector2 direction = (target.transform.position- transform.position);
        Vector2[] directions = new Vector2[]
        {
            direction,
            direction + Vector2.up * verticalOffset,
            direction - Vector2.up * verticalOffset,
            direction + Vector2.up * verticalOffset *offsetMultiplier,
            direction + Vector2.up * verticalOffset *offsetMultiplier
        };


        for (int i = 0; i < rays.Length; i++)
        {
            rays[i] = new Ray2D(transform.position, directions[i]);
            RaycastHit2D hit = Physics2D.Raycast(rays[i].origin, rays[i].direction, maxRayDistance);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Player");
                }
            }

        }

      

      
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