using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoForward : MonoBehaviour
{
    public float bulletSpeed = 100.0f;
    public int damage;

    Rigidbody2D rb;
    public Enemy shooter;

    void Start()
    {
      rb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
      rb.velocity = transform.up * bulletSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
            shooter.damageDone += damage;
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
        }

        Debug.Log(collision.gameObject.name);
        Destroy(this.gameObject);
    }
}
