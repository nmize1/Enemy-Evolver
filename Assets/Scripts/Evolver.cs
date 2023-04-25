using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Evolver : MonoBehaviour
{
    public static int pop_size = 10;
    public List<float> fitnesses;

    public GameObject enemyManager;
    public GameObject wave;
    public GameObject menu;
    public GameObject winscreen;

    public TMP_InputField genEntry;
    public Slider mutEntry;
    public Slider crossEntry;
    public Slider tournEntry;

    public TMP_Text mutVal;
    public TMP_Text crossVal;
    public TMP_Text tournVal;

    String max_gen;
    String mutation;
    String crossover;
    String tourn_size;
    int counter = 0;

    void Start()
    {
        for(int i = 0; i < pop_size; i++)
        {
          fitnesses.Add(0);
        }
        wave.GetComponent<TMP_Text>().text = (counter.ToString("X"));
    }

    void Update()
    {
      if(counter == 0)
      {
        mutVal.text = mutEntry.value.ToString().Substring(0,3);
        crossVal.text = crossEntry.value.ToString().Substring(0,3);
        tournVal.text = tournEntry.value.ToString();
      }
    }

    void sendSettings()
    {
      max_gen = (Int32.Parse(genEntry.text) - 1).ToString();
      mutation = mutEntry.value.ToString();
      crossover = crossEntry.value.ToString();
      tourn_size = tournEntry.value.ToString();

      UdpClient client = new UdpClient(5600);
      try
      {
        client.Connect("127.0.0.1", 5500);
        byte[] sendBytes = Encoding.ASCII.GetBytes(max_gen);
        client.Send(sendBytes, sendBytes.Length);

        sendBytes = Encoding.ASCII.GetBytes(mutation.ToString());
        client.Send(sendBytes, sendBytes.Length);

        sendBytes = Encoding.ASCII.GetBytes(crossover.ToString());
        client.Send(sendBytes, sendBytes.Length);

        sendBytes = Encoding.ASCII.GetBytes(tourn_size.ToString());
        client.Send(sendBytes, sendBytes.Length);
      }
      catch(Exception e)
      {
        Debug.Log("Exception thrown in Evolve: " + e.Message);
      }
      client.Close();
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
      if(counter == 0)
      {
        menu.SetActive(false);
        sendSettings();
      }
      counter++;

      if(counter - 1 > Int32.Parse(max_gen))
      {
        winscreen.SetActive(true);
      }
      
      wave.GetComponent<TMP_Text>().text = (counter.ToString("X"));

      UdpClient client = new UdpClient(5600);
      try
      {
        client.Connect("127.0.0.1", 5500);
        byte[] sendBytes = Encoding.ASCII.GetBytes("Game Connected");
        client.Send(sendBytes, sendBytes.Length);

        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 5500);
        byte[] receiveBytes = client.Receive(ref remoteEndPoint);
        string receivedString = Encoding.ASCII.GetString(receiveBytes);
        //receivedString = Encoding.ASCII.GetString(receiveBytes);
        Debug.Log("RECEIVED STRING: " + receivedString);
        Debug.Log((counter - 1).ToString() + " " + max_gen);
        if(counter - 1 <= Int32.Parse(max_gen))
        {
          getStats(receivedString);
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
