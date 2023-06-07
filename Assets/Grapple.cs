using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grapple : MonoBehaviour
{
    public Camera mainCamera;
    public LineRenderer lineRenderer;
    public DistanceJoint2D distanceJoint;

    private void Start()
    {
        distanceJoint.enabled = false;

        
    }
    private void Update()
    {
        if (distanceJoint.enabled)
        {
            lineRenderer.SetPosition(1, transform.position);
        }
    }
    public void GrappleStart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 mousePos = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.SetPosition(0, mousePos);
            lineRenderer.SetPosition(1, transform.position);
            distanceJoint.connectedAnchor = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
            distanceJoint.enabled = true;
            lineRenderer.enabled= true;
        }
        else if(context.canceled)
        {
            distanceJoint.enabled=false;
            lineRenderer.enabled= false;
        }
       
      
    }

   
}
