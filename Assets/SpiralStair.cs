using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralStair : MonoBehaviour
{
    public int numberOfSteps;
    public float rise;
    public float treadWidth;
    public float treadLength = 1f; // 1m
    public Vector3 startPosition;
    public float angle = 30;
    public Vector3 pivotPoint;

    // Start is called before the first frame update
    void Start()
    {
        pivotPoint = new Vector3(startPosition.x + (treadLength / 2.0f), startPosition.y, startPosition.z + treadWidth / 2.0f);
        for (int i = 1; i <= numberOfSteps; i++)
        {
            Vector3 stepPosition = new Vector3(startPosition.x, startPosition.y + (i * rise) - (rise / 2), startPosition.z + treadWidth/2.0f);
            GameObject stairStep = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stairStep.transform.position = stepPosition;
            stairStep.transform.localScale = new Vector3(treadLength, rise, treadWidth);
            stairStep.transform.RotateAround(pivotPoint, transform.up, angle * (i - 1));
            stairStep.transform.parent = transform;
            Color randomColor = new Color(
                Random.Range(0f, 1f), // Red
                Random.Range(0f, 1f), // Green
                Random.Range(0f, 1f)  // Blue
            );

            // Change the color of the cube to the random color
            stairStep.GetComponent<Renderer>().material.color = randomColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
