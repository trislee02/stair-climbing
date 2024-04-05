using Microsoft.Azure.Kinect.BodyTracking;
using System;

public class BackgroundData
{
    public static readonly JointId LEFT_JOINT_ID = JointId.FootLeft;
    public static readonly JointId RIGHT_JOINT_ID = JointId.FootRight;
    public static readonly int MAX_BODY_JOINT_SIZE = 100;

    public Body CurrentBody;
    public bool CouldHasData { get; set; }

    public BackgroundData()
    {
        this.CouldHasData = false;
        this.CurrentBody = new Body(BackgroundData.MAX_BODY_JOINT_SIZE);
    }
}

