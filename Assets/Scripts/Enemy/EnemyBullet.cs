using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    public float force;
    int bulletDamage = 15;

    Transform player;
    PlayerHealth playerHealth;

    [SerializeField] float yOffset = 1.2f;
    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerController>().transform;

        ProcessDirection();

    }

    private void ProcessDirection()
    {
        Vector3 direction = player.position - transform.position;
        Vector3 rotation = transform.position - player.position;
        rb.velocity = new Vector2(direction.x, direction.y + yOffset).normalized * force;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerHealth.TakeDamage(bulletDamage);
            
        }
        Destroy(this.gameObject);
    }
}
