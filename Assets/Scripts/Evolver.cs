using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using TMPro;

public class Evolver : MonoBehaviour
{
    public static int pop_size = 10;
    public List<float> fitnesses;

    public GameObject enemyManager;

    void Start()
    {
        for(int i = 0; i < pop_size; i++)
        {
          fitnesses.Add(0);
        }
    }

    void getStats(String stats)
    {
      string[] sepStats = stats.Split(" ");
      Debug.Log(sepStats);
      enemyManager.GetComponent<SpawnEnemies>().spawn(sepStats);
    }

    public void Evolve()
    {
      Debug.Log("Evolving.");

      UdpClient client = new UdpClient(5600);
      try
      {
        client.Connect("127.0.0.1", 5500);
        byte[] sendBytes = Encoding.ASCII.GetBytes("Game Connected");
        client.Send(sendBytes, sendBytes.Length);

        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 5500);
        byte[] receiveBytes = client.Receive(ref remoteEndPoint);
        string receivedString = Encoding.ASCII.GetString(receiveBytes);
        receivedString = Encoding.ASCII.GetString(receiveBytes);
        Debug.Log(receivedString);
        if(receivedString != "Game Over")
        {
          getStats(receivedString);
        }
        else
        {
          // Trigger Gameover
        }
      }
      catch(Exception e)
      {
        Debug.Log("Exception thrown in Evolve: " + e.Message);
      }
      client.Close();
    }

    public void sendFitness()
    {
      UdpClient client = new UdpClient(5600);
      try
      {
        client.Connect("127.0.0.1", 5500);
        string fit = "";

        for(int i = 0; i < pop_size; i++)
        {
          fit += (fitnesses[i].ToString("0.0000") + " ");
        }
        byte[] sendBytes = Encoding.ASCII.GetBytes(fit);
        client.Send(sendBytes, sendBytes.Length);
      }
      catch(Exception e)
      {
        Debug.Log("Exception thrown in sendFitness: " + e.Message);
      }
      client.Close();
    }
}
