using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    public GameObject[] walls;
    // public GameObject[] doors;

    // Never  Eat   Soggy  Waffles
    // North, East, South, West
    // 0      1     2      3

    public void UpdateRoom(bool[] status)
    {
        for (int i = 0; i < status.Length; i++)
        {
            // doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);

        }
        
    }
}
