using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    private Camera cam;
    private Vector3 mousePos;
    PlayerController pc;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        pc = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        print(pc.isFacingRight);

        if (!pc.isFacingRight)
        {
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            Vector3 rotation = mousePos - transform.position;


            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, rotZ);

        }
        else
        {
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            Vector3 rotation = -(mousePos - transform.position);


            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }









    }
}
