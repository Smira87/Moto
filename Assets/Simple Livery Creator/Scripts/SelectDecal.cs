using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectDecal : MonoBehaviour
{
    LiveryCreator liveryCreator;
    Sprite Img;
    void Start()
    {
        liveryCreator = FindObjectOfType<LiveryCreator>();
        Img = GetComponent<Image>().sprite;
    }
    
    public void SendDecal()
    {
        liveryCreator.Decal = Img;
        liveryCreator.decalSelected = true;
    }
}
