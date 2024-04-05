using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System;

public class TrackerHandler : MonoBehaviour
{
    private float FootHeightDataLeft;
    private float FootHeightDataRight;

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


            Vector3 hipLeftPosition = new Vector3(trackerFrameData.currentBody.JointPositions3D[(int)JointId.HipLeft].X,
                                                    trackerFrameData.currentBody.JointPositions3D[(int)JointId.HipLeft].Y,
                                                    trackerFrameData.currentBody.JointPositions3D[(int)JointId.HipLeft].Z);

            Vector3 hipRightPosition = new Vector3(trackerFrameData.currentBody.JointPositions3D[(int)JointId.HipRight].X,
                                                    trackerFrameData.currentBody.JointPositions3D[(int)JointId.HipRight].Y,
                                                    trackerFrameData.currentBody.JointPositions3D[(int)JointId.HipRight].Z);

            Vector3 kneeLeftPosition = new Vector3(trackerFrameData.currentBody.JointPositions3D[(int)JointId.KneeLeft].X,
                                                    trackerFrameData.currentBody.JointPositions3D[(int)JointId.KneeLeft].Y,
                                                    trackerFrameData.currentBody.JointPositions3D[(int)JointId.KneeLeft].Z);

            Vector3 kneeRightPosition = new Vector3(trackerFrameData.currentBody.JointPositions3D[(int)JointId.KneeRight].X,
                                                    trackerFrameData.currentBody.JointPositions3D[(int)JointId.KneeRight].Y,
                                                    trackerFrameData.currentBody.JointPositions3D[(int)JointId.KneeRight].Z);

            Vector3 ankleLeftPosition = new Vector3(trackerFrameData.currentBody.JointPositions3D[(int)JointId.AnkleLeft].X,
                                                    trackerFrameData.currentBody.JointPositions3D[(int)JointId.AnkleLeft].Y,
                                                    trackerFrameData.currentBody.JointPositions3D[(int)JointId.AnkleLeft].Z);

            Vector3 ankleRightPosition = new Vector3(trackerFrameData.currentBody.JointPositions3D[(int)JointId.AnkleRight].X,
                                                    trackerFrameData.currentBody.JointPositions3D[(int)JointId.AnkleRight].Y,
                                                    trackerFrameData.currentBody.JointPositions3D[(int)JointId.AnkleRight].Z);

            // calculate bone length
            float hipKneeBoneLeft = Vector3.Distance(hipLeftPosition, kneeLeftPosition);
            float hipKneeBoneRight = Vector3.Distance(hipRightPosition, kneeRightPosition);
            float kneeAnkleBoneLeft = Vector3.Distance(kneeLeftPosition, ankleLeftPosition);
            float kneeAnkleBoneRight = Vector3.Distance(kneeRightPosition, ankleRightPosition);

            Vector3 hipKneeLeft = hipLeftPosition - kneeLeftPosition;
            Vector3 hipKneeRight = hipRightPosition - kneeRightPosition;
            Vector3 kneeAnkleLeft = ankleLeftPosition - kneeLeftPosition;
            Vector3 kneeAnkleRight = ankleRightPosition - kneeRightPosition;

            float angleLeft = Vector3.Angle(hipKneeLeft, kneeAnkleLeft);
            float angleRight = Vector3.Angle(hipKneeRight, kneeAnkleRight);

            // cal
            float opAngleLeft = 180f - angleLeft;
            float opAngleRight = 180f - angleRight;
            float tmpSegmentLeft = Mathf.Cos(opAngleLeft / 180f * Mathf.PI) * hipKneeBoneLeft;
            float tmpSegmentRight = Mathf.Cos(opAngleRight / 180f * Mathf.PI) * hipKneeBoneRight;
            this.FootHeightDataLeft = hipKneeBoneLeft - tmpSegmentLeft;
            this.FootHeightDataRight = hipKneeBoneRight - tmpSegmentRight;

            // Debug.Log("Angle left: " + angleLeft + ", Angle right: " + angleRight);
            // Debug.Log("Computed height left: " + heightLeft + ", Height right: " + heightRight);

            //if (fps < 0.00006)
            //{
            //    startTick = DateTime.Now.Ticks;
            //}
            ////
            //fps += 1.0f;
            //TimeSpan elapse = new TimeSpan(DateTime.Now.Ticks - startTick);
            //if (elapse.TotalSeconds > 10.0)
            //{

            //    fps = fps / (float)elapse.TotalSeconds;
            //    Debug.Log("FPS Kinect: " + fps);
            //    fps = 0;
            //    startTick = DateTime.Now.Ticks;
            //}
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

    public float getFootHeightLeft()
    {
        return this.FootHeightDataLeft;
    }

    public float getFootHeightRight()
    {
        return this.FootHeightDataRight;
    }

    public float getFootDeltaHeight() {
        return this.FootHeightDataRight - this.FootHeightDataLeft;
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
