using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityActivator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // "obstacle"/"throwable"/"grabable" object
        string tag = other.gameObject.tag;
        if (
            tag == "obstacle" ||
            tag == "throwable" ||
            tag == "grabable"
        ) {
            // active gravity for the object
            Rigidbody rigid = other.gameObject.GetComponent<Rigidbody>();
            if (rigid != null)
            {
                rigid.useGravity = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
