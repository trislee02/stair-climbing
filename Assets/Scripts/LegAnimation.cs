using System;
using System.Collections.Specialized;
using UnityEngine;

public class LegAnimation : MonoBehaviour
{

    public float speed;
    public float rotationSpeed;
    public float jumpSpeed;
    public float jumpButtonGracePeriod;

    [SerializeField]
    private float scaleHeightFootPosition;

    private Animator animator;
    private DataManager dataManager;

    private Vector3 previousLeftFootIKPosition;
    private Vector3 previousRightFootIKPosition;

    private float previousLeftFootRealHeight;
    private float previousRightFootRealHeight;

    bool isInitFootHeight = true;

    private float currentLeftFootHeight;
    private float currentRightFootHeight;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        dataManager = GetComponent<DataManager>();
    }

    // Update is called once per frame
    void Update()
    {
        currentLeftFootHeight = scaleHeightFootPosition * (float)Math.Sin((double)(dataManager.accelerator.roll1) * (Math.PI) / 180.0) * 30;
        currentRightFootHeight = scaleHeightFootPosition * (float)Math.Sin((double)(dataManager.accelerator.roll2) * (Math.PI) / 180.0) * 30;

        // ******************** Simple method (Work) ********************
        //if (movementDirection != Vector3.zero)
        //{
        //    animator.SetBool("isMoving", true);
        //    Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        //}
        //else
        //{
        //    animator.SetBool("isMoving", false);
        //}
        // ******************************************************

        //ySpeed += Physics.gravity.y * Time.deltaTime;

        //if (characterController.isGrounded)
        //{
        //    lastGroundedTime = Time.time;
        //}

        //if (Input.GetButtonDown("Jump"))
        //{
        //    jumpButtonPressedTime = Time.time;
        //}

        //if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        //{
        //    characterController.stepOffset = originalStepOffset;
        //    ySpeed = -0.5f;

        //    if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
        //    {
        //        ySpeed = jumpSpeed;
        //        jumpButtonPressedTime = null;
        //        lastGroundedTime = null;
        //    }
        //}
        //else
        //{
        //    characterController.stepOffset = 0;
        //}

        //Vector3 velocity = movementDirection * magnitude;
        //velocity.y = ySpeed;

        //characterController.Move(velocity * Time.deltaTime);

        //if (movementDirection != Vector3.zero)
        //{
        //    animator.SetBool("isMoving", true);
        //    Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        //}
        //else
        //{
        //    animator.SetBool("isMoving", false);
        //}
    }


    float fps = 0;
    long startTick;
    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            if (isInitFootHeight)
            {
                previousLeftFootRealHeight = 0;
                previousRightFootRealHeight = 0;

                previousLeftFootIKPosition = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
                previousRightFootIKPosition = animator.GetIKPosition(AvatarIKGoal.RightFoot);
                isInitFootHeight = false;
            }
            else
            { 
                //Debug.Log("Current foot height: left: " + currentLeftFootHeight + " right: " + currentRightFootHeight);

                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

                animator.SetIKPosition(AvatarIKGoal.LeftFoot, new Vector3(previousLeftFootIKPosition.x,
                                                                          previousLeftFootIKPosition.y + currentLeftFootHeight,
                                                                          previousLeftFootIKPosition.z));

                animator.SetIKPosition(AvatarIKGoal.RightFoot, new Vector3(previousRightFootIKPosition.x,
                                                                           previousRightFootIKPosition.y + currentRightFootHeight,
                                                                           previousRightFootIKPosition.z));

                if (fps < 0.00006)
                {
                    startTick = DateTime.Now.Ticks;
                }
                //
                fps += 1.0f;
                TimeSpan elapse = new TimeSpan(DateTime.Now.Ticks - startTick);
                if (elapse.TotalSeconds > 10.0)
                {

                    fps = fps / (float)elapse.TotalSeconds;
                    Debug.Log("IK FPS: " + fps);
                    fps = 0;
                    startTick = DateTime.Now.Ticks;
                }

                //previousLeftFootIKPosition = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
                //previousRightFootIKPosition = animator.GetIKPosition(AvatarIKGoal.RightFoot);
            }
        }        
    }
}