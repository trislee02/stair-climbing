using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuHandler : MonoBehaviour
{
    Stack menuStateStack = new Stack();

    enum MenuState
    {
        Main,
        LeaderBoard,
        Profile,
        GameOver,
        Ongoing,
        Help
    }

    [SerializeField]
    GameObject profile;
    [SerializeField]
    GameObject gameOver;
    [SerializeField]
    GameObject main;
    [SerializeField]
    GameObject help;
    [SerializeField]
    GameObject leaderBoard;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadLevel(string levelName)
    {
        //SceneManager.LoadScene(levelName);
        Debug.Log("Loading level: " + levelName);
    }

    public void quitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }

    public void showLeaderboard()
    {
        leaderBoard.SetActive(true);
        menuStateStack.Push(MenuState.LeaderBoard);
    }

    public void showProfile()
    {
        profile.SetActive(true);
        menuStateStack.Push(MenuState.Profile);
    }

    public void showGame()
    {
        
    }

    public void showGameOver()
    {
        gameOver.SetActive(true);
        menuStateStack.Push(MenuState.GameOver);
    }

    public void showMain()
    {
        main.SetActive(true);
        menuStateStack.Push(MenuState.Main);
    }

    public void showHelp()
    {
        help.SetActive(true);
        menuStateStack.Push(MenuState.Help);
    }


    public void back(GameObject currentMenu)
    {
        //TODO: Pop from stack
        var previousMenu = (MenuState) menuStateStack.Pop();
        currentMenu.SetActive(false);
        switch (previousMenu)
        {
            case MenuState.Main:
                showMain(); break;
            case MenuState.LeaderBoard:
                showLeaderboard(); break;
            case MenuState.Profile: 
                showProfile(); break;
            case MenuState.GameOver:
                showGameOver(); break;
            case MenuState.Ongoing:
                showGame(); break;
            case MenuState.Help:
                showHelp(); break;
        }

    }
}
