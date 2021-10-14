using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class OrbitCam : MonoBehaviour
{
    public Transform target;
    public float distance = 5.0f;
    public float height = 5.0f;
    public float xSpeed = 20.0f;
    public float ySpeed = 20.0f;
    public float pinchSpeed = 2f;
    private float lastDist = 10f;
    private float curDist = 10f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    public float smoothTime = 2f;

    public float groundHitPos = -4.2f;

    float rotationYAxis = 0.0f;
    float rotationXAxis = 0.0f;

    float velocityX = 0.0f;
    float velocityY = 0.0f;
    Vector3 position;
    bool snap1 = true;
    bool snap2 = true;
    bool snap3 = true;

    bool speedReduced = false;
    bool mouseControl = true;

    Touch touch;
    float smoothdist = 0;
    Quaternion toRotation;
    Vector3 negDistance;
    float pan;

    LiveryCreator liveryCreator;
    // Use this for initialization
    void Start()
    {
        liveryCreator = FindObjectOfType<LiveryCreator>();
        Vector3 angles = transform.eulerAngles;
        rotationYAxis = angles.y;
        rotationXAxis = angles.x;
        smoothdist = distance;

    }

    void LateUpdate()
    {
        
        if (target)
        {
            if (Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftShift))
            {

                velocityX += xSpeed * Input.GetAxis("Mouse X") * 0.02f;
                velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
            }
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved && !IsPointerOverUIObject() && !liveryCreator.decalSelected)
            {

                //One finger touch does orbit

                mouseControl = false; // Disable mouse mode

                if (!speedReduced)
                {
                    //Decreasing cam rotation speed for suitable rotation using touch

                    xSpeed /= 4;
                    ySpeed /= 4;
                    speedReduced = true;
                }

                touch = Input.GetTouch(0);

                velocityX += xSpeed * touch.deltaPosition.x * 0.02f;
                velocityY += ySpeed * touch.deltaPosition.y * 0.02f;

            }
            if (Input.touchCount > 1 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved))

            {

                //Two finger touch does pinch to zoom

                mouseControl = false; // Disable mouse mode

                var touch1 = Input.GetTouch(0);

                var touch2 = Input.GetTouch(1);

                curDist = Vector2.Distance(touch1.position, touch2.position);

                if (curDist > lastDist)

                {
                    // Zoom in
                  
                    smoothdist -= Vector2.Distance(touch1.deltaPosition, touch2.deltaPosition) * pinchSpeed / 10;
                   

                }
                else 
                {
                    // Zoom out

                    smoothdist += Vector2.Distance(touch1.deltaPosition, touch2.deltaPosition) * pinchSpeed / 10;
                    
                }
                distance = Mathf.Lerp(distance, smoothdist, Time.deltaTime * smoothTime);
                distance = Mathf.Clamp(distance, 5f, 20f); // Limit distance
                lastDist = curDist;

            }


            if (Input.GetKeyDown(KeyCode.Alpha1)||Input.GetKeyDown("[1]"))
                StartCoroutine(SnappingTime(1));
            else if (Input.GetKeyDown(KeyCode.Alpha2)||Input.GetKeyDown("[2]"))
                StartCoroutine(SnappingTime(2));
            else if (Input.GetKeyDown(KeyCode.Alpha3)||Input.GetKeyDown("[3]"))
                StartCoroutine(SnappingTime(3));


            if (snap1 == false)
            {
                rotationYAxis = Mathf.Lerp(rotationYAxis, 0, Time.deltaTime * smoothTime);
                rotationXAxis = Mathf.Lerp(rotationXAxis, 0, Time.deltaTime * smoothTime);
            }
            else if (snap2 == false)
            {
                rotationYAxis = Mathf.Lerp(rotationYAxis, 90, Time.deltaTime * smoothTime);
                rotationXAxis = Mathf.Lerp(rotationXAxis, 0, Time.deltaTime * smoothTime);
            }
            else if (snap3 == false)
            {
                rotationYAxis = Mathf.Lerp(rotationYAxis, 90, Time.deltaTime * smoothTime);
                rotationXAxis = Mathf.Lerp(rotationXAxis, 90, Time.deltaTime * smoothTime);
            }

            else if (snap1 && snap2 && snap3)
            {
                rotationYAxis += velocityX;
                rotationXAxis -= velocityY;
            }

            yMinLimit = Mathf.Sin(-1.66f/distance)*Mathf.Rad2Deg;
            rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);

            Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
           
            if (mouseControl)
            {
                smoothdist -= Input.GetAxis("Mouse ScrollWheel") * 5;
                
                distance = Mathf.Lerp(distance, smoothdist, Time.deltaTime * smoothTime);
                distance = Mathf.Clamp(distance, 4f, 20f); // Limit distance
                if (Input.GetMouseButton(2))
                {
                    height -= xSpeed * Input.GetAxis("Mouse Y") * 0.005f;
                    pan -= xSpeed * Input.GetAxis("Mouse X") * 0.005f;

                }
                
            }

            negDistance = new Vector3(pan, height, -distance);

            position = toRotation * negDistance + target.position;

            // if(transform.position.y!=groundHitPos)
            // transform.rotation = toRotation;
            // else
            // transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,toRotation.eulerAngles.y,toRotation.eulerAngles.z);
            transform.rotation = toRotation;
            transform.position = new Vector3(position.x,Mathf.Max(position.y,groundHitPos),position.z);


            velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
            velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);

        }

    }

    private bool IsPointerOverUIObject()
    {
        // Detect if cursor is on UI element

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
    IEnumerator SnappingTime(int a)
    {
        if (a == 1)
        {
            snap1 = false;
            yield return new WaitForSeconds(0.5f);
            snap1 = true;
        }
        if (a == 2)
        {
            snap2 = false;
            yield return new WaitForSeconds(0.5f);
            snap2 = true;
        }
        if (a == 3)
        {
            snap3 = false;
            yield return new WaitForSeconds(0.5f);
            snap3 = true;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}