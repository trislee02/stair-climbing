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
    private float curveLiftFoot = 3.0f; // Must be a number larger than 1 

    [SerializeField]
    private ParticleSystem stepRipple;

    [SerializeField]
    private float sensorDegree;

    [SerializeField]
    private float avatarRotation = 30.0f;

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
    private Transform currentGrandParentTransform;

    private bool isLeftAbove;
    private bool isRightAbove;

    private Vector3 currentLeftIKPosition;
    private Vector3 currentRightIKPosition;

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

        currentGrandParentTransform = transform.parent.parent.transform;
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
        
        // Move the character when a leg is pushing down
        if (isLeftAbove || isRightAbove)
        {
            Vector3 destinationAvatarPosition = currentAvatarPosition
                                        + transform.parent.parent.transform.forward * stepWidth
                                        + transform.parent.parent.transform.up * stepRise;
            
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
            Debug.Log("Avatar position: " + transform.parent.parent.position + " | Destination: " + destinationAvatarPosition);
        }

        if (isLeftAbove && currentLeftDiffFootHeight <= 0)
        {
            isLeftAbove = false;
            transform.parent.parent.Rotate(transform.parent.parent.up, avatarRotation);
        }
        if (isRightAbove && currentRightDiffFootHeight <= 0)
        {
            transform.parent.parent.Rotate(transform.parent.parent.up, avatarRotation);
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
                dataManager.accelerator.roll1 = sensorDegree;
                dataManager.accelerator.roll2 = 0;

                currentLeftDiffFootHeight = (float)Math.Sin((double)(dataManager.accelerator.roll1) * (Math.PI) / 180.0f) * pedalLength;
                currentRightDiffFootHeight = (float)Math.Sin((double)(dataManager.accelerator.roll2) * (Math.PI) / 180.0f) * pedalLength;

                Debug.Log("PreDiff Left height: " + currentLeftDiffFootHeight + "; PreDiff Right height: " + currentRightDiffFootHeight);

                // Clip foot height
                currentLeftDiffFootHeight = currentLeftDiffFootHeight < maxFootHeight ? currentLeftDiffFootHeight : maxFootHeight;
                currentRightDiffFootHeight = currentRightDiffFootHeight < maxFootHeight ? currentRightDiffFootHeight : maxFootHeight;

                currentLeftDiffFootHeight = currentLeftDiffFootHeight < minFootHeight ? minFootHeight : currentLeftDiffFootHeight;
                currentRightDiffFootHeight = currentRightDiffFootHeight < minFootHeight ? minFootHeight : currentRightDiffFootHeight;   
                
                // Translate to the origin
                currentLeftDiffFootHeight -= minFootHeight;
                currentRightDiffFootHeight -= minFootHeight;

                Debug.Log("Diff Left height: " + currentLeftDiffFootHeight + "; Diff Right height: " + currentRightDiffFootHeight);

                // Inverse kinematics
                // Update IKPosition
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

                float scaleHeightLeftFoot = currentLeftDiffFootHeight <= 0 ? 0 : risePerRealHeightUnit;
                float scaleHeightRightFoot = currentRightDiffFootHeight <= 0 ? 0 : risePerRealHeightUnit;

                // Map the delta height to the delta distance
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

                Debug.Log("Left displacement: Rise = " + deltaLeftHeight + ", Tread = " + deltaLeftDistance);
                Debug.Log("Right displacement: Rise = " + deltaRightHeight + ", Tread = " + deltaRightDistance);

                Vector3 leftFootIKWorldPosition = transform.TransformPoint(initialLeftFootIKLocalPosition); 
                Vector3 rightFootIKWorldPosition = transform.TransformPoint(initialRightFootIKLocalPosition);

                leftFootIKWorldPosition = leftFootIKWorldPosition + 
                                            transform.parent.parent.transform.forward * deltaLeftDistance +
                                            transform.parent.parent.transform.up * deltaLeftHeight;

                rightFootIKWorldPosition = rightFootIKWorldPosition +
                                            transform.parent.parent.transform.forward * deltaRightDistance +
                                            transform.parent.parent.transform.up * deltaRightHeight;

                animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootIKWorldPosition);
                animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootIKWorldPosition);

                currentLeftIKPosition = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
                currentRightIKPosition = animator.GetIKPosition(AvatarIKGoal.RightFoot);

                Debug.Log("Left IK position: " + currentLeftIKPosition + "; Right IK position: " + currentRightIKPosition);

                // Update IKRotation
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

                Quaternion currentLeftFootRotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
                Quaternion currentRightFootRotation = animator.GetIKRotation(AvatarIKGoal.RightFoot);

                animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.AngleAxis(dataManager.accelerator.roll1, currentGrandParentTransform.right) * currentLeftFootRotation);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.AngleAxis(dataManager.accelerator.roll2, currentGrandParentTransform.right) * currentRightFootRotation);
            }
        }
    }

    Vector3 computeDirection(Vector3 origin, Vector3 destination)
    {   
        return destination - origin; 
    }
}