using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralStair : MonoBehaviour
{
    public int numberOfSteps;
    public float rise;
    public float treadLength = 1f; // 1m
    public float supplementTreadLength = 0.5f;
    public Transform startingPoint;
    public float treadWidth;
    public float angleTheta = 30;
    public bool isWallVisible = true;
    public GameObject wallCylinder;
    public GameObject stairStepObject;
    public GameObject handrail;

    private Vector3 pivotPoint;
    private GameObject wall;

    // Start is called before the first frame update
    void Start()
    {
        float radius = (treadWidth / 2.0f) / Mathf.Sin(angleTheta/2.0f * Mathf.Deg2Rad);
        float alpha = (180.0f - angleTheta) / 2.0f;
        pivotPoint = startingPoint.forward * radius;
        pivotPoint = Quaternion.AngleAxis(alpha, startingPoint.up) * pivotPoint + startingPoint.position;
        Debug.DrawRay(pivotPoint, Vector3.up * 10, Color.red);
        
        Vector3 initialPosition = new Vector3(startingPoint.position.x, startingPoint.position.y, pivotPoint.z);
        for (int i = 1; i <= numberOfSteps; i++)
        {
            GameObject stairStep;
            if (stairStepObject != null)
                stairStep = Instantiate(stairStepObject);
            else
                stairStep = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            Vector3 stepPosition = new Vector3(initialPosition.x, 
                                               initialPosition.y + (i * rise) - (rise / 2), 
                                               initialPosition.z);

            stepPosition = Quaternion.AngleAxis(angleTheta * i, startingPoint.up) * (stepPosition - pivotPoint) + pivotPoint;
            stairStep.SetActive(true);
            stairStep.transform.position = stepPosition;
            stairStep.transform.localScale = new Vector3(treadLength + 2 * supplementTreadLength, 
                                                         rise, 
                                                         treadWidth + 0.1f); // +0.1f is the supplement width

            stairStep.transform.Rotate(stairStep.transform.up, angleTheta * i);
            stairStep.transform.parent = transform;

            if (handrail != null)
            {
                float handrailHeight = 1.0f;
                GameObject handrailInstance = Instantiate(handrail);
                handrailInstance.transform.position = new Vector3(startingPoint.position.x + stairStep.transform.localScale.x / 2,
                                                                  startingPoint.position.y + (i * rise) + (handrailHeight / 2),
                                                                  startingPoint.position.z);
                handrailInstance.transform.localScale = new Vector3(0.1f, handrailHeight, stairStep.transform.localScale.z);
                handrailInstance.transform.RotateAround(pivotPoint, handrailInstance.transform.up, angleTheta * i);
                handrailInstance.transform.parent = transform;
            }
        }

        float wallHeight = numberOfSteps * rise;
        if (wallCylinder != null)
            wall = wallCylinder;
        else
            wall = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        
        wall.transform.position = new Vector3(pivotPoint.x, pivotPoint.y + wallHeight/2.0f, pivotPoint.z);
        wall.transform.localScale = new Vector3(2*(radius+treadLength/2), wallHeight/2, 2* (radius + treadLength / 2));
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(pivotPoint, Vector3.up * 10, Color.red);
        wall.SetActive(isWallVisible);
    }
}
