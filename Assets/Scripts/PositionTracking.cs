using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class PositionTracking : MonoBehaviour
{
    [SerializeField]
    private string loggerPath;

    private MyLogger logger;

    private UnityEngine.Color[] COLORS = {
        UnityEngine.Color.red,
        UnityEngine.Color.blue,
        UnityEngine.Color.green,
        UnityEngine.Color.cyan,
        UnityEngine.Color.magenta,
        UnityEngine.Color.white,
        UnityEngine.Color.black,
    };

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
        //GameObject headPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //headPoint.transform.position = pos;
        //headPoint.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        //System.Random rnd = new System.Random();
        //int index = rnd.Next(COLORS.Length);
        //headPoint.GetComponent<Renderer>().material.color = COLORS[index];
    }

    void OnApplicationQuit()
    {
        logger.Save();
    }
}
