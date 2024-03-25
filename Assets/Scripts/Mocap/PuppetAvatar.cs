using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using System.Text;

public class PuppetAvatar : MonoBehaviour
{
    public TrackerHandler KinectDevice;

    private void Start()
    {
        //
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        float data = KinectDevice.getFootDeltaHeight();
        //print("Height: " + data);
    }
}
