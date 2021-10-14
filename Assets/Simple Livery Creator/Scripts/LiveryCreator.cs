using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class LiveryCreator : MonoBehaviour
{
    public GameObject brushCursor, brushContainer;
    public Camera sceneCamera, canvasCam;
    private RenderTexture canvasTexture;
    public Material canvasMaterial, liveryMaterial;
    public Material baseMaterial;
    private float sizeSlider = 1f;
    private float controlRot = 1;
    public Sprite Decal;
    public float scaleSensitivity;
    public float rotateSensitivity;
    public List<Texture2D> undoTex = new List<Texture2D>();
    public Texture2D current;

    public bool decalSelected = false;

    Color brushColor;
    bool saving = false;
    Vector3 oldMousePos, currentpos;
    float sizeSliderOld = 1;
    float controlRotOld = 1;
    BodyPaintPicker bodyPaintPicker;
    DecalPaintPicker decalPaintPicker;
    Vector2 pixelUV;
    Color32[] DecalPixels;
    bool validClick;
    float sheerX=1,sheerY=1;


    void Start()
    {
        brushCursor.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        canvasTexture = new RenderTexture(2048, 2048, 0, RenderTextureFormat.ARGB32);
        canvasTexture.Create();
        canvasMaterial.SetTexture("_MainTex", canvasTexture);
        canvasMaterial.SetColor("_Color", new Color(0.9f, 0.9f, 0.9f, 1));
        canvasCam.targetTexture = canvasTexture;
        bodyPaintPicker = FindObjectOfType<BodyPaintPicker>();
        decalPaintPicker = FindObjectOfType<DecalPaintPicker>();

        //Starting BaseMaterial Color
        Texture2D texture_;
        texture_ = new Texture2D(2048, 2048);
        // Reset all pixels color to white
        Color32 resetColor = new Color32(255, 255, 255, 0);
        Color32[] resetColorArray = texture_.GetPixels32();

        for (int i = 0; i < resetColorArray.Length; i++)
        {
            resetColorArray[i] = resetColor;
        }

        texture_.SetPixels32(resetColorArray);
        texture_.Apply();
        baseMaterial.mainTexture = texture_;
    }
    void Update()
    {

        brushCursor.GetComponent<SpriteRenderer>().sprite = Decal;
        if (decalPaintPicker._update != decalPaintPicker.idle)
            brushCursor.GetComponent<SpriteRenderer>().color = new Color(decalPaintPicker.resultColor.r, decalPaintPicker.resultColor.g, decalPaintPicker.resultColor.b, 0.5f);

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyUp(KeyCode.R)) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
        {


            if (brushCursor.activeSelf)
            {
                validClick = true;
                Undo();
            }
        }
    

        if (Input.GetMouseButtonUp(0) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl) && bodyPaintPicker._update == bodyPaintPicker.idle && validClick)
        {
            PasteDecal();
        }
        if (Input.GetKeyUp(KeyCode.Z) && undoTex.Count > 0)
        {
            baseMaterial.mainTexture = undoTex[undoTex.Count - 1];
            undoTex.RemoveAt(undoTex.Count - 1);
        }
        if (bodyPaintPicker._update != bodyPaintPicker.idle) // records MouseButtonUp
        {
            if (Input.GetMouseButton(0))
                BodyPaint();

            else if (Input.GetMouseButtonUp(0))
            {
                StartCoroutine(BodyPaintFinalize());
                canvasMaterial.SetTexture("_MainTex", canvasTexture);

            }

        }


        UpdateBrushCursor();
        SetBrushSize();


    }
    void BodyPaint()
    {
        if (canvasMaterial.mainTexture != null)
            canvasMaterial.mainTexture = null;
        canvasMaterial.SetColor("_Color", bodyPaintPicker.resultColor);
    }
    IEnumerator BodyPaintFinalize()
    {
        yield return new WaitForEndOfFrame();
        Texture2D tex = (Texture2D)baseMaterial.mainTexture;
        Color32 resetColor = bodyPaintPicker.resultColor;
        Color32[] resetColorArray = tex.GetPixels32();
        Color32 testCol = (Color32)tex.GetPixel(1, 1);
        if (!bodyPaintPicker.overwrite)
        {
            for (int i = 0; i < resetColorArray.Length; i++)
            {
                if (resetColorArray[i].Equals(testCol))
                {
                    resetColorArray[i] = resetColor;

                }
            }
        }
        else
        {
            for (int i = 0; i < resetColorArray.Length; i++)
            {

                resetColorArray[i] = resetColor;


            }
        }
        tex.SetPixels32(resetColorArray);
        canvasMaterial.SetColor("_Color", Color.white);
        tex.Apply();
        baseMaterial.mainTexture = tex;

    }
    void Undo()
    {
        Vector3 uvWorldPosition = Vector3.zero;
        if (HitTestUVPosition(ref uvWorldPosition))
            undoTex.Add((Texture2D)baseMaterial.mainTexture);
    }
    void PasteDecal()
    {
        
        if (saving)
            return;
        Vector3 uvWorldPosition = Vector3.zero;
        if (HitTestUVPosition(ref uvWorldPosition))
        {
            GameObject brushObj, DecalObj;
            DecalObj = new GameObject("Temp Decal Prefab");
            DecalObj.AddComponent<SpriteRenderer>();
            DecalObj.GetComponent<SpriteRenderer>().sprite = Decal;
            DecalObj.GetComponent<SpriteRenderer>().color = decalPaintPicker.resultColor;
            brushObj = Instantiate(DecalObj);
            brushColor.a = ShiftSize() * 2.0f;
            brushObj.transform.parent = brushContainer.transform;
            brushObj.transform.localPosition = uvWorldPosition;
            brushObj.transform.localScale = brushCursor.transform.localScale;
            brushObj.transform.eulerAngles = new Vector3(0, 0, ControlRotate());
            Destroy(DecalObj);
        }
        validClick = false;
        saving = true;
        Invoke("SaveTexture", 0.1f);
        Decal = null;
        decalSelected = false;
    }
    void UpdateBrushCursor()
    {
        Vector3 uvWorldPosition = Vector3.zero;
        if (HitTestUVPosition(ref uvWorldPosition) && !saving)
        {
            brushCursor.SetActive(true);
            brushCursor.transform.position = uvWorldPosition + brushContainer.transform.position;
        }
        else
        {
            brushCursor.SetActive(false);
        }
    }
    bool HitTestUVPosition(ref Vector3 uvWorldPosition)
    {
        RaycastHit hit;
        Vector3 cursorPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        Ray cursorRay = sceneCamera.ScreenPointToRay(cursorPos);
        if (Physics.Raycast(cursorRay, out hit, 200))
        {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
                return false;
            if (bodyPaintPicker.UVChanged % 2 == 0)
            {
                pixelUV = new Vector2(hit.textureCoord.x, hit.textureCoord.y);


            }
            else
            {
                pixelUV = new Vector2(hit.lightmapCoord.x, hit.lightmapCoord.y);

            }

            uvWorldPosition.x = pixelUV.x - canvasCam.orthographicSize;
            uvWorldPosition.y = pixelUV.y - canvasCam.orthographicSize;
            uvWorldPosition.z = 0.0f;
            return true;
        }
        else
        {
            return false;
        }

    }


    void SaveTexture()
    {
        RenderTexture.active = canvasTexture;
        Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        baseMaterial.mainTexture = tex;

        foreach (Transform child in brushContainer.transform)
        {
            Destroy(child.gameObject);
        }
        Invoke("ShowCursor", 0.1f);
    }
    void ShowCursor()
    {
        saving = false;
    }
    public void SetBrushSize()
    {
        if(Input.GetKey(KeyCode.W))
        sheerY +=Time.deltaTime;
        else if(Input.GetKey(KeyCode.S))
        sheerY -=Time.deltaTime;

        if(Input.GetKey(KeyCode.D))
        sheerX +=Time.deltaTime;
        else if(Input.GetKey(KeyCode.A))
        sheerX -=Time.deltaTime;

        if (bodyPaintPicker.hflip && bodyPaintPicker.vflip)
            brushCursor.transform.localScale = new Vector3(-sheerX, -sheerY, 1) * ShiftSize();
        else if (bodyPaintPicker.hflip)
            brushCursor.transform.localScale = new Vector3(-sheerX, sheerY, 1) * ShiftSize();
        else if (bodyPaintPicker.vflip)
            brushCursor.transform.localScale = new Vector3(sheerX, -sheerY, 1) * ShiftSize();
        else
            brushCursor.transform.localScale = new Vector3(sheerX, sheerY, 1) * ShiftSize();

        brushCursor.transform.eulerAngles = new Vector3(0, 0, ControlRotate());

    }
    public float ShiftSize()
    {
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift))
            oldMousePos = Input.mousePosition;
        if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift))
            currentpos = Input.mousePosition;
        if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift))
        {
            sizeSlider = sizeSliderOld + ((currentpos.x + currentpos.y) - (oldMousePos.x + oldMousePos.y)) * 0.01f * scaleSensitivity;
        }
        if (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftShift))
            sizeSliderOld = sizeSlider;
        return sizeSlider;
    }
    public float ControlRotate()
    {
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl))
            oldMousePos = Input.mousePosition;
        if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl))
            currentpos = Input.mousePosition;
        if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl))
        {
            controlRot = controlRotOld + (((currentpos.x + currentpos.y) - (oldMousePos.x + oldMousePos.y)) * rotateSensitivity);
        }
        if (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftControl))
            controlRotOld = controlRot;
        return controlRot;
    }
    public void SaveLivery()
    {
        StartCoroutine(SaveTextureToFile((Texture2D)baseMaterial.mainTexture));
    }
    IEnumerator SaveTextureToFile(Texture2D savedTexture)
    {
        if (savedTexture != null)
        {
            string dateTime = System.DateTime.Now.ToString("hh-mm-ss") + ".png";
            string fullPath = System.IO.Directory.GetCurrentDirectory() + "/Simple Livery Creator/MyLiveries/" + dateTime;
            string fileName = dateTime;
            if (!System.IO.Directory.Exists(fullPath))
                System.IO.Directory.CreateDirectory(fullPath);
            var bytes = savedTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/Simple Livery Creator/MyLiveries/" + fileName, bytes);
            Debug.Log("<color=green>Livery Saved! </color>" + Application.dataPath + "/Simple Livery Creator/MyLiveries/" + fileName + ". Please find Livery Materials and Textures inside MyLiveries Folder.");
            yield return new WaitForSeconds(1);

            AssetDatabase.Refresh();
            yield return new WaitForSeconds(1);
            ApplyToMaterial(fileName);
        }

    }
    void ApplyToMaterial(string path)
    {
        Texture2D livery;
        livery = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Simple Livery Creator/MyLiveries/" + path, typeof(Texture2D));
        liveryMaterial.SetTexture("_MainTex", livery);
        var _Glossiness = canvasMaterial.GetFloat("_Glossiness");
        liveryMaterial.SetFloat("_Glossiness", _Glossiness);
        var _Metallic = canvasMaterial.GetFloat("_Metallic");
        liveryMaterial.SetFloat("_Metallic", _Metallic);
    }
}
