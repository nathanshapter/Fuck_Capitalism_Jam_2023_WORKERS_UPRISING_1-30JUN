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
    private void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
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
            StartCoroutine(Shoot());
        }
        
    }


    IEnumerator Shoot()
    {
        isFiring= true;
        yield return new WaitForSeconds(1);
        Instantiate(enemyBullet, bulletTransform.position, Quaternion.identity);
       StartCoroutine(Shoot());
    }
}
