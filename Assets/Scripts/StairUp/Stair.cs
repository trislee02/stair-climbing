using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stair : MonoBehaviour
{
    public int numberOfSteps;
    public float stepHeight;
    public float stepWidth;
    public Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i <= numberOfSteps; i++)
        {
            Vector3 stepPosition = new Vector3(startPosition.x, startPosition.y + (i * stepHeight) - (stepHeight / 2), startPosition.z + (i * stepWidth) + (stepWidth / 2));
            GameObject stairStep = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stairStep.transform.position = stepPosition;
            stairStep.transform.localScale = new Vector3(1f, stepHeight, stepWidth);
            stairStep.transform.parent = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
