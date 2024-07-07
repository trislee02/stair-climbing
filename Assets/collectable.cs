using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectable : MonoBehaviour
{
    private GameManager gameManager;
    private SoundManager soundManager;

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

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        float triggerRight = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
        float triggerLeft = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);
        Debug.Log("TriggerRight: " + triggerRight);
        if (other.CompareTag("controller_right") && triggerRight > 0.5)
        {
            Debug.Log("Collected the object");
            gameManager.itemsPickingUpCallback();
            this.gameObject.SetActive(false);
            soundManager.PlayCollectSound();
        }
        if (other.CompareTag("controller_left") && triggerLeft > 0.5)
        {
            Debug.Log("Collected the object");
            gameManager.itemsPickingUpCallback();
            this.gameObject.SetActive(false);
            soundManager.PlayCollectSound();
        }
    }
}
