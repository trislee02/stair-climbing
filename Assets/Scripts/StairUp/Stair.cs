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

    public int wallStep = 1;
    public GameObject stairSample;
    public GameObject wallSample;
    public GameObject handrailSample;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 stairTextureScale = new Vector2(Mathf.Ceil(treadLength*3), Mathf.Ceil(treadWidth*3));
        Vector2 wallTextureScale = new Vector2(Mathf.Ceil(treadWidth * wallStep), Mathf.Ceil(wallHeight));

        stairWalls = new GameObject("StairWalls");
        for (int i = 1; i <= numberOfSteps; i++)
        {
            Vector3 stepPosition = new Vector3(startPosition.x, startPosition.y + (i * rise) - (rise / 2), startPosition.z + (i * treadWidth) - (treadWidth / 4));
            GameObject stairStep;
            if (stairSample != null)
            {
                stairStep = Instantiate(stairSample);
            } else {
                stairStep = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            stairStep.transform.position = stepPosition;
            stairStep.transform.localScale = new Vector3(treadLength, rise, treadWidth);
            stairStep.transform.parent = transform;
            stairStep.transform.localScale = new Vector3(treadLength, rise, treadWidth);
            //stairStep.GetComponent<Renderer>().material = stairMaterial;
            stairStep.GetComponent<Renderer>().material.mainTextureScale = stairTextureScale;
            //stairStep.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

            if (i % wallStep == 0)
            {
                GameObject stairWall;
                if (wallSample != null)
                {
                    stairWall = Instantiate(wallSample);
                } else
                {
                    stairWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                }
                //stairWall.transform.position = new Vector3(startPosition.x + treadLength / 2.0f, startPosition.y + ((i-4+1) * rise) - (rise / 2) + (wallHeight / 2), startPosition.z + ((i-4+1) * treadWidth) - (treadWidth / 2));
                stairWall.transform.position = new Vector3(startPosition.x + 0.6f + (i == wallStep ? 1.5f : 0.05f), stepPosition.y + wallHeight/2f - rise * wallStep, stepPosition.z - treadWidth * wallStep / 2f + (treadWidth/2f));
                stairWall.transform.localScale = new Vector3((i == wallStep ? 3f : 0.1f), wallHeight, treadWidth*wallStep);
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
                handrailInstance.transform.parent = transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        stairWalls.SetActive(isWallVisible);
    }
}
