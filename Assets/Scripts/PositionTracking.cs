using System;
using System.Collections.Generic;
using UnityEngine;

public class PositionTracking : MonoBehaviour
{
    [SerializeField]
    private string loggerPath;

    private MyLogger logger;

    // Start is called before the first frame update
    void Start()
    {
        logger = new MyLogger(loggerPath, -1);
        logger.Start(new string[] { "PosX", "PosY", "PosZ", "QuaX", "QuaY", "QuaZ", "QuaW" });
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        Quaternion quat = transform.rotation;
        List<float> nums = new List<float> { pos.x, pos.y, pos.z, quat.x, quat.y, quat.z, quat.w };
        logger.Push(nums);
    }

    void OnApplicationQuit()
    {
        logger.Save();
    }
}
