using Microsoft.Azure.Kinect.BodyTracking;
using System;

public class BackgroundData
{
    public static readonly JointId LEFT_JOINT_ID = JointId.FootLeft;
    public static readonly JointId RIGHT_JOINT_ID = JointId.FootRight;
    public static readonly int MAX_BODY_JOINT_SIZE = 100;

    public float Left { get; set; }
    public float Right { get; set; }
    public bool CouldHasData { get; set; }

    public BackgroundData()
    {
        this.CouldHasData = false;
        this.Left = 0;
        this.Right = 0;
    }
}

