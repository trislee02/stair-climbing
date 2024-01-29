using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class IKFootPlacement : MonoBehaviour
{
    Animator anim;

    [Range(0f, 1f)]
    [SerializeField]
    private float distanceToGround;

    [SerializeField]
    private float upwardDegree;

    [SerializeField]
    private LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    { 
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

        //float maxDistance = distanceToGround + 1f;
        float maxDistance = Mathf.Infinity;
        
        // Left foot
        RaycastHit hit;
        Ray ray = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
        {
            if (hit.transform.tag.Equals("Walkable"))
            {
                Debug.Log("Hit a walkable object on left leg");
                Vector3 footPosition = hit.point;
                footPosition.y += distanceToGround;
                //anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward + transform.up * upwardDegree, hit.normal));

                Quaternion leftQuaternion = Quaternion.LookRotation(transform.forward + transform.up * upwardDegree, hit.normal);
            }
        }

        // Right foot
        ray = new Ray(anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
        {
            if (hit.transform.tag.Equals("Walkable"))
            {
                Vector3 footPosition = hit.point;
                footPosition.y += distanceToGround;
                //anim.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward + transform.up * upwardDegree, hit.normal));
            }
        }
    }

}
