using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;

public class TrackerHandler : MonoBehaviour
{
    private float footHeightData;

    // Start is called before the first frame update
    void Awake()
    {

    }

    public void updateTracker(BackgroundData trackerFrameData)
    {
        //this is an array in case you want to get the n closest bodies
        int closestBody = findClosestTrackedBody(trackerFrameData);

        // render the closest body
        Body skeleton = trackerFrameData.Bodies[closestBody];
        updateBody(skeleton);
    }

    private int findClosestTrackedBody(BackgroundData trackerFrameData)
    {
        int closestBody = -1;
        const float MAX_DISTANCE = 5000.0f;
        float minDistanceFromKinect = MAX_DISTANCE;
        for (int i = 0; i < (int)trackerFrameData.NumOfBodies; i++)
        {
            var pelvisPosition = trackerFrameData.Bodies[i].JointPositions3D[(int)JointId.Pelvis];
            Vector3 pelvisPos = new Vector3((float)pelvisPosition.X, (float)pelvisPosition.Y, (float)pelvisPosition.Z);
            if (pelvisPos.magnitude < minDistanceFromKinect)
            {
                closestBody = i;
                minDistanceFromKinect = pelvisPos.magnitude;
            }
        }
        return closestBody;
    }

    public float getFootDeltaHeight()
    {
        return this.footHeightData;
    }

    public void updateBody(Body body)
    {
        int ankleLeftId = (int)JointId.AnkleLeft;
        int ankleRightId = (int)JointId.AnkleRight;
        //Vector3 ankleLeftPos = new Vector3(body.JointPositions3D[ankleLeftId].X, -body.JointPositions3D[ankleLeftId].Y, body.JointPositions3D[ankleLeftId].Z);
        //Vector3 ankleRightPos = new Vector3(body.JointPositions3D[ankleRightId].X, -body.JointPositions3D[ankleRightId].Y, body.JointPositions3D[ankleRightId].Z);

        float ankleLeftY = body.JointPositions3D[ankleLeftId].Y;
        float ankleRightY = body.JointPositions3D[ankleRightId].Y;

        this.footHeightData = ankleLeftY - ankleRightY;
    }
}
