using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIKVRMap : MonoBehaviour
{
    [SerializeField]
    private Transform leftHand;

    [SerializeField] 
    private Transform rightHand;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        

        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);

        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation * Quaternion.Euler(0, 0, 90));
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation * Quaternion.Euler(0, 0, -90));
    }
}
