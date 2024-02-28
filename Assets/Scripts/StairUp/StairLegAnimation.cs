using System;
using System.Collections.Specialized;
using UnityEngine;

public class StairLegAnimation : MonoBehaviour
{
    [SerializeField]
    private float maxFootHeight;

    [SerializeField]
    private float stepWidth;

    [SerializeField]
    private float stepRise;

    [SerializeField]
    float pedalLength = 0.2f; // Distance from the origin of pedal to the sensor

    private float risePerRealHeightUnit;
    private float widthPerRealHeightUnit;

    private Animator animator;
    private DataManager dataManager;

    private Vector3 initialLeftFootIKLocalPosition = new Vector3(-0.085f, 0.1f, -0.004f); // Local position
    private Vector3 initialRightFootIKLocalPosition = new Vector3(0.085f, 0.1f, -0.004f); // Local position

    private bool isInitFootHeight = true;

    private float currentLeftFootHeight; // Real height
    private float currentRightFootHeight; // Real height

    private float previousLeftFootHeight; // Real height
    private float previousRightFootHeight; // Real height

    private bool isLeftAbove;
    private bool isRightAbove;

    private Vector3 destinationAvatarPosition;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        dataManager = GetComponent<DataManager>();

        risePerRealHeightUnit = stepRise / maxFootHeight;
        widthPerRealHeightUnit = stepWidth / maxFootHeight;
    }

    // Update is called once per frame
    void Update()
    {
        float scaleDistanceLeftFoot = currentLeftFootHeight < 0 ? 0 : widthPerRealHeightUnit;
        float scaleDistanceRightFoot = currentRightFootHeight < 0 ? 0 : widthPerRealHeightUnit;

        float scaleHeightLeftFoot = currentLeftFootHeight < 0 ? 0 : risePerRealHeightUnit;
        float scaleHeightRightFoot = currentRightFootHeight < 0 ? 0 : risePerRealHeightUnit;

        if (!isLeftAbove && currentLeftFootHeight >= maxFootHeight)
        {
            isLeftAbove = true;
            destinationAvatarPosition = new Vector3(transform.parent.parent.position.x,
                                                    transform.parent.parent.position.y + scaleHeightLeftFoot * maxFootHeight,
                                                    transform.parent.parent.position.z + scaleDistanceLeftFoot * maxFootHeight);
        }
        if (!isRightAbove && currentRightFootHeight >= maxFootHeight)
        {
            isRightAbove = true;
            destinationAvatarPosition = new Vector3(transform.parent.parent.position.x,
                                                    transform.parent.parent.position.y + scaleHeightLeftFoot * maxFootHeight,
                                                    transform.parent.parent.position.z + scaleDistanceLeftFoot * maxFootHeight);
        }
        
        // Move the character when a leg is pushing down
        if (isLeftAbove || isRightAbove)
        {
            if (isLeftAbove)
            {
                Vector3 currentAvatarPosition = transform.parent.parent.position;
                Vector3 newAvatarPosition = new Vector3(destinationAvatarPosition.x,
                                                        destinationAvatarPosition.y - scaleHeightLeftFoot * currentLeftFootHeight,
                                                        destinationAvatarPosition.z - scaleDistanceLeftFoot * currentLeftFootHeight);
                transform.parent.parent.position = newAvatarPosition;
            }
            if (isRightAbove)
            {
                Vector3 currentAvatarPosition = transform.parent.parent.position;
                Vector3 newAvatarPosition = new Vector3(destinationAvatarPosition.x,
                                                        destinationAvatarPosition.y - scaleHeightLeftFoot * currentRightFootHeight,
                                                        destinationAvatarPosition.z - scaleDistanceLeftFoot * currentRightFootHeight);
                transform.parent.parent.position = newAvatarPosition;
            }
            Debug.Log("Destination Position: " + destinationAvatarPosition + " current position: " + transform.parent.parent.position);
        }

        if (currentLeftFootHeight <= 0)
        {
            isLeftAbove = false;
        }
        if (currentRightFootHeight <= 0)
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
                previousLeftFootHeight = 0;
                previousRightFootHeight = 0;

                isInitFootHeight = false;
            }
            else
            {
                Vector3 initialLeftFootIKWorldPosition = transform.TransformPoint(initialLeftFootIKLocalPosition);
                Vector3 initialRightFootIKWorldPosition = transform.TransformPoint(initialRightFootIKLocalPosition);

                currentLeftFootHeight = (float)Math.Sin((double)(dataManager.accelerator.roll1) * (Math.PI) / 180.0f) * pedalLength;
                currentRightFootHeight = (float)Math.Sin((double)(dataManager.accelerator.roll2) * (Math.PI) / 180.0f) * pedalLength;

                Debug.Log("Left height: " + currentLeftFootHeight + "; Right height: " + currentRightFootHeight);

                // Clip foot height
                currentLeftFootHeight = currentLeftFootHeight < maxFootHeight ? currentLeftFootHeight : maxFootHeight;
                currentRightFootHeight = currentRightFootHeight < maxFootHeight ? currentRightFootHeight : maxFootHeight;

                currentLeftFootHeight = currentLeftFootHeight < 0 ? 0 : currentLeftFootHeight;
                currentRightFootHeight = currentRightFootHeight < 0 ? 0 : currentRightFootHeight;               

                // Inverse kinematics
                // Update IKPosition
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

                float scaleDistanceLeftFoot = currentLeftFootHeight < 0 ? 0 : widthPerRealHeightUnit;
                float scaleDistanceRightFoot = currentRightFootHeight < 0 ? 0 : widthPerRealHeightUnit;

                float scaleHeightLeftFoot = currentLeftFootHeight < 0 ? 0 : risePerRealHeightUnit;
                float scaleHeightRightFoot = currentRightFootHeight < 0 ? 0 : risePerRealHeightUnit;

                Debug.Log("Left displacement: Rise = " + scaleHeightLeftFoot * currentLeftFootHeight + ", Tread = " + scaleDistanceLeftFoot * currentLeftFootHeight);
                Debug.Log("Right displacement: Rise = " + scaleHeightRightFoot * currentRightFootHeight + ", Tread = " + scaleDistanceRightFoot * currentRightFootHeight);

                animator.SetIKPosition(AvatarIKGoal.LeftFoot, new Vector3(initialLeftFootIKWorldPosition.x,
                                                                          initialLeftFootIKWorldPosition.y + scaleHeightLeftFoot * currentLeftFootHeight,
                                                                          initialLeftFootIKWorldPosition.z + scaleDistanceLeftFoot * currentLeftFootHeight));

                animator.SetIKPosition(AvatarIKGoal.RightFoot, new Vector3(initialRightFootIKWorldPosition.x,
                                                                           initialRightFootIKWorldPosition.y + scaleHeightRightFoot * currentRightFootHeight,
                                                                           initialRightFootIKWorldPosition.z + scaleDistanceRightFoot * currentRightFootHeight));

                Vector3 currentLeftIK = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
                Vector3 currentRightIK = animator.GetIKPosition(AvatarIKGoal.RightFoot);
                //Debug.Log("Height: " + (currentLeftIK.y - initialLeftFootIKWorldPosition.y));

                // Update IKRotation
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

                animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.AngleAxis(dataManager.accelerator.roll1, transform.right));
                animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.AngleAxis(dataManager.accelerator.roll2, transform.right));

                // Set previous state as current state
                previousLeftFootHeight = currentLeftFootHeight;
                previousRightFootHeight = currentRightFootHeight;
                Debug.Log("current left IK: " + currentLeftIK + "; origin ik: " + initialLeftFootIKWorldPosition);
            }
        }
    }

    Vector3 computeDirection(Vector3 origin, Vector3 destination)
    {   
        return destination - origin; 
    }
}