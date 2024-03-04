using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stair : MonoBehaviour
{
    public int numberOfSteps;
    public float rise;
    public float treadWidth;
    public Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i <= numberOfSteps; i++)
        {
            Vector3 stepPosition = new Vector3(startPosition.x, startPosition.y + (i * rise) - (rise / 2), startPosition.z + (i * treadWidth) - (treadWidth / 4));
            GameObject stairStep = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stairStep.transform.position = stepPosition;
            stairStep.transform.localScale = new Vector3(1f, rise, treadWidth);
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
