using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{

    Transform target;
    float rotationOffset;
  [SerializeField]  GameObject enemyBullet;
    [SerializeField] Transform bulletTransform;
    public bool isFiring = false;
    SpriteRenderer gunSprite;
    [SerializeField] float waitForGunEnable = 3;
    [SerializeField] float timeBetweenBullets =1;
    private void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
        gunSprite = GetComponent<SpriteRenderer>();
        gunSprite.enabled = false;
    }

    void Update()
    {
        LookAtTarget();
    }
   
    void LookAtTarget()
    {
        var dir = target.position - this.transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + rotationOffset;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        if(!isFiring)
        {
            StartCoroutine(EnableGun());
        }
        
    }


    IEnumerator EnableGun()
    {

        yield return new WaitForSeconds(waitForGunEnable);
        gunSprite.enabled = true;
        isFiring= true;
      
       StartCoroutine(Shoot());
    }
    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(timeBetweenBullets);
        Instantiate(enemyBullet, bulletTransform.position, Quaternion.identity);
        StartCoroutine(Shoot());
    }
    
}
