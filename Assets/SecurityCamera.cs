using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    private Ray2D ray;
    private float maxRayDistance = 10f;

    [SerializeField] Transform target;

    private void Update()
    {
        ShootRay();
    }

    private void ShootRay()
    {
        ray = new Ray2D(transform.position, target.transform.position - transform.position);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, maxRayDistance);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(ray.origin, (target.transform.position - transform.position) * maxRayDistance);
        Gizmos.color = Color.black;
        
        
    }
}