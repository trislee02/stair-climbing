using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stair : MonoBehaviour
{
    public int numberOfSteps;
    public float rise;
    public float treadWidth;
    public float treadLength;
    public Vector3 startPosition;
    private GameObject stairWalls;
    public float wallHeight;
    public bool isWallVisible = true;
    public Material stairMaterial;
    public int wallStep = 1;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 stairTextureScale = new Vector2(Mathf.Ceil(treadLength*3), Mathf.Ceil(treadWidth*3));

        stairWalls = new GameObject("StairWalls");
        for (int i = 1; i <= numberOfSteps; i++)
        {
            Vector3 stepPosition = new Vector3(startPosition.x, startPosition.y + (i * rise) - (rise / 2), startPosition.z + (i * treadWidth) - (treadWidth / 4));
            GameObject stairStep = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stairStep.transform.position = stepPosition;
            stairStep.transform.localScale = new Vector3(treadLength, rise, treadWidth);
            stairStep.transform.parent = transform;
            stairStep.transform.localScale = new Vector3(treadLength, rise, treadWidth);
            //// Change the color of the cube to the random color
            //Color randomColor = new Color(
            //    Random.Range(0f, 1f), // Red
            //    Random.Range(0f, 1f), // Green
            //    Random.Range(0f, 1f)  // Blue
            //);
            //stairStep.GetComponent<Renderer>().material.color = randomColor;
            stairStep.GetComponent<Renderer>().material = stairMaterial;
            stairStep.GetComponent<Renderer>().material.mainTextureScale = stairTextureScale;
            stairStep.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

            if (i % wallStep == 0)
            {
                GameObject stairWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //stairWall.transform.position = new Vector3(startPosition.x + treadLength / 2.0f, startPosition.y + ((i-4+1) * rise) - (rise / 2) + (wallHeight / 2), startPosition.z + ((i-4+1) * treadWidth) - (treadWidth / 2));
                stairWall.transform.position = new Vector3(stepPosition.x + treadLength / 2f, stepPosition.y + wallHeight/2f - rise * wallStep, stepPosition.z - treadWidth * wallStep / 2f + (treadWidth/2f));
                stairWall.transform.localScale = new Vector3(0.1f, wallHeight, treadWidth*wallStep);
                stairWall.transform.parent = stairWalls.transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        stairWalls.SetActive(isWallVisible);
    }
}
