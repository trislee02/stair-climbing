using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class NewSpiralStairLegAnimation : MonoBehaviour
{
    [SerializeField]
    private string sensorLoggerPath;

    [SerializeField]
    private float maxFootHeight;

    [SerializeField]
    private float minFootHeight;

    [SerializeField]
    private float stepWidth;

    [SerializeField]
    private float stepRise;
    
    [SerializeField]
    private float bodyLiftingSpeed = 1.0f; // Speed of lifting the body when a leg is pushing down

    [SerializeField]
    private float moveBackwardLegSpeed = 0.5f;

    [SerializeField]
    private float moveUpwardLegSpeed = 1f;

    [SerializeField]
    private float curveLiftFoot = 7.0f; // Must be a number larger than 1 

    [SerializeField]
    private ParticleSystem stepRipple;

    [SerializeField]
    private SpiralStair spiralStair;

    [SerializeField]
    private bool isDebug;

    [Range(0.01f, 0.05f)]
    [SerializeField]
    private float footHeightDebug;

    [SerializeField]
    private OVRCameraRig ovrCameraRig;


    private float maxDiffFootHeight;
    private float risePerRealHeightUnit;
    private float widthPerRealHeightUnit;
    private float curveLiftCoefficient;
    private float bodyLiftingConstant;

    private Animator animator;
    private DataManager dataManager;

    private Vector3 initialLeftFootIKLocalPosition = new Vector3(-0.085f, 0.1f, -0.004f); // Local position
    private Vector3 initialRightFootIKLocalPosition = new Vector3(0.085f, 0.1f, -0.004f); // Local position

    private bool isInitFootHeight = true;

    private float currentLeftDiffFootHeight; // Real height
    private float currentRightDiffFootHeight; // Real height

    private bool isLeftAbove;
    private bool isRightAbove;

    private Vector3 currentLeftIKPosition;
    private Vector3 currentRightIKPosition;

    private Vector3 avatarStartingPosition;
    private float rotationAngle;

    private Quaternion lastVRCameraRotation;
    private Quaternion lastBodyRotation;
    private Quaternion destinationVRCameraRotation;
    private Quaternion destinationBodyRotation;
    private Quaternion startingVRCameraRotation;
    private Quaternion startingBodyRotation;

    private MyLogger sensorLogger;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        dataManager = GetComponent<DataManager>();

        maxDiffFootHeight = maxFootHeight - minFootHeight;
        risePerRealHeightUnit = stepRise / maxDiffFootHeight;
        widthPerRealHeightUnit = stepWidth / maxDiffFootHeight;
        curveLiftCoefficient = stepWidth / (float)Math.Pow(stepRise, curveLiftFoot);
        bodyLiftingConstant = stepRise - bodyLiftingSpeed * stepRise;

        rotationAngle = spiralStair.angleTheta;

        lastVRCameraRotation = transform.parent.localRotation * transform.localRotation * ovrCameraRig.transform.localRotation;
        lastBodyRotation = transform.parent.localRotation * transform.localRotation;

        sensorLogger = new MyLogger(sensorLoggerPath, -1);
        sensorLogger.Start(new string[] { "LeftRoll", "LeftFootHeight", "RightRoll", "RightFootHeight" });
    }

    // Update is called once per frame
    void Update()
    {
        // Play ripple effect when a foot is above a step
        if (!isRightAbove && !isLeftAbove)
        {
            if (currentLeftDiffFootHeight >= maxDiffFootHeight)
            {
                isLeftAbove = true;
                stepRipple.transform.position = currentLeftIKPosition - new Vector3(0, 0.1f, 0);
            }
            else if (currentRightDiffFootHeight >= maxDiffFootHeight)
            {
                isRightAbove = true;
                stepRipple.transform.position = currentRightIKPosition - new Vector3(0, 0.1f, 0);
            }
            if (isLeftAbove || isRightAbove)
            {
                stepRipple.Play();
                avatarStartingPosition = transform.parent.position;
                startingVRCameraRotation = ovrCameraRig.transform.rotation;
                startingBodyRotation = transform.rotation;

                destinationVRCameraRotation = Quaternion.AngleAxis(rotationAngle, ovrCameraRig.transform.up) * startingVRCameraRotation;
                destinationBodyRotation = Quaternion.AngleAxis(rotationAngle, transform.up) * startingBodyRotation;
            }
        }

        Vector3 destinationAvatarPosition = avatarStartingPosition
                                        + transform.parent.forward * stepWidth
                                        + transform.parent.up * stepRise;

        // Move the character when a foot is pushing down, i.e. one foot is above the stair
        if (isLeftAbove || isRightAbove)
        {
            float scaleHeightLeftFoot = currentLeftDiffFootHeight <= 0 ? 0 : risePerRealHeightUnit;
            float scaleHeightRightFoot = currentRightDiffFootHeight <= 0 ? 0 : risePerRealHeightUnit;

            float deltaLeftHeight = scaleHeightLeftFoot * currentLeftDiffFootHeight;
            deltaLeftHeight = bodyLiftingSpeed * deltaLeftHeight + bodyLiftingConstant;
            deltaLeftHeight = deltaLeftHeight < 0 ? 0 : deltaLeftHeight;
            float deltaLeftDistance = stepWidth / stepRise * deltaLeftHeight;

            float deltaRightHeight = scaleHeightRightFoot * currentRightDiffFootHeight;
            deltaRightHeight = bodyLiftingSpeed * deltaRightHeight + bodyLiftingConstant;
            deltaRightHeight = deltaRightHeight < 0 ? 0 : deltaRightHeight;
            float deltaRightDistance = stepWidth / stepRise * deltaRightHeight;


            Vector3 newAvatarPosition;
            if (isLeftAbove)
            {
                newAvatarPosition = destinationAvatarPosition - transform.parent.forward * deltaLeftDistance
                                                              - transform.parent.up * deltaLeftHeight;
            }
            else
            {
                newAvatarPosition = destinationAvatarPosition - transform.parent.forward * deltaRightDistance
                                                              - transform.parent.up * deltaRightHeight;
            }
            transform.parent.position = newAvatarPosition;

            // Ratio between the current position and the destination position
            float rotationStep = (transform.parent.position - avatarStartingPosition).magnitude / (destinationAvatarPosition - avatarStartingPosition).magnitude;
            ovrCameraRig.transform.rotation = Quaternion.Slerp(startingVRCameraRotation, destinationVRCameraRotation, rotationStep);
            transform.rotation = Quaternion.Slerp(startingBodyRotation, destinationBodyRotation, rotationStep);
        }

        // Update the last VR camera's rotation
        lastVRCameraRotation = transform.parent.localRotation * transform.localRotation * ovrCameraRig.transform.localRotation;
        lastBodyRotation = transform.parent.localRotation * transform.localRotation;

        if (isLeftAbove && currentLeftDiffFootHeight <= 0)
        {
            Debug.Log("Rotate Left Foot");
            isLeftAbove = false;

            transform.parent.Rotate(transform.parent.up, rotationAngle);
            // Keep the VR camera's rotation unchanged
            ovrCameraRig.transform.localRotation = Quaternion.Inverse(transform.parent.localRotation * transform.localRotation) * lastVRCameraRotation;
            transform.localRotation = Quaternion.Inverse(transform.parent.localRotation) * lastBodyRotation;
        }
        if (isRightAbove && currentRightDiffFootHeight <= 0)
        {
            Debug.Log("Rotate Right Foot");
            isRightAbove = false;

            transform.parent.Rotate(transform.parent.up, rotationAngle);
            // Keep the VR camera's rotation unchanged
            ovrCameraRig.transform.localRotation = Quaternion.Inverse(transform.parent.localRotation * transform.localRotation) * lastVRCameraRotation;
            transform.localRotation = Quaternion.Inverse(transform.parent.localRotation) * lastBodyRotation;
        }

        //Debug.DrawRay(transform.parent.Find("CenterEyeAnchor").transform.position, transform.parent.Find("CenterEyeAnchor").transform.forward * 10, Color.red);

        //Debug.DrawRay(ovrCameraRig.transform.position, ovrCameraRig.transform.forward * 10, Color.yellow);
        //Debug.DrawRay(transform.parent.position, transform.parent.forward * 10, Color.green);
        //Debug.DrawRay(transform.parent.position, transform.parent.up * 10, Color.blue);
        //Debug.Log("CenterEyeAnchor rotation: " + ovrCameraRig.centerEyeAnchor.localRotation);


    }



    private void OnApplicationQuit()
    {
        sensorLogger.Save();
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            if (isInitFootHeight)
            {
                isInitFootHeight = false;
            }
            else
            {
                float roll1Logging = 0;
                float roll2Logging = 0;
                float dataLeftFootHeight = dataManager.getFootHeight(DataManager.LEFT_LEG, out roll1Logging);
                float dataRightFootHeight = dataManager.getFootHeight(DataManager.RIGHT_LEG, out roll2Logging);
                if (isDebug)
                {
                    dataRightFootHeight = footHeightDebug;
                }

                // Add to log
                List<float> nums = new List<float> { roll1Logging, dataLeftFootHeight, roll2Logging, dataRightFootHeight };
                sensorLogger.Push(nums);

                Debug.Log("Data Left height: " + dataLeftFootHeight + "; Data Right height: " + dataRightFootHeight);

                // Clip foot height
                currentLeftDiffFootHeight = Mathf.Clamp(dataLeftFootHeight, minFootHeight, maxFootHeight);
                currentRightDiffFootHeight = Mathf.Clamp(dataRightFootHeight, minFootHeight, maxFootHeight);

                currentLeftDiffFootHeight -= minFootHeight;
                currentRightDiffFootHeight -= minFootHeight;

                Debug.Log("Diff Left height: " + currentLeftDiffFootHeight + "; Diff Right height: " + currentRightDiffFootHeight);

                // Inverse kinematics
                // Update IKPosition
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

                float scaleHeightLeftFoot = currentLeftDiffFootHeight <= 0 ? 0 : risePerRealHeightUnit;
                float scaleHeightRightFoot = currentRightDiffFootHeight <= 0 ? 0 : risePerRealHeightUnit;

                float deltaLeftHeight = scaleHeightLeftFoot * currentLeftDiffFootHeight;
                float deltaLeftDistance = 0;

                float deltaRightHeight = scaleHeightRightFoot * currentRightDiffFootHeight;
                float deltaRightDistance = 0;

                if (!isLeftAbove && !isRightAbove)
                {
                    deltaLeftDistance = curveLiftCoefficient * (float)Math.Pow(deltaLeftHeight, curveLiftFoot);
                    deltaRightDistance = curveLiftCoefficient * (float)Math.Pow(deltaRightHeight, curveLiftFoot);
                }
                else
                {
                    // Push down the leg to lift the body
                    if (isLeftAbove)
                    {
                        deltaLeftHeight = bodyLiftingSpeed * deltaLeftHeight + bodyLiftingConstant;
                        deltaLeftHeight = deltaLeftHeight < 0 ? 0 : deltaLeftHeight;
                        deltaLeftDistance = stepWidth / stepRise * deltaLeftHeight;

                        if (deltaLeftHeight > stepRise / 2.0f)
                        {
                            deltaRightDistance = moveBackwardLegSpeed * (deltaLeftDistance - stepWidth);
                            deltaRightHeight = moveUpwardLegSpeed * (stepRise - deltaLeftHeight);
                        }
                        else
                        {
                            deltaRightDistance = -moveBackwardLegSpeed * deltaLeftDistance;
                            deltaRightHeight = moveUpwardLegSpeed * deltaLeftHeight;
                        }
                    }
                    else
                    {
                        deltaRightHeight = bodyLiftingSpeed * deltaRightHeight + bodyLiftingConstant;
                        deltaRightHeight = deltaRightHeight < 0 ? 0 : deltaRightHeight;
                        deltaRightDistance = stepWidth / stepRise * deltaRightHeight;

                        if (deltaRightHeight > stepRise / 2.0f)
                        {
                            deltaLeftDistance = moveBackwardLegSpeed * (deltaRightDistance - stepWidth);
                            deltaLeftHeight = moveUpwardLegSpeed * (stepRise - deltaRightHeight);
                        }
                        else
                        {
                            deltaLeftDistance = -moveBackwardLegSpeed * deltaRightDistance;
                            deltaLeftHeight = moveUpwardLegSpeed * deltaRightHeight;
                        }
                    }
                }

                //Debug.Log("Left displacement: Rise = " + deltaLeftHeight + ", Tread = " + deltaLeftDistance);
                //Debug.Log("Right displacement: Rise = " + deltaRightHeight + ", Tread = " + deltaRightDistance);

                Vector3 leftFootIKWorldPosition = transform.TransformPoint(initialLeftFootIKLocalPosition);
                Vector3 rightFootIKWorldPosition = transform.TransformPoint(initialRightFootIKLocalPosition);
                leftFootIKWorldPosition = leftFootIKWorldPosition +
                                                 transform.parent.forward * deltaLeftDistance +
                                                 transform.parent.up * deltaLeftHeight;

                rightFootIKWorldPosition = rightFootIKWorldPosition +
                                                  transform.parent.forward * deltaRightDistance +
                                                  transform.parent.up * deltaRightHeight;

                animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootIKWorldPosition);
                animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootIKWorldPosition);

                currentLeftIKPosition = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
                currentRightIKPosition = animator.GetIKPosition(AvatarIKGoal.RightFoot);
                //Debug.Log("Height: " + (currentLeftIKPosition.y - leftFootIKWorldPosition.y));

                // Update IKRotation
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

                Quaternion currentLeftFootRotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
                Quaternion currentRightFootRotation = animator.GetIKRotation(AvatarIKGoal.RightFoot);

                animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.AngleAxis(dataManager.getFootAngle(DataManager.LEFT_LEG), transform.right) * currentLeftFootRotation);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.AngleAxis(dataManager.getFootAngle(DataManager.RIGHT_LEG), transform.right) * currentRightFootRotation);

                //Debug.Log("current left IK: " + currentLeftIKPosition + "; origin ik: " + leftFootIKWorldPosition);
            }
        }
    }

    Vector3 computeDirection(Vector3 origin, Vector3 destination)
    {
        return destination - origin;
    }
}