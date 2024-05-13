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
    public GameObject centerTree;
    public GameObject wall;
    //
    public List<GameObject> details;
    public int detailRange = 5;

    private Vector3 pivotPoint;

    // Start is called before the first frame update
    void Start()
    {
        // Move starting point forward in order to keep place for foot
        float radius = (treadWidth / 2.0f) / Mathf.Sin(angleTheta/2.0f * Mathf.Deg2Rad);
        float alpha = (180.0f - angleTheta) / 2.0f;
        pivotPoint = startingPoint.forward * radius;

        pivotPoint = Quaternion.AngleAxis(alpha, startingPoint.up) * pivotPoint;
        Debug.DrawRay(pivotPoint, Vector3.up * 10, Color.red);
        
        Vector3 initialPosition = new Vector3(startingPoint.position.x, 0, pivotPoint.z);
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
                                                    // Add the angle to position the stair step back to keep place for foot
            stepPosition = Quaternion.AngleAxis(angleTheta * i - 0.25f, startingPoint.up) * (stepPosition - pivotPoint) + pivotPoint;
            stairStep.SetActive(true);
            stairStep.transform.position = stepPosition;
            stairStep.transform.localScale = new Vector3(treadLength,  //+ 2 * supplementTreadLength, 
                                                         rise, 
                                                         treadWidth + 0.1f); // +0.1f is the supplement width

            stairStep.transform.Rotate(stairStep.transform.up, angleTheta * i);
            stairStep.transform.parent = transform;

            if (handrail != null)
            {
                float handrailHeight = 1.0f;
                GameObject handrailInstance = Instantiate(handrail);
                handrailInstance.transform.position = new Vector3(startingPoint.position.x + stairStep.transform.localScale.x / 2,
                                                                  0 + (i * rise) + (handrailHeight / 2),
                                                                  startingPoint.position.z);
                handrailInstance.transform.localScale = new Vector3(0.1f, handrailHeight, stairStep.transform.localScale.z);
                handrailInstance.transform.RotateAround(pivotPoint, handrailInstance.transform.up, angleTheta * i);
                handrailInstance.transform.parent = transform;

                // Detail
                if (i % detailRange == 0 && details.Count > 0)
                {
                    int index = Random.Range(0, details.Count - 1);
                    GameObject handrailObj = Instantiate(details[index]);
                    handrailObj.transform.position = new Vector3(handrailInstance.transform.position.x, handrailInstance.transform.position.y + handrailHeight / 2 + handrailObj.transform.localScale.y / 2, handrailInstance.transform.position.z);
                    handrailInstance.transform.parent = transform;
                }
            }
        }

        float wallHeight = numberOfSteps * rise;
        for (int i = 0; i < 360 / angleTheta; i++)
        {
            GameObject wallInstance = Instantiate(wall);
            wallInstance.transform.position = new Vector3(startingPoint.position.x - treadLength / 2,
                                                              0 + (wallHeight / 2),
                                                              startingPoint.position.z);
            wallInstance.transform.localScale = new Vector3(0.1f, wallHeight, treadWidth + 0.1f);
            wallInstance.transform.RotateAround(pivotPoint, wallInstance.transform.up, angleTheta * i);
            wallInstance.transform.parent = transform;
        }

        centerTree.transform.position = pivotPoint;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(pivotPoint, Vector3.up * 10, Color.red);
        wall.SetActive(isWallVisible);
    }
}
