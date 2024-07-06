using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class MenuHandler : MonoBehaviour
{
    Stack menuStack = new Stack();

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
    [SerializeField]
    GameObject menuBoard;
    [SerializeField]
    GameObject laserPointer;
    
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

    public void quitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }

    public void showLeaderboard(GameObject currentObject)
    {
        changeMenu(currentObject, leaderBoard);
    }

    public void showGame(GameObject currentObject)
    {
        //string name = nameInputField.text;
        string name = "Player";
        if (name != null && name.Length > 0 && gameManager)
        {
            gameManager.startNewGame(name);
            hideMenu();
            profile.SetActive(false);
            menuStack.Clear();
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
        if (shouldClear) menuStack.Clear();
        showMenu();
    }

    public void showMain(GameObject currentObject)
    {
        changeMenu(currentObject, main);
    }

    public void showHelp(GameObject currentObject)
    {
        changeMenu(currentObject, help);
    }

    public void back(GameObject currentMenu)
    {
        //TODO: Pop from stack
        GameObject previousMenu = (GameObject) menuStack.Pop();
        currentMenu.SetActive(false);
        previousMenu.SetActive(true);
    }

    public void showProfile(GameObject currentObject)
    {
        changeMenu(currentObject, profile);
        Debug.Log("Showing profile");
    }

    public void showMenu()
    {
        menuBoard.SetActive(true);
        laserPointer.GetComponent<LineRenderer>().gameObject.SetActive(true);
    }

    public void hideMenu()
    {
        menuBoard.SetActive(false);
        laserPointer.GetComponent<LineRenderer>().gameObject.SetActive(false);
    }

    private void changeMenu(GameObject currentMenu, GameObject nextMenu)
    {
        nextMenu.SetActive(true);
        if (currentMenu != null)
        {
            menuStack.Push(currentMenu);
            currentMenu.SetActive(false);
        }
        else
        {
            menuStack.Clear();
        }
    }

    public void showKeyboard(GameObject keyboard)
    {
        keyboard.SetActive(true);
    }
    
    
}
