using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public SpawnEnemies spawner;
    public int round = 0;
    public float startTime;

    public bool dead = false;
    public int hp;
    public int damage;
    public int speed;

    public float shotTimer = 1.0f;
    public bool justShot = false;
    public int damageDone = 0;

    public GameObject player;

    public GameObject gun;
    public GameObject bulletPrefab;

    float angleToRotate;
    Vector2 rotateDir;
    Rigidbody2D rb;

    public float fitness;

    // Start is called before the first frame update
    void Start()
    {
      rb = GetComponent<Rigidbody2D>();
      player = GameObject.FindGameObjectWithTag("Player");
      speed /= 10;
      startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
      if(!dead)
      {
        angleToRotate = 0;
        Vector2 playerDir = player.transform.position - transform.position;
        angleToRotate = Mathf.Atan2(playerDir.y, playerDir.x) * Mathf.Rad2Deg - 90;
        rb.rotation = angleToRotate;
        Fire();
      }
    }

    void FixedUpdate()
    {
      if(!dead)
      {
        rb.velocity = (player.transform.position - transform.position).normalized * speed;
      }
    }

    void Fire()
    {
      if(!justShot)
      {
        GameObject bullet = Instantiate(bulletPrefab, gun.transform.position, transform.rotation);
        bullet.GetComponent<GoForward>().damage = damage;
        bullet.GetComponent<GoForward>().shooter = this;
        justShot = true;
        StartCoroutine(Cooldown());
      }
    }

    IEnumerator Cooldown()
    {
      yield return new WaitForSeconds(shotTimer);
      if(Random.Range(0.0f, 1.0f) < round)
      {
        justShot = false;
      }
    }

    public void TakeDamage(int damage)
    {
      hp -= damage;
      if(hp <= 0)
      {
        spawner.dead++;
        setFitness();
        foreach (Transform child in gameObject.transform)
        {
           GameObject.Destroy(child.gameObject);
        }
        Destroy(GetComponent<BoxCollider2D>());
        dead = true;
        //play a cool explosion maybe
      }
    }

    void setFitness()
    {
      float timeAlive = Time.time - startTime;
      fitness = timeAlive + (float)damageDone;
    }
}
