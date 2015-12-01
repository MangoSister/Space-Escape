using UnityEngine;
using System.Collections;

/// <summary>
/// A grid drawer using basic openGL functions
/// To use it:
/// 1 attach it to the main camera,
/// 2 attach the grid system to the drawer
/// 3 attach one reference Y plane 
/// </summary>
public class GridDrawer : MonoBehaviour
{
    public GridSystem gridSystem;
    public GameObject plane;

    public bool showMain = true;
    public bool showSub = false;

    public int gridSizeX;
    public int gridSizeY;
    public int gridSizeZ;

    public float smallStep;
    public float largeStep;

    public Vector3 start;

    private float offsetY = 0;
    private float scrollRate = 0.1f;
    private float lastScroll = 0f;

    public Material lineMaterial;

    private Color mainColor = new Color(0f, 1f, 0f, 1f);
    private Color subColor = new Color(0f, 0.5f, 0f, 1f);

    void Start()
    {
        gridSizeX = gridSystem.gridSizeX;
        gridSizeY = 0;
        gridSizeZ = gridSystem.gridSizeZ;
        start.x = gridSystem.transform.position.x -0.5f * (float)gridSystem.cellSize;
        start.y = gridSystem.transform.position.y; //- 0.5f * (float)gridSystem.cellSize;
        start.z = gridSystem.transform.position.z - 0.5f * (float)gridSystem.cellSize;

        largeStep = gridSystem.cellSize;
    }

    void Update()
    {
        if (lastScroll + scrollRate < Time.time)
        {
            if (Input.GetKey(KeyCode.KeypadPlus))
            {
                plane.transform.position = new Vector3(plane.transform.position.x, plane.transform.position.y + smallStep, plane.transform.position.z);
                offsetY += smallStep;
                lastScroll = Time.time;
            }
            if (Input.GetKey(KeyCode.KeypadMinus))
            {
                plane.transform.position = new Vector3(plane.transform.position.x, plane.transform.position.y - smallStep, plane.transform.position.z);
                offsetY -= smallStep;
                lastScroll = Time.time;
            }
        }
    }

    void OnPostRender()
    {
        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);

        if (showSub)
        {
            GL.Color(subColor);

            //Layers
            for (float j = start.y; j <= start.y + gridSizeY* largeStep; j += smallStep)
            {
                //X axis lines
                for (float i = 0; i <= gridSizeZ * largeStep; i += smallStep)
                {
                    GL.Vertex3(start.x, j + offsetY, start.z + i);
                    GL.Vertex3(start.x + gridSizeX * largeStep, j + offsetY, start.z + i);
                }

                //Z axis lines
                for (float i = 0; i <= gridSizeX * largeStep; i += smallStep)
                {
                    GL.Vertex3(start.x + i, j + offsetY, start.z);
                    GL.Vertex3(start.x + i, j + offsetY, start.z + gridSizeZ * largeStep);
                }
            }

            //Y axis lines
            for (float i = 0; i <= gridSizeZ * largeStep; i += smallStep)
            {
                for (float k = 0; k <= gridSizeX * largeStep; k += smallStep)
                {
                    GL.Vertex3(start.x + k, start.y + offsetY, start.z + i);
                    GL.Vertex3(start.x + k, start.y + gridSizeY * largeStep + offsetY, start.z + i);
                }
            }
        }

        if (showMain)
        {
            GL.Color(mainColor);

            //Layers
            for (float j = start.y; j <= start.y + gridSizeY * largeStep; j += largeStep)
            {
                //X axis lines
                for (float i = 0; i <= gridSizeZ * largeStep; i += largeStep)
                {
                    GL.Vertex3(start.x, j + offsetY, start.z + i);
                    GL.Vertex3(start.x+ gridSizeX * largeStep, j + offsetY, start.z + i);
                }

                //Z axis lines
                for (float i = 0; i <= gridSizeX * largeStep; i += largeStep)
                {
                    GL.Vertex3(start.x + i, j + offsetY, start.z);
                    GL.Vertex3(start.x + i, j + offsetY, start.z + gridSizeZ * largeStep);
                }
            }

            //Y axis lines
            for (float i = 0; i <= gridSizeZ * largeStep; i += largeStep)
            {
                for (float k = 0; k <= gridSizeX * largeStep; k += largeStep)
                {
                    GL.Vertex3(start.x + k, start.y + offsetY, start.z + i);
                    GL.Vertex3(start.x + k, start.y + gridSizeY * largeStep + offsetY, start.z + i);
                }
            }
        }


        GL.End();
    }
}