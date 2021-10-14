using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBackground : MonoBehaviour
{
    public List<GameObject> background = new List<GameObject>();
    int iterator;


    void Start()
    {
        for (int i = 0; i < background.Count; i++)
        {
            if(i==0)
            background[i].SetActive(true);
            else
            background[i].SetActive(false);
        }
        
    }

    public void SwitchBg()
    {
        iterator++;

        iterator%=background.Count;

        for (int i = 0; i < background.Count; i++)
        {
            background[i].SetActive(false);
        }
        
        background[iterator].SetActive(true);
        
    }
}
