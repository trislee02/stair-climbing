using System.Collections.Specialized;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed;
    public float rotationSpeed;
    public float jumpSpeed;
    public float jumpButtonGracePeriod;

    [SerializeField]
    private float scaleHeightFootPosition;

    private Animator animator;
    private CharacterController characterController;
    private DataManager dataManager;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;

    private Vector3 previousLeftFootIKPosition;
    private Vector3 previousRightFootIKPosition;

    private float previousLeftFootRealHeight;
    private float previousRightFootRealHeight;

    bool isInitFootHeight = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
        dataManager = GetComponent<DataManager>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;
        movementDirection.Normalize();

        // ******************** Simple method (Work) ********************
        //transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);

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

        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;
        }

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpSpeed;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            characterController.stepOffset = 0;
        }

        Vector3 velocity = movementDirection * magnitude;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        if (movementDirection != Vector3.zero)
        {
            animator.SetBool("isMoving", true);
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            if (isInitFootHeight)
            {
                previousLeftFootRealHeight = dataManager.accelerator.roll1;
                previousRightFootRealHeight = dataManager.accelerator.roll2;

                previousLeftFootIKPosition = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
                previousRightFootIKPosition = animator.GetIKPosition(AvatarIKGoal.RightFoot);
                isInitFootHeight = false;
            }
            else
            { 
                float currentLeftFootHeight = (dataManager.accelerator.roll1 + 13) * scaleHeightFootPosition;
                float currentRightFootHeight = (dataManager.accelerator.roll2 + 13) * scaleHeightFootPosition;

                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

                animator.SetIKPosition(AvatarIKGoal.LeftFoot, new Vector3(previousLeftFootIKPosition.x,
                                                                          previousLeftFootIKPosition.y + currentLeftFootHeight - previousLeftFootRealHeight,
                                                                          previousLeftFootIKPosition.z));

                animator.SetIKPosition(AvatarIKGoal.RightFoot, new Vector3(previousRightFootIKPosition.x,
                                                                           previousRightFootIKPosition.y + currentRightFootHeight - previousRightFootRealHeight,
                                                                           previousRightFootIKPosition.z));
                
                previousLeftFootIKPosition = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
                previousRightFootIKPosition = animator.GetIKPosition(AvatarIKGoal.RightFoot);
            }
        }        
    }
}