using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralStair : MonoBehaviour
{
    public int numberOfSteps;
    public float rise;
    public float treadLength = 1f; // 1m
    public Transform startingPoint;
    public float treadWidth;
    public float angleTheta = 30;
    public bool isWallVisible = true;

    private Vector3 pivotPoint;
    private GameObject wall;

    // Start is called before the first frame update
    void Start()
    {


        //for (int i = 1; i <= numberOfSteps; i++)
        //{
        //    Vector3 stepPosition = new Vector3(startingPoint.x, startingPoint.y + (i * rise) - (rise / 2), startingPoint.z + treadWidth / 2.0f);
        //    GameObject stairStep = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    stairStep.transform.position = stepPosition;
        //    stairStep.transform.localScale = new Vector3(treadLength, rise, treadWidth);
        //    stairStep.transform.RotateAround(pivotPoint, transform.up, angleTheta * (i - 1));
        //    stairStep.transform.parent = transform;
        //    Color randomColor = new Color(
        //        Random.Range(0f, 1f), // Red
        //        Random.Range(0f, 1f), // Green
        //        Random.Range(0f, 1f)  // Blue
        //    );

        //    // Change the color of the cube to the random color
        //    stairStep.GetComponent<Renderer>().material.color = randomColor;
        //}

        float radius = (treadWidth / 2.0f) / Mathf.Sin(angleTheta/2.0f * Mathf.Deg2Rad);
        float alpha = (180.0f - angleTheta) / 2.0f;
        pivotPoint = startingPoint.forward * radius;
        pivotPoint = Quaternion.AngleAxis(alpha, startingPoint.up) * pivotPoint + startingPoint.position;
        Debug.DrawRay(pivotPoint, Vector3.up * 10, Color.red);

        Vector3 initialPosition = new Vector3(startingPoint.position.x, startingPoint.position.y, pivotPoint.z);
        for (int i = 1; i <= numberOfSteps; i++)
        {
            Vector3 stepPosition = new Vector3(initialPosition.x, initialPosition.y + (i * rise) - (rise / 2), initialPosition.z);
            stepPosition = Quaternion.AngleAxis(angleTheta * i, startingPoint.up) * (stepPosition - pivotPoint) + pivotPoint;
            GameObject stairStep = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stairStep.transform.position = stepPosition;
            stairStep.transform.localScale = new Vector3(treadLength, rise, treadWidth + 0.1f);
            stairStep.transform.Rotate(stairStep.transform.up, angleTheta * i);
            stairStep.transform.parent = transform;
        }

        float wallHeight = numberOfSteps * rise;
        wall = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wall.transform.position = new Vector3(pivotPoint.x, pivotPoint.y + wallHeight/2.0f, pivotPoint.z);
        wall.transform.localScale = new Vector3(2*(radius - treadLength/2.0f), wallHeight/2, 2*(radius - treadLength / 2.0f));
         

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(pivotPoint, Vector3.up * 10, Color.red);
        wall.SetActive(isWallVisible);
    }
}
