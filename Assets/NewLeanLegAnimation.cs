using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class NewLeanLegAnimation : MonoBehaviour
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

    private float maxDiffFootHeight;
    private float risePerRealHeightUnit;
    private float widthPerRealHeightUnit;
    private float curveLiftCoefficient;
    private float bodyLiftingConstant;

    private bool isRightFootRotationFixed = true;
    private bool isLeftFootRotationFixed = true;
    private float footAngle;

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

    private Vector3 destinationAvatarPosition;

    private Vector3 currentAvatarPosition;

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

        footAngle = 90 - (float)Math.Atan(stepWidth / stepRise) * 180 / (float)Math.PI;

        sensorLogger = new MyLogger(sensorLoggerPath, -1);
        sensorLogger.Start(new string[] { "LeftRoll", "LeftFootHeight", "RightRoll", "RightFootHeight" });
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRightAbove && !isLeftAbove)
        {
            if (currentLeftDiffFootHeight >= maxDiffFootHeight)
            {
                isLeftAbove = true;
                if (stepRipple)
                    stepRipple.transform.position = currentLeftIKPosition - new Vector3(0, 0.1f, 0);
            }
            else if (currentRightDiffFootHeight >= maxDiffFootHeight)
            {
                isRightAbove = true;
                if (stepRipple)
                    stepRipple.transform.position = currentRightIKPosition - new Vector3(0, 0.1f, 0);
            }
            if (isLeftAbove || isRightAbove)
            {
                if (stepRipple)
                    stepRipple.Play();
                currentAvatarPosition = transform.parent.parent.position;
            }
        }

        Vector3 destinationAvatarPosition = currentAvatarPosition
                                        + transform.parent.parent.transform.forward * stepWidth
                                        + transform.parent.parent.transform.up * stepRise;
        
        // Move the character when a leg is pushing down
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
                newAvatarPosition = destinationAvatarPosition - transform.parent.parent.transform.forward * deltaLeftDistance
                                                              - transform.parent.parent.transform.up * deltaLeftHeight;
            }
            else
            {
                newAvatarPosition = destinationAvatarPosition - transform.parent.parent.transform.forward * deltaRightDistance
                                                              - transform.parent.parent.transform.up * deltaRightHeight;
            }
            transform.parent.parent.position = newAvatarPosition;
        }

        isLeftFootRotationFixed = true;
        isRightFootRotationFixed = true;

        if (currentLeftDiffFootHeight <= 0)
        {
            isLeftAbove = false;
        }
        else if (!isLeftAbove)
        {
            isLeftFootRotationFixed = false;
        }

        if (currentRightDiffFootHeight <= 0)
        {
            isRightAbove = false;
        }
        else if (!isRightAbove)
        {
            isRightFootRotationFixed = false;
        }
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

                // Add to log
                List<float> nums = new List<float> { roll1Logging, dataLeftFootHeight, roll2Logging, dataRightFootHeight };
                sensorLogger.Push(nums);

                Debug.Log("Data Left height: " + dataLeftFootHeight + "; Data Right height: " + dataRightFootHeight);

                // Clip foot height
                currentLeftDiffFootHeight = Mathf.Clamp(dataLeftFootHeight, minFootHeight, maxFootHeight);
                currentRightDiffFootHeight = Mathf.Clamp(dataRightFootHeight, minFootHeight, maxFootHeight);

                currentLeftDiffFootHeight -= minFootHeight;
                currentRightDiffFootHeight -= minFootHeight;

                //Debug.Log("Diff Left height: " + currentLeftDiffFootHeight + "; Diff Right height: " + currentRightDiffFootHeight);

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
                    //Debug.Log("Delta left distance: " + deltaLeftDistance + "; Delta right distance: " + deltaRightDistance);
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

                Vector3 initialLeftFootIKWorldPosition = transform.TransformPoint(initialLeftFootIKLocalPosition);
                Vector3 initialRightFootIKWorldPosition = transform.TransformPoint(initialRightFootIKLocalPosition);
                initialLeftFootIKWorldPosition = initialLeftFootIKWorldPosition +
                                                 transform.parent.parent.transform.forward * deltaLeftDistance +
                                                 transform.parent.parent.transform.up * deltaLeftHeight;

                initialRightFootIKWorldPosition = initialRightFootIKWorldPosition +
                                                  transform.parent.parent.transform.forward * deltaRightDistance +
                                                  transform.parent.parent.transform.up * deltaRightHeight;

                animator.SetIKPosition(AvatarIKGoal.LeftFoot, initialLeftFootIKWorldPosition);
                animator.SetIKPosition(AvatarIKGoal.RightFoot, initialRightFootIKWorldPosition);

                currentLeftIKPosition = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
                currentRightIKPosition = animator.GetIKPosition(AvatarIKGoal.RightFoot);
                //Debug.Log("Height: " + (currentLeftIKPosition.y - initialLeftFootIKWorldPosition.y));

                // Update IKRotation
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

                Quaternion currentLeftFootRotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
                Quaternion currentRightFootRotation = animator.GetIKRotation(AvatarIKGoal.RightFoot);

                animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.AngleAxis(-footAngle, transform.right) * currentLeftFootRotation);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.AngleAxis(-footAngle, transform.right) * currentRightFootRotation);

                //if (isLeftFootRotationFixed)
                //{
                //    animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.AngleAxis(-footAngle, transform.right) * currentLeftFootRotation);
                //}
                //else
                //{
                //    //animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.AngleAxis(dataManager.getFootAngle(DataManager.LEFT_LEG), transform.right) * currentLeftFootRotation);
                //}

                //if (isRightFootRotationFixed)
                //{
                //    animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.AngleAxis(-footAngle, transform.right) * currentRightFootRotation);
                //}
                //else
                //{
                //    //animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.AngleAxis(dataManager.getFootAngle(DataManager.RIGHT_LEG), transform.right) * currentRightFootRotation);
                //}


                //Debug.Log("current left IK: " + currentLeftIKPosition + "; origin ik: " + initialLeftFootIKWorldPosition);
            }
        }
    }

    Vector3 computeDirection(Vector3 origin, Vector3 destination)
    {
        return destination - origin;
    }
}