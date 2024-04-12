using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainStair : MonoBehaviour
{
    public int numberOfSteps;
    public float rise;
    public float treadWidth;
    public float treadLength;
    public Vector3 startPosition;

    public GameObject[] stairSamples;

    // Start is called before the first frame update
    void Start()
    {
        if (stairSamples.Length == 0) return;
        for (int i = 1; i <= numberOfSteps; i++)
        {
            Vector3 stepPosition = new Vector3(startPosition.x, startPosition.y + (i * rise) - (rise / 2), startPosition.z + (i * treadWidth) - (treadWidth / 4));
            int rdIndex = Mathf.FloorToInt(Random.Range(0, stairSamples.Length-0.001f));
            GameObject stairStep = Instantiate(stairSamples[rdIndex]);
            stairStep.transform.position = stepPosition;
            stairStep.transform.parent = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
