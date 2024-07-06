using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectable : MonoBehaviour
{
    private GameManager gameManager;

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

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        float triggerRight = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
        Debug.Log("TriggerRight: " + triggerRight);
        if (other.CompareTag("controller") && triggerRight > 0.5)
        {
            Debug.Log("Collected the object");
            gameManager.itemsPickingUpCallback();
            this.gameObject.SetActive(false);
        }
    }
}
