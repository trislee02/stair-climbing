using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        // find the GameManager object in the scene by name "GameManager"
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        // check if the object that collided with the goal is the player
        if (collision.gameObject.tag == "player")
        {
            Debug.Log("Goal touched by player");
            // call the Win() method in the GameManager script
            if (gameManager)
                gameManager.goalTouchingCallback();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
