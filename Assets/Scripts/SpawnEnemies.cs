using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public Evolver evolver;
    public GameObject enemyPrefab;
    public GameObject[] spawnPoints;
    public List<GameObject> enemies;

    public int round = 1;
    public int dead = 0;
    public bool roundOver = false;
    public bool readyToStart = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      if(readyToStart)
      {
        if(Input.GetButtonDown("Submit"))
        {
          evolver.Evolve();
          readyToStart = false;
        }
      }
      if(dead >= 10)
      {
        roundOver = true;
      }
      if(roundOver)
      {
        getFitnesses();
        clearEnemies();
        dead = 0;
        roundOver = false;
        readyToStart = true;
      }
    }

    public void spawn(string[] stats)
    {
      for(int i = 0; i < Evolver.pop_size * 3; i += 3)
      {
        int hp =  Convert.ToInt32(stats[i], 2);
        int damage = Convert.ToInt32(stats[i+1], 2);
        int speed = Convert.ToInt32(stats[i+2], 2);
        Debug.Log(hp + " " + damage + " " + speed);
        GameObject enemy = Instantiate(enemyPrefab, spawnPoints[i / 3].transform.position, Quaternion.identity);
        enemy.GetComponent<Enemy>().hp = hp;
        enemy.GetComponent<Enemy>().damage = damage;
        enemy.GetComponent<Enemy>().speed = speed;
        enemy.GetComponent<Enemy>().round = round;
        enemy.GetComponent<Enemy>().spawner = this;
        enemies.Add(enemy);
      }
    }

    public void getFitnesses()
    {
      List<float> ret = new List<float>();
      for(int i = 0; i < Evolver.pop_size; i++)
      {
        ret.Add(enemies[i].GetComponent<Enemy>().fitness);
      }
      evolver.fitnesses = ret;
      evolver.sendFitness();
    }

    void clearEnemies()
    {
      foreach(GameObject enemy in enemies)
      {
        Destroy(enemy);
      }
      enemies.Clear();
      round++;
    }
}
