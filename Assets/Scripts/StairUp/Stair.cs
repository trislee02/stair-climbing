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
    private GameObject handrails;
    public float wallHeight;
    public bool isWallVisible = true;

    public int wallStep = 1;
    public GameObject stairSample;
    public GameObject wallSample;
    public GameObject handrailSample;

    public GameObject[] enviromentSamples;
    public int enviromentSpawningStepDistance = 5;

    // Start is called before the first frame update
    void Start()
    {
        int envSampleLen = enviromentSamples.Length;
        //Vector2 stairTextureScale = new Vector2(Mathf.Ceil(treadLength * 6), Mathf.Ceil(treadWidth * 3));
        Vector2 wallTextureScale = new Vector2(Mathf.Ceil(treadWidth * wallStep * 2.6f), Mathf.Ceil(wallHeight * 2.8f));
        Vector2 handrailTextureScale = new Vector2(2f, 4f);

        stairWalls = new GameObject("StairWalls");
        handrails = new GameObject("Handrails");
        for (int i = 1; i <= numberOfSteps; i++)
        {
            Vector3 stepPosition = new Vector3(startPosition.x, startPosition.y + (i * rise) - (rise / 2), startPosition.z + (i * treadWidth) - (treadWidth / 4));
            GameObject stairStep;
            if (stairSample != null)
            {
                stairStep = Instantiate(stairSample);
            }
            else
            {
                stairStep = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            stairStep.transform.position = stepPosition;
            stairStep.transform.localScale = new Vector3(treadLength, rise, treadWidth);
            stairStep.transform.parent = transform;
            stairStep.transform.localScale = new Vector3(treadLength, rise, treadWidth);
            //stairStep.GetComponent<Renderer>().material.mainTextureScale = stairTextureScale;

            if (i % wallStep == 0)
            {
                GameObject stairWall;
                if (wallSample != null)
                {
                    stairWall = Instantiate(wallSample);
                }
                else
                {
                    stairWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                }
                //stairWall.transform.position = new Vector3(startPosition.x + treadLength / 2.0f, startPosition.y + ((i-4+1) * rise) - (rise / 2) + (wallHeight / 2), startPosition.z + ((i-4+1) * treadWidth) - (treadWidth / 2));
                stairWall.transform.position = new Vector3(startPosition.x + 0.6f + (i == wallStep ? 1.5f : 0.05f), stepPosition.y + wallHeight / 2f - rise * wallStep, stepPosition.z - treadWidth * wallStep / 2f + (treadWidth / 2f));
                stairWall.transform.localScale = new Vector3((i == wallStep ? 3f : 0.1f), wallHeight, treadWidth * wallStep + 0.02f);
                stairWall.transform.parent = stairWalls.transform;
                //
                stairWall.GetComponent<Renderer>().material.mainTextureScale = wallTextureScale;
            }

            if (handrailSample != null)
            {
                float handrailHeight = 1.0f;
                GameObject handrailInstance = Instantiate(handrailSample);
                handrailInstance.transform.position = new Vector3(startPosition.x - stairStep.transform.localScale.x / 2, startPosition.y + (i * rise) + (handrailHeight / 2), startPosition.z + (i * treadWidth) - (treadWidth / 4));
                handrailInstance.transform.localScale = new Vector3(0.1f, handrailHeight, stairStep.transform.localScale.z);
                handrailInstance.transform.parent = handrails.transform;
                handrailInstance.GetComponent<Renderer>().material.mainTextureScale = handrailTextureScale;
            }

            if (i % enviromentSpawningStepDistance == 0 && envSampleLen > 0)
            {
                int rdIndex = Mathf.FloorToInt(Random.Range(0f, 0.99f) * envSampleLen);
                GameObject envObj = Instantiate(enviromentSamples[rdIndex]);
                envObj.transform.position = new Vector3(startPosition.x - stairStep.transform.localScale.x / 2 - 6.5f, startPosition.y + (i * rise) - Random.Range(1f, 5f), startPosition.z + (i * treadWidth) - (treadWidth / 4));
                envObj.transform.parent = transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        stairWalls.SetActive(isWallVisible);
    }
}