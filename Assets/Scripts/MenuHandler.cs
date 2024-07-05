using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    TextMeshProUGUI scoreText;
    [SerializeField]
    GameObject main;
    [SerializeField]
    GameObject help;
    [SerializeField]
    GameObject leaderBoard;
    [SerializeField]
    TMP_InputField nameInputField;
    
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        // find the GameManager object by name "GameManager"
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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

    public void showProfile(bool shouldClear = false)
    {
        profile.SetActive(true);
        if (shouldClear) menuStateStack.Clear();
        menuStateStack.Push(MenuState.Profile);
    }

    public void showGame()
    {
        string name = nameInputField.text;
        if (name != null && name.Length > 0 && gameManager)
        {
            gameManager.startNewGame(name);
            profile.SetActive(false);
            menuStateStack.Clear();
        }
        else
        {
            Debug.Log("Name is empty");
            // TODO: Show error message
        }
    }

    public void showGameOver(int score, bool shouldClear = false)
    {
        gameOver.SetActive(true);
        if (scoreText) scoreText.text = score.ToString();
        if (shouldClear) menuStateStack.Clear();
        menuStateStack.Push(MenuState.GameOver);
    }

    public void showMain(bool shouldClear=false)
    {
        main.SetActive(true);
        if (shouldClear) menuStateStack.Clear();
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
                showGameOver(-999); break;
            case MenuState.Ongoing:
                showGame(); break;
            case MenuState.Help:
                showHelp(); break;
        }

    }
}
