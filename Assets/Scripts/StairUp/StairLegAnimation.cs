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

    [SerializeField]
    private GameObject visualLeftFootIKPosition;

    [SerializeField]
    private GameObject visualOriginLeftFootIKPosition;

    private Animator animator;
    private DataManager dataManager;

    private Vector3 initialLeftFootIKLocalPosition = new Vector3(-0.085f, -0.05f, -0.004f); // Local position
    private Vector3 initialRightFootIKLocalPosition = new Vector3(0.085f, -0.05f, -0.004f); // Local position

    private bool isInitFootHeight = true;

    private float currentLeftFootHeight;
    private float currentRightFootHeight;

    private float previousLeftFootHeight;
    private float previousRightFootHeight;

    private Vector3 currentLeftFootIKWorldPosition;
    private Vector3 currentRightFootIKWorldPosition;

    private Vector3 previousLeftFootPosition;
    private Vector3 previousRightFootPosition;
    
    private Vector3 currentLeftFootPosition;
    private Vector3 currentRightFootPosition;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        dataManager = GetComponent<DataManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Transform leftFoot = transform.FindChildRecursive("mixamorig:LeftFoot");
        Transform rightFoot = transform.FindChildRecursive("mixamorig:RightFoot");

        if (isInitFootHeight)
        {
            previousLeftFootPosition = leftFoot.position;
            previousRightFootPosition = rightFoot.position;
        }
        else
        {
            currentLeftFootPosition = leftFoot.position;
            currentRightFootPosition = rightFoot.position;

            if (previousLeftFootPosition.y > currentLeftFootPosition.y)
            {
                Vector3 direction = previousLeftFootPosition - currentLeftFootPosition;
                Debug.Log("Direction left vector: " + (previousLeftFootPosition - currentLeftFootPosition));
                transform.parent.parent.Translate(direction, Space.World);
            }
            
            if (previousRightFootPosition.y > currentRightFootPosition.y)
            {
                Vector3 direction = previousRightFootPosition - currentRightFootPosition;
                Debug.Log("Direction right vector: " + (previousRightFootPosition - currentRightFootPosition));
                transform.parent.parent.Translate(direction, Space.World);
            }

            previousLeftFootPosition = leftFoot.position; 
            previousRightFootPosition = rightFoot.position;
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

                currentLeftFootHeight = (float)Math.Sin((double)(dataManager.accelerator.roll1) * (Math.PI) / 180.0) * 30;
                currentRightFootHeight = (float)Math.Sin((double)(dataManager.accelerator.roll2) * (Math.PI) / 180.0) * 30;
                                
                // Move forward
                float deltaLeftFoot = currentLeftFootHeight - previousLeftFootHeight;
                float deltaRightFoot = currentRightFootHeight - previousRightFootHeight;

                // Only move when there is a leg going down
                if ((deltaLeftFoot < -0.01f && currentLeftFootHeight > 0f) || (deltaRightFoot < -0.01f && currentRightFootHeight > 0f))
                {
                    Debug.Log("TracK MOVE");
                    //// Simple move using offset
                    //float distance = previousLeftFootHeight > previousRightFootHeight ? Math.Abs(deltaLeftFoot) : Math.Abs(deltaRightFoot);
                    //Vector3 movementDirection = Vector3.zero;
                    //Debug.Log("Movement distance: " + distance);
                    //if (distance > 0.01f)
                    //{
                    //    movementDirection += transform.forward * distance * forwardSpeed;
                    //}

                    //// Step on stair
                    //if (distance > 0.01f)
                    //{
                    //    movementDirection += transform.up * distance * upSpeed;
                    //}
                    //transform.parent.parent.transform.Translate(movementDirection);

                    //// Advanced move using vector
                    Vector3 direction;
                                        
                    float deltaHeight;
                    float currentHeight;

                    if (deltaLeftFoot < 0 && currentLeftFootHeight > 0)
                    {
                        //direction = computeDirection(initialLeftFootIKWorldPosition, currentLeftFootIKWorldPosition);
                        direction = currentLeftFootPosition - previousLeftFootPosition;
                        deltaHeight = -deltaLeftFoot;
                        currentHeight = previousLeftFootHeight;
                    }
                    else
                    {
                        //direction = computeDirection(initialRightFootIKWorldPosition, currentRightFootIKWorldPosition);
                        direction = previousLeftFootPosition - currentLeftFootPosition;
                        deltaHeight = -deltaRightFoot;
                        currentHeight = previousRightFootHeight;
                    }
                    Debug.Log("Track MOVE vector: " + direction);
                    
                }
                else
                {
                    Debug.Log("Track NO MOVE");
                }

                // Inverse kinematics
                // Update IKPosition
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

                deltaLeftFoot = currentLeftFootHeight;
                deltaRightFoot = currentRightFootHeight;

                float scaleDistanceLeftFoot = currentLeftFootHeight < 0 ? 0 : scaleDistanceFootPosition;
                float scaleDistanceRightFoot = currentRightFootHeight < 0 ? 0 : scaleDistanceFootPosition;

                float scaleHeightLeftFoot = currentLeftFootHeight < 0 ? 0 : scaleHeightFootPosition;
                float scaleHeightRightFoot = currentRightFootHeight < 0 ? 0 : scaleHeightFootPosition;

                animator.SetIKPosition(AvatarIKGoal.LeftFoot, new Vector3(initialLeftFootIKWorldPosition.x,
                                                                          initialLeftFootIKWorldPosition.y + scaleHeightLeftFoot * deltaLeftFoot,
                                                                          initialLeftFootIKWorldPosition.z + scaleDistanceLeftFoot * deltaLeftFoot));

                animator.SetIKPosition(AvatarIKGoal.RightFoot, new Vector3(initialRightFootIKWorldPosition.x,
                                                                           initialRightFootIKWorldPosition.y + scaleHeightRightFoot * deltaRightFoot,
                                                                           initialRightFootIKWorldPosition.z + scaleDistanceRightFoot * deltaRightFoot));

                currentLeftFootIKWorldPosition = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
                currentRightFootIKWorldPosition = animator.GetIKPosition(AvatarIKGoal.RightFoot);


                //visualLeftFootIKPosition.transform.position = currentLeftFootIKWorldPosition;
                //visualOriginLeftFootIKPosition.transform.position = initialLeftFootIKWorldPosition;

                // Update IKRotation
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

                animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.AngleAxis(dataManager.accelerator.roll1, transform.right));
                animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.AngleAxis(dataManager.accelerator.roll2, transform.right));

                // Set previous state as current state
                previousLeftFootHeight = currentLeftFootHeight;
                previousRightFootHeight = currentRightFootHeight;

            }
        }
    }

    Vector3 computeDirection(Vector3 origin, Vector3 destination)
    {   
        return destination - origin; 
    }
}