using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{
    private GameManager gameManager;

    private void OnTriggerEnter(Collider collision)
    {
        // check if the object that collided with the goal is the player
        if (collision.gameObject.tag == "obstacle")
        {
            Debug.Log("Obstacle touched by player");
            // call the Win() method in the GameManager script
            if (gameManager)
                gameManager.obstacleTouchingCallback();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // find the GameManager object in the scene by name "GameManager"
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
