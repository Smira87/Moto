using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MatrixBlender))]
public class Perspective : MonoBehaviour
{
    private Matrix4x4 ortho, perspective;
    public float fov = 60f,near = .3f,far = 1000f,orthographicSize = 50f;
    private float aspect;
    private MatrixBlender blender;
    private bool orthoOn;
    Camera m_camera;

    void Start()
    {
        aspect = (float)Screen.width / (float)Screen.height;
        ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
        perspective = Matrix4x4.Perspective(fov, aspect, near, far);
        m_camera = GetComponent<Camera>();
        //m_camera.projectionMatrix = ortho;
        orthoOn = false;
        blender = (MatrixBlender)GetComponent(typeof(MatrixBlender));
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            orthoOn = !orthoOn;
            if (orthoOn)
                blender.BlendToMatrix(ortho, 0.5f, 8, true);
            else
                blender.BlendToMatrix(perspective, 0.5f, 8, false);
        }
    }

    public void OrthoCubeToggle()
    {
        orthoOn = !orthoOn;
            if (orthoOn)
                blender.BlendToMatrix(ortho, 0.5f, 8, true);
            else
                blender.BlendToMatrix(perspective, 0.5f, 8, false);
    }
}