using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

public class UpdateDecalLibrary : EditorWindow
{
    //public List<Sprite> sprites = new List<Sprite>();

    [MenuItem("Update Decal Library/Update")]
    static void UpdateLib()
    {
        string[] guids2 = AssetDatabase.FindAssets("", new[] { "Assets/Simple Livery Creator/Sprites/Decal Library" });
        //Debug.Log(guids2.Length);
        GameObject TileImg;
        TileImg = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Simple Livery Creator/Prefabs/Image.prefab", typeof(GameObject));
        GameObject Content = GameObject.Find("Content");
        int childnum = 0;
        foreach (RectTransform child in Content.transform)
            {
                ++childnum;
            }
            for(int i =0;i<childnum;i++)
            GameObject.DestroyImmediate(Content.transform.GetChild(0).gameObject);
            Sprite spriteFound;

        foreach (string guid2 in guids2)
        {
            spriteFound = (Sprite)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid2), typeof(Sprite)); 
            TileImg.GetComponent<Image>().sprite = spriteFound;
            Instantiate(TileImg,Content.transform);
        }
    }
}
