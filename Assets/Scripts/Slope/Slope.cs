using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slope : MonoBehaviour
{
    public float stepWidth;
    public float stepRise;
    public float length;
    public float width;
    public Vector3 startPosition;
    public float wallHeight;
    public float slopeThickness = 0.1f;
    public float handrailHeight = 1.0f;

    public int wallStep = 1;
    public GameObject wallSample;
    public GameObject handrailSample;
    public GameObject slopeSample;

    public GameObject[] enviromentSamples;
    public int enviromentSpawningStepDistance = 5;

    // Start is called before the first frame update
    void Start()
    {
        int envSampleLen = enviromentSamples.Length;
        Vector2 wallTextureScale = new Vector2(Mathf.Ceil(width * wallStep), Mathf.Ceil(wallHeight));
        Vector2 slopeTextureScale = new Vector2(Mathf.Ceil(width), Mathf.Ceil(length));
        float alpha = Mathf.Atan(stepRise / stepWidth);
        float sinAlpha = Mathf.Sin(alpha);
        float cosAlpha = Mathf.Cos(alpha);

        Vector3 slopePosition = new Vector3(startPosition.x, startPosition.y + (sinAlpha * length/2) - slopeThickness - cosAlpha * slopeThickness, startPosition.z + (cosAlpha * length / 2) + stepRise);
        GameObject slopeObj;
        if (slopeSample != null)
        {
            slopeObj = Instantiate(slopeSample);
        }
        else
        {
            slopeObj = GameObject.CreatePrimitive(PrimitiveType.Cube); 
        }
        slopeObj.transform.position = slopePosition;
        slopeObj.transform.Rotate(-alpha/Mathf.PI*180f, 0, 0);
        slopeObj.transform.localScale = new Vector3(width, slopeThickness, length);
        slopeObj.transform.parent = transform;
        slopeObj.GetComponent<Renderer>().material.mainTextureScale = slopeTextureScale;

        for (int i = 1; i <= length; i++)
        {
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
                stairWall.transform.position = new Vector3(startPosition.x + 1.9f + (i == wallStep ? 1.5f : 0.05f), startPosition.y + wallHeight / 2 + sinAlpha*(i-1) - stepRise, startPosition.z + cosAlpha * (i + 1 - wallStep));
                stairWall.transform.localScale = new Vector3((i == wallStep ? 3f : 0.1f), wallHeight, cosAlpha * wallStep);
                stairWall.transform.parent = transform;
                //
                stairWall.GetComponent<Renderer>().material.mainTextureScale = wallTextureScale;
            }

            if (handrailSample != null)
            {
                GameObject handrailInstance = Instantiate(handrailSample);
                handrailInstance.transform.position = new Vector3(startPosition.x - width / 2f, startPosition.y + handrailHeight / 2 + sinAlpha * (i - 1) - stepRise, startPosition.z + cosAlpha * (i));
                handrailInstance.transform.localScale = new Vector3(0.1f, handrailHeight, cosAlpha * wallStep);
                handrailInstance.transform.parent = transform;
            }

            if (i % enviromentSpawningStepDistance == 0 && envSampleLen > 0)
            {
                int rdIndex = Mathf.FloorToInt(Random.Range(0f, 0.99f) * envSampleLen);
                GameObject envObj = Instantiate(enviromentSamples[rdIndex]);
                envObj.transform.position = new Vector3(startPosition.x - 6.5f - width / 2f, startPosition.y + (sinAlpha * (i - 1) - stepRise) - Random.Range(1f, 5f), startPosition.z + cosAlpha * (i));
                envObj.transform.parent = transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //
    }
}
