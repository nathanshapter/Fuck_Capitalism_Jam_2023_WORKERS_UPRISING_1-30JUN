using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{

    Transform target;
    float rotationOffset;
  [SerializeField]  GameObject enemyBullet;
    [SerializeField] Transform bulletTransform;

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
        StartCoroutine(Shoot());
    }


    IEnumerator Shoot()
    {
        Instantiate(enemyBullet, bulletTransform.position, Quaternion.identity);
        yield return new WaitForSeconds(1);
    }
}
