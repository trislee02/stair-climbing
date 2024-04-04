using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System;

public class TrackerHandler : MonoBehaviour
{
    private float footHeightData;

    float fps = 0;
    long startTick;

    // Start is called before the first frame update
    void Awake()
    {

    }

    public void updateTracker(BackgroundData trackerFrameData)
    {
        //this is an array in case you want to get the n closest bodies
        //int closestBody = findClosestTrackedBody(trackerFrameData);

        // render the closest body
        if (trackerFrameData.CouldHasData) {
            //updateBody(trackerFrameData.Left, trackerFrameData.Right);
            this.footHeightData = trackerFrameData.Left - trackerFrameData.Right;

            if (fps < 0.00006)
            {
                startTick = DateTime.Now.Ticks;
            }
            //
            fps += 1.0f;
            TimeSpan elapse = new TimeSpan(DateTime.Now.Ticks - startTick);
            if (elapse.TotalSeconds > 10.0)
            {

                fps = fps / (float)elapse.TotalSeconds;
                Debug.Log("FPS Kinect: " + fps);
                fps = 0;
                startTick = DateTime.Now.Ticks;
            }
        }
    }

    //private int findClosestTrackedBody(BackgroundData trackerFrameData)
    //{
    //    int closestBody = -1;
    //    const float MAX_DISTANCE = 5000.0f;
    //    float minDistanceFromKinect = MAX_DISTANCE;
    //    for (int i = 0; i < (int)trackerFrameData.NumOfBodies; i++)
    //    {
    //        var pelvisPosition = trackerFrameData.Bodies[i].JointPositions3D[(int)JointId.Pelvis];
    //        Vector3 pelvisPos = new Vector3((float)pelvisPosition.X, (float)pelvisPosition.Y, (float)pelvisPosition.Z);
    //        if (pelvisPos.magnitude < minDistanceFromKinect)
    //        {
    //            closestBody = i;
    //            minDistanceFromKinect = pelvisPos.magnitude;
    //        }
    //    }
    //    return closestBody;
    //}

    public float getFootDeltaHeight()
    {
        return this.footHeightData;
    }

    //public void updateBody(float left, float right)
    //{
    //    //int ankleLeftId = (int)JointId.FootLeft;
    //    //int ankleRightId = (int)JointId.FootRight;
    //    ////Vector3 ankleLeftPos = new Vector3(body.JointPositions3D[ankleLeftId].X, -body.JointPositions3D[ankleLeftId].Y, body.JointPositions3D[ankleLeftId].Z);
    //    ////Vector3 ankleRightPos = new Vector3(body.JointPositions3D[ankleRightId].X, -body.JointPositions3D[ankleRightId].Y, body.JointPositions3D[ankleRightId].Z);

    //    //float ankleLeftY = body.JointPositions3D[ankleLeftId].Y;
    //    //float ankleRightY = body.JointPositions3D[ankleRightId].Y;

    //    this.footHeightData = left - right;
    //}
}
