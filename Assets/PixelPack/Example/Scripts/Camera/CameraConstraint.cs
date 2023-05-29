/**
 *  CameraConstraint is used by CameraFollow class to constraint the movement of camera.
 *  It uses BoxCollider2D size to constraint the camera movement.
 */

using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraConstraint : MonoBehaviour
{
    public bool StartRect = false; /**< Starting rect. Set true for constraint that contains PlayerSpawnPoint */

    private BoxCollider2D boxCollider2D;
    private void OnDrawGizmos()
    {
        if (transform.localScale == Vector3.one)
        {
            boxCollider2D = GetComponent<BoxCollider2D>();
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            Gizmos.DrawWireCube(boxCollider2D.transform.position, new Vector3(boxCollider2D.size.x, boxCollider2D.size.y, .1f));

        }
        else
            Debug.LogError("Scale the collider, not object!");
    }
}
