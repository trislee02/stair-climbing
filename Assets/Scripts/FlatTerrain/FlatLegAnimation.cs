using System;
using System.Collections.Specialized;
using UnityEngine;

public class FlatLegAnimation : MonoBehaviour
{
    [SerializeField]
    private float scaleHeightFootPosition;

    [SerializeField] 
    private float scaleDistanceFootPosition;

    [SerializeField]
    private CharacterController characterController;

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

                currentLeftFootHeight = dataManager.getFootHeight(DataManager.LEFT_LEG);
                currentRightFootHeight = dataManager.getFootHeight(DataManager.RIGHT_LEG);

                Debug.Log("Current foot height: left: " + currentLeftFootHeight + " right: " + currentRightFootHeight);

                Vector3 currentLeftFootIKPosition = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
                Vector3 currentRightFootIKPosition = animator.GetIKPosition(AvatarIKGoal.RightFoot);

                Debug.Log("Current LeftFootIK position: " + currentLeftFootIKPosition + ", Current RightFootIK position: " + currentRightFootIKPosition);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

                animator.SetIKPosition(AvatarIKGoal.LeftFoot, new Vector3(previousLeftFootIKPosition.x,
                                                                          previousLeftFootIKPosition.y + scaleHeightFootPosition * currentLeftFootHeight,
                                                                          previousLeftFootIKPosition.z + scaleDistanceFootPosition * currentLeftFootHeight));

                animator.SetIKPosition(AvatarIKGoal.RightFoot, new Vector3(previousRightFootIKPosition.x,
                                                                           previousRightFootIKPosition.y + scaleHeightFootPosition * currentRightFootHeight,
                                                                           previousRightFootIKPosition.z + scaleDistanceFootPosition * currentRightFootHeight));

                // IKRotation
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

                animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.AngleAxis(dataManager.getFootAngle(DataManager.LEFT_LEG), transform.right));
                animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.AngleAxis(dataManager.getFootAngle(DataManager.RIGHT_LEG), transform.right));


                float distance = Math.Abs(currentLeftFootHeight - previousLeftFootHeight);
                if (distance > 0.1f)
                {
                    Debug.Log("Movement distance: " + distance);
                    Vector3 movementDirection = transform.forward * distance * forwardSpeed;
                    Debug.Log("Movement vector: " + movementDirection);
                    transform.parent.parent.transform.Translate(movementDirection);
                }

                Debug.Log("Parent position: " + transform.parent.transform.position);

                previousLeftFootHeight = currentLeftFootHeight;
                previousRightFootHeight = currentRightFootHeight;

                // Debug 
                //if (fps < 0.00006)
                //{
                //    startTick = DateTime.Now.Ticks;
                //}
                ////
                //fps += 1.0f;
                //TimeSpan elapse = new TimeSpan(DateTime.Now.Ticks - startTick);
                //if (elapse.TotalSeconds > 10.0)
                //{

                //    fps = fps / (float)elapse.TotalSeconds;
                //    Debug.Log("IK FPS: " + fps);
                //    fps = 0;
                //    startTick = DateTime.Now.Ticks;
                //}

                //previousLeftFootIKPosition = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
                //previousRightFootIKPosition = animator.GetIKPosition(AvatarIKGoal.RightFoot);
            }
        }        
    }
}