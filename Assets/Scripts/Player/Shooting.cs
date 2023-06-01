using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    private Camera cam;
  public   Vector3 mousePos;
    PlayerController pc;
    public GameObject bullet;
    public Transform bulletTransform;
    public bool canFire;
    private float timer;
    public float timeBetweenFiring;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        pc = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {


        mousePos = Mouse.current.position.ReadValue();
        mousePos = cam.ScreenToWorldPoint(mousePos);

        ProcessAimingDirection();

        ProcessStoppingTime();

    }

    public void OnShoot(InputAction.CallbackContext context)
    {

        if (canFire)
        {
            canFire = false;
            Instantiate(bullet, bulletTransform.position, Quaternion.identity);

        }


    }
    private void ProcessStoppingTime()
    {
        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }
    }

    private void ProcessAimingDirection()
    {
        if (!pc.isFacingRight)
        {

            Vector3 rotation = mousePos - transform.position;


            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, rotZ);

        }
        else
        {

            Vector3 rotation = -(mousePos - transform.position);


            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }
    }

   
}
