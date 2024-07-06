using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem effect;

    private GameManager gameManager;

    private bool hasTouchTarget = false;

    // Start is called before the first frame update
    void Start()
    {
        // find the game manager by name "GameManager"
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
