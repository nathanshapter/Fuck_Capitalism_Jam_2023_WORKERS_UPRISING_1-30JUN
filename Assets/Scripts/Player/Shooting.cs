using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
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

    [SerializeField] ParticleSystem bulletFire;
    [SerializeField] SpriteRenderer gunSprite;
    public int ammo =30;
   public int magazines;
    public  int ammoMax = 30;
    [SerializeField] float camShakeIntensity;
    [SerializeField] float camShakeTime = .1f;

    [SerializeField] private AudioClip fire, reload, empty;

    CanvasScript cs;
    private void Start()
    {
       cs = FindObjectOfType<CanvasScript>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        pc = GetComponentInParent<PlayerController>();
        ammo = ammoMax;
        magazines = 1;
        gunSprite = GetComponentInChildren<SpriteRenderer>();
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
        if(context.performed)
        {
            if (canFire && ammo > 0)
            {
                AudioManager.instance.PlaySound(fire, 1);
                CinemachineShake.instance.ShakeCamera(camShakeIntensity, camShakeTime);
                canFire = false;
                Instantiate(bullet, bulletTransform.position, Quaternion.identity);
                ammo -= 1;
                cs.UpdateAmmoText();
                bulletFire.Emit(1);

            }
            else if (ammo == 0)
            {
                AudioManager.instance.PlaySound(empty, 1);
                // play empty ammo sound
                if (magazines > 0)
                {
                    print("you could reload here");
                }
            }
        }
       
        


    }
    bool isReloading = false;
    public void Reload(InputAction.CallbackContext context)
    {
        if(ammo != ammoMax && magazines >=1 && !isReloading)
        {
            AudioManager.instance.PlaySound(reload, 1);
            isReloading = true;
            StartCoroutine(ReloadRoutine());
        }
    }
    private IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(0.5f); // shall be animation time
        magazines -= 1;
        ammo = ammoMax;
        isReloading = false;
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
