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

    [SerializeField]
    private float upSpeed;

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

                isInitFootHeight = false;
            }
            else
            {
                previousLeftFootIKPosition = transform.TransformPoint(initialLeftFootIKPosition);
                previousRightFootIKPosition = transform.TransformPoint(initialRightFootIKPosition);

                currentRightFootHeight = (float)Math.Sin((double)(dataManager.accelerator.roll1) * (Math.PI) / 180.0) * 30;
                currentLeftFootHeight = (float)Math.Sin((double)(dataManager.accelerator.roll2) * (Math.PI) / 180.0) * 30;

                //Debug.Log("Current foot height: left: " + currentLeftFootHeight + " right: " + currentRightFootHeight);

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
                    movementDirection += transform.up * distance * upSpeed;
                }

                if ((deltaLeftFoot < 0 && currentLeftFootHeight > 0) || (deltaRightFoot < 0 && currentRightFootHeight > 0))
                {
                    Debug.Log("Track move: MOVE");
                    transform.parent.transform.Translate(movementDirection);
                }
                else
                {
                    Debug.Log("Track move: NO MOVE");
                }

                // Inverse kinematics
                // IKPosition
                deltaLeftFoot = currentLeftFootHeight; // deltaLeftFoot > 0 ? currentLeftFootHeight : 0;
                deltaRightFoot = currentRightFootHeight; // deltaRightFoot > 0 ? currentLeftFootHeight : 0;

                float scaleDistanceLeftFoot = currentLeftFootHeight < 0 ? 0 : scaleDistanceFootPosition;
                float scaleDistanceRightFoot = currentRightFootHeight < 0 ? 0 : scaleDistanceFootPosition;

                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

                animator.SetIKPosition(AvatarIKGoal.LeftFoot, new Vector3(previousLeftFootIKPosition.x,
                                                                          previousLeftFootIKPosition.y + scaleHeightFootPosition * deltaLeftFoot,
                                                                          previousLeftFootIKPosition.z + scaleDistanceLeftFoot * deltaLeftFoot));

                animator.SetIKPosition(AvatarIKGoal.RightFoot, new Vector3(previousRightFootIKPosition.x,
                                                                           previousRightFootIKPosition.y + scaleHeightFootPosition * deltaRightFoot,
                                                                           previousRightFootIKPosition.z + scaleDistanceRightFoot * deltaRightFoot));

                // IKRotation
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

                animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.AngleAxis(dataManager.accelerator.roll1, transform.right));
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.AngleAxis(dataManager.accelerator.roll2, transform.right));


                previousLeftFootHeight = currentLeftFootHeight;
                previousRightFootHeight = currentRightFootHeight;
            }
        }
    }
}