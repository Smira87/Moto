using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchVehicle : MonoBehaviour
{
    public List<GameObject> vehicles = new List<GameObject>();
    int iterator;
    LiveryCreator liveryCreator;
    BodyPaintPicker bodyPaintPicker;
    void Start()
    {
        liveryCreator = FindObjectOfType<LiveryCreator>();
        bodyPaintPicker = FindObjectOfType<BodyPaintPicker>();
        for (int i = 0; i < vehicles.Count; i++)
        {
            if(i==0)
            vehicles[i].SetActive(true);
            else
            vehicles[i].SetActive(false);
        }
    }
    Texture2D ResetColor()
    {
    Texture2D texture_;
    texture_ = new Texture2D(2048, 2048);
     // Reset all pixels color to white
     Color32 resetColor = new Color32(255, 255, 255, 0);
     Color32[] resetColorArray = texture_.GetPixels32();

     for (int i = 0; i < resetColorArray.Length; i++) {
         resetColorArray[i] = resetColor;
     }
      
     texture_.SetPixels32(resetColorArray);
     texture_.Apply();
     return texture_;
    }
    public void SwitchV()
    {
        iterator++;

        iterator%=vehicles.Count;

        for (int i = 0; i < vehicles.Count; i++)
        {
            vehicles[i].SetActive(false);
        }
        
        vehicles[iterator].SetActive(true);

        liveryCreator.baseMaterial.mainTexture = ResetColor();
        bodyPaintPicker.UVChanged=0;
        liveryCreator.canvasMaterial.SetFloat("_FirstTexUVSet", 0);


    }
    
}
