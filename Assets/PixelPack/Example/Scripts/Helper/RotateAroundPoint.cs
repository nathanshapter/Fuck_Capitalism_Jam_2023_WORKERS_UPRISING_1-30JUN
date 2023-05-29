/**
 * Rotates object around RotationCenterPoint, which can be set with an editor handle. * 
 */

using UnityEngine;

public class RotateAroundPoint : MonoBehaviour
{
    public Vector3 RotationCenterPoint;
    public float RotationSpeed = 20f;
    private Vector3 localRotationCenterPoint;

    private void Start()
    {
        localRotationCenterPoint = RotationCenterPoint + transform.position;
    }

    private void Update()
    {
        transform.RotateAround(localRotationCenterPoint, Vector3.back, RotationSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.identity;
    }
}
