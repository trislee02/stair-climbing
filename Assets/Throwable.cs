using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem effect;

    private GameManager gameManager;
    private SoundManager soundManager;

    private bool hasTouchTarget = false;
    private bool isHolding = false;

    // Start is called before the first frame update
    void Start()
    {
        // find the game manager by name "GameManager"
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        Debug.Log("Collision enter");
        if (!hasTouchTarget && collision.gameObject.tag == "snowman")
        {
            rb.isKinematic = true;
            effect.transform.position = gameObject.transform.position;
            effect.Play();
            hasTouchTarget = true;
            collision.gameObject.transform.parent.parent.transform.gameObject.SetActive(false);
            gameObject.SetActive(false);
            //
            gameManager.snowmanHitCallback();
            soundManager.PlayHitSound();
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Throwable enter");
        float triggerRight = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
        float triggerLeft = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);
        if ((triggerRight > 0.5 && other.gameObject.tag == "controller_right") || 
            (triggerLeft > 0.5 && other.gameObject.tag == "controller_left"))
        {
            isHolding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exit");
        float triggerRight = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
        float triggerLeft = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);
        if (isHolding &&
            ((triggerRight < 0.5 && other.gameObject.tag == "controller_right") ||
            (triggerLeft < 0.5 && other.gameObject.tag == "controller_left")))
        {
            isHolding = false;
            soundManager.PlayThrowSound();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Collision exit");
        if (collision.gameObject.tag == "controller_right" || collision.gameObject.tag == "controller_left")
        {
            soundManager.PlayThrowSound();
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
