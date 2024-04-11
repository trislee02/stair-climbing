using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadIK : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

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
        Vector3 position = camera.transform.forward * 10;

        animator.SetLookAtWeight(1);
        animator.SetLookAtPosition(position);
    }
}
