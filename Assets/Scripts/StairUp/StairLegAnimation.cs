using System;
using System.Collections.Specialized;
using UnityEngine;

public class StairLegAnimation : MonoBehaviour
{
    [SerializeField]
    private float scaleHeightFootPosition;

    [SerializeField]
    private float scaleDistanceFootPosition;

    [SerializeField]
    private float forwardSpeed;

    private Animator animator;
    private DataManager dataManager;

    private Vector3 initialLeftFootIKPosition = new Vector3(-0.05f, -0.05f, -0.004f);
    private Vector3 initialRightFootIKPosition = new Vector3(0.05f, -0.05f, -0.004f);

    private Vector3 previousLeftFootIKPosition;
    private Vector3 previousRightFootIKPosition;

    private bool isInitFootHeight = true;

    private float ySpeed;

    private float currentLeftFootHeight;
    private float currentRightFootHeight;

    private float previousLeftFootHeight;
    private float previousRightFootHeight;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        dataManager = GetComponent<DataManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    float fps = 0;
    long startTick;
    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            if (isInitFootHeight)
            {
                previousLeftFootHeight = 0;
                previousRightFootHeight = 0;

                previousLeftFootIKPosition = transform.TransformPoint(initialLeftFootIKPosition);
                previousRightFootIKPosition = transform.TransformPoint(initialRightFootIKPosition);
                Debug.Log("transformed point: " + previousLeftFootIKPosition + ", " + previousRightFootIKPosition);
                isInitFootHeight = false;
            }
            else
            {
                previousLeftFootIKPosition = transform.TransformPoint(initialLeftFootIKPosition);
                previousRightFootIKPosition = transform.TransformPoint(initialRightFootIKPosition);

                Debug.Log("transformed point: " + previousLeftFootIKPosition + ", " + previousRightFootIKPosition);

                currentRightFootHeight = (float)Math.Sin((double)(dataManager.accelerator.roll1) * (Math.PI) / 180.0) * 30;
                currentLeftFootHeight = (float)Math.Sin((double)(dataManager.accelerator.roll2) * (Math.PI) / 180.0) * 30;

                Debug.Log("Current foot height: left: " + currentLeftFootHeight + " right: " + currentRightFootHeight);

                Vector3 currentLeftFootIKPosition = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
                Vector3 currentRightFootIKPosition = animator.GetIKPosition(AvatarIKGoal.RightFoot);

                Debug.Log("Current LeftFootIK position: " + currentLeftFootIKPosition + ", Current RightFootIK position: " + currentRightFootIKPosition);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

                // Move forward
                float deltaLeftFoot = currentLeftFootHeight - previousLeftFootHeight;
                float deltaRightFoot = currentRightFootHeight - previousRightFootHeight;

                float distance = Math.Abs(deltaLeftFoot);
                Vector3 movementDirection = Vector3.zero;
                Debug.Log("Movement distance: " + distance);
                if (distance > 0.1f)
                {
                    movementDirection += transform.forward * distance * forwardSpeed;
                }

                // Step on stair
                if (distance > 0.1f)
                {
                    movementDirection += transform.up * distance * forwardSpeed;
                }

                transform.parent.transform.Translate(movementDirection);

                // Inverse kinematics
                deltaLeftFoot = deltaLeftFoot > 0 ? currentLeftFootHeight : 0;
                deltaRightFoot = deltaRightFoot > 0 ? currentLeftFootHeight : 0;

                animator.SetIKPosition(AvatarIKGoal.LeftFoot, new Vector3(previousLeftFootIKPosition.x,
                                                                          previousLeftFootIKPosition.y + scaleHeightFootPosition * deltaLeftFoot,
                                                                          previousLeftFootIKPosition.z + scaleDistanceFootPosition * deltaLeftFoot));

                animator.SetIKPosition(AvatarIKGoal.RightFoot, new Vector3(previousRightFootIKPosition.x,
                                                                           previousRightFootIKPosition.y + scaleHeightFootPosition * deltaRightFoot,
                                                                           previousRightFootIKPosition.z + scaleDistanceFootPosition * deltaRightFoot));

                previousLeftFootHeight = currentLeftFootHeight;
                previousRightFootHeight = currentRightFootHeight;
            }
        }
    }
}