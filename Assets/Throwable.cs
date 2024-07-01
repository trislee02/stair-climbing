using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem effect;

    private bool hasTouchTarget = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasTouchTarget && collision.gameObject.tag == "snowman")
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            effect.transform.position = gameObject.transform.position;
            effect.Play();
            Debug.Log("Collision enter");
            hasTouchTarget = true;
            //TODO: Add score
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "snowman")
        {
            Debug.Log("Collision stay");
        }
    }
}
