using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int hp = 100;
    public int speed = 50;
    public int damage = 10;
    public float shotTimer = 1.0f;
    public bool justShot = false;

    public GameObject gun;
    public GameObject bulletPrefab;
    public GameObject slider;
    public GameObject goscreen;

    int maxhp; 
    float angleToRotate;
    Vector2 rotateDir;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
      rb = GetComponent<Rigidbody2D>();
      maxhp = hp;
    }

    // Update is called once per frame
    void Update()
    {
      slider.GetComponent<Slider>().value = hp;
      angleToRotate = 0;
      if(Input.GetAxis("GVertical") != 0 || Input.GetAxis("GHorizontal") != 0)
      {
        angleToRotate = Mathf.Atan2(Input.GetAxis("GVertical"), Input.GetAxis("GHorizontal")) * Mathf.Rad2Deg - 90;
        Fire();
      }
      rb.rotation = angleToRotate;
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
    }

    void Fire()
    {
      if(!justShot)
      {
        GameObject bullet = Instantiate(bulletPrefab, gun.transform.position, transform.rotation);
        bullet.GetComponent<GoForward>().damage = damage;
        justShot = true;
        StartCoroutine(Cooldown());
      }
    }

    IEnumerator Cooldown()
    {
      yield return new WaitForSeconds(shotTimer);
      justShot = false;
    }

    public void TakeDamage(int damage)
    {
      hp -= damage;
      if(hp <= 0)
      {
        GameOver();
      }
    }

    void GameOver()
    {
      Destroy(this.gameObject);
      goscreen.SetActive(true);
      //maybe play a cool explosion
      //open gameover screen
    }
}
