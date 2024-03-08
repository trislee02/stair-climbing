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

    // Start is called before the first frame update
    void Start()
    {
        stairWalls = new GameObject("StairWalls");
        for (int i = 1; i <= numberOfSteps; i++)
        {
            Vector3 stepPosition = new Vector3(startPosition.x, startPosition.y + (i * rise) - (rise / 2), startPosition.z + (i * treadWidth) - (treadWidth / 4));
            GameObject stairStep = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stairStep.transform.position = stepPosition;
            stairStep.transform.localScale = new Vector3(treadLength, rise, treadWidth);
            stairStep.transform.parent = transform;
            stairStep.transform.localScale = new Vector3(treadLength, rise, treadWidth);
            // Change the color of the cube to the random color
            Color randomColor = new Color(
                Random.Range(0f, 1f), // Red
                Random.Range(0f, 1f), // Green
                Random.Range(0f, 1f)  // Blue
            );
            stairStep.GetComponent<Renderer>().material.color = randomColor;

            GameObject stairWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stairWall.transform.position = new Vector3(startPosition.x + treadLength / 2.0f, startPosition.y + (i * rise) - (rise / 2) + (wallHeight / 2), startPosition.z + (i * treadWidth) - (treadWidth / 4));
            stairWall.transform.localScale = new Vector3(0.1f, wallHeight, treadWidth);
            stairWall.transform.parent = stairWalls.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        stairWalls.SetActive(isWallVisible);
    }
}
