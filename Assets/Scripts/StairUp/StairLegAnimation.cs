using System;
using System.Collections.Specialized;
using UnityEngine;

public class StairLegAnimation : MonoBehaviour
{
    [SerializeField]
    private float maxFootHeight;

    [SerializeField]
    private float minFootHeight;

    [SerializeField]
    private float stepWidth;

    [SerializeField]
    private float stepRise;

    [SerializeField]
    private float pedalLength = 0.2f; // Distance from the origin of pedal to the sensor

    [SerializeField]
    private float curvePushFoot = 1.0f / 7.0f; // Must be a positive number smaller than 1

    [SerializeField]
    private float curveLiftFoot = 7.0f; // Must be a number larger than 1 

    [SerializeField]
    private ParticleSystem stepRipple;

    private float maxDiffFootHeight;
    private float risePerRealHeightUnit;
    private float widthPerRealHeightUnit;
    private float curveLiftCoefficient;
    private float curvePushCoefficient;

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

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        dataManager = GetComponent<DataManager>();

        maxDiffFootHeight = maxFootHeight - minFootHeight;
        risePerRealHeightUnit = stepRise / maxDiffFootHeight;
        widthPerRealHeightUnit = stepWidth / maxDiffFootHeight;
        curveLiftCoefficient = stepWidth / (float) Math.Pow(stepRise, curveLiftFoot);
        curvePushCoefficient = stepWidth / (float) Math.Pow(stepRise, curvePushFoot);
    }

    // Update is called once per frame
    void Update()
    {
        float scaleHeightLeftFoot = currentLeftDiffFootHeight <= 0 ? 0 : risePerRealHeightUnit;
        float scaleHeightRightFoot = currentRightDiffFootHeight <= 0 ? 0 : risePerRealHeightUnit;

        float deltaLeftHeight = scaleHeightLeftFoot * currentLeftDiffFootHeight;
        float deltaLeftDistance = curvePushCoefficient * (float)Math.Pow(deltaLeftHeight, curvePushFoot);

        float deltaRightHeight = scaleHeightRightFoot * currentRightDiffFootHeight;
        float deltaRightDistance = curvePushCoefficient * (float)Math.Pow(deltaRightHeight, curvePushFoot);

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
                currentAvatarPosition = transform.parent.parent.position;
            }
        }

        Vector3 destinationAvatarPosition = currentAvatarPosition
                                        + transform.parent.parent.transform.forward * stepWidth
                                        + transform.parent.parent.transform.up * stepRise;
        // Move the character when a leg is pushing down
        if (isLeftAbove || isRightAbove)
        {
            Vector3 newAvatarPosition = new Vector3();
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

        if (isLeftAbove && currentLeftDiffFootHeight <= 0)
        {
            isLeftAbove = false;
        }
        if (isRightAbove && currentRightDiffFootHeight <= 0)
        {
            isRightAbove = false;
        }
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
                currentLeftDiffFootHeight = dataManager.getFootHeight(DataManager.LEFT_LEG);
                currentRightDiffFootHeight = dataManager.getFootHeight(DataManager.RIGHT_LEG);

                Debug.Log("PreDiff Left height: " + currentLeftDiffFootHeight + "; PreDiff Right height: " + currentRightDiffFootHeight);

                // Clip foot height
                currentLeftDiffFootHeight = currentLeftDiffFootHeight < maxFootHeight ? currentLeftDiffFootHeight : maxFootHeight;
                currentRightDiffFootHeight = currentRightDiffFootHeight < maxFootHeight ? currentRightDiffFootHeight : maxFootHeight;

                currentLeftDiffFootHeight = currentLeftDiffFootHeight < minFootHeight ? minFootHeight : currentLeftDiffFootHeight;
                currentRightDiffFootHeight = currentRightDiffFootHeight < minFootHeight ? minFootHeight : currentRightDiffFootHeight;   
                
                currentLeftDiffFootHeight -= minFootHeight;
                currentRightDiffFootHeight -= minFootHeight;

                Debug.Log("Diff Left height: " + currentLeftDiffFootHeight + "; Diff Right height: " + currentRightDiffFootHeight);

                // Inverse kinematics
                // Update IKPosition
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

                float scaleDistanceLeftFoot = currentLeftDiffFootHeight <= 0 ? 0 : widthPerRealHeightUnit;
                float scaleDistanceRightFoot = currentRightDiffFootHeight <= 0 ? 0 : widthPerRealHeightUnit;

                float scaleHeightLeftFoot = currentLeftDiffFootHeight <= 0 ? 0 : risePerRealHeightUnit;
                float scaleHeightRightFoot = currentRightDiffFootHeight <= 0 ? 0 : risePerRealHeightUnit;


                float deltaLeftHeight = scaleHeightLeftFoot * currentLeftDiffFootHeight;
                float deltaLeftDistance = 0;

                float deltaRightHeight = scaleHeightRightFoot * currentRightDiffFootHeight;
                float deltaRightDistance = 0;

                if (!isLeftAbove)
                {
                    deltaLeftDistance = curveLiftCoefficient * (float) Math.Pow(deltaLeftHeight, curveLiftFoot);
                }
                else
                {
                    deltaLeftDistance = curvePushCoefficient * (float) Math.Pow(deltaLeftHeight, curvePushFoot);
                }

                if (!isRightAbove)
                {
                    deltaRightDistance = curveLiftCoefficient * (float)Math.Pow(deltaRightHeight, curveLiftFoot);
                }
                else
                {
                    deltaRightDistance = curvePushCoefficient * (float)Math.Pow(deltaRightHeight, curvePushFoot);
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

                animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.AngleAxis(dataManager.getFootAngle(DataManager.LEFT_LEG), transform.right) * currentLeftFootRotation);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.AngleAxis(dataManager.getFootAngle(DataManager.RIGHT_LEG), transform.right) * currentRightFootRotation);

                //Debug.Log("current left IK: " + currentLeftIKPosition + "; origin ik: " + initialLeftFootIKWorldPosition);
            }
        }
    }

    Vector3 computeDirection(Vector3 origin, Vector3 destination)
    {   
        return destination - origin; 
    }
}