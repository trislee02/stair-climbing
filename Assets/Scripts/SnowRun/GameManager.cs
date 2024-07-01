using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public enum GameState
{
    NotInitialized,
    GameIniting,
    GameInited,
    LevelLoading,
    Starting,
    Playing,
    Pausing,
    Paused,
    Resuming,
    Winning,
    Losing,
    LevelUpping,
    Ending,
    Ended,
};

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject scoreManagerObject;
    [SerializeField]
    private GameObject timerObject;
    [SerializeField]
    private GameObject woodLogsHolder;
    [SerializeField]
    private GameObject snowmenHolder;
    [SerializeField]
    private GameObject pickingUpItemsHolder;
    [SerializeField]
    private GameObject obstaclesHolder;
    [SerializeField]
    private GameObject goalObject;

    private static readonly int SNOWMAN_HIT_SCORE = 10;
    private static readonly int GOOD_FRUIT_SCORE = 2;

    // define level schemes
    private List<LevelScheme> levelSchemes = new List<LevelScheme>() {
        new LevelScheme() { couldHasObstacles = false, couldHasPickingUpItems = false, couldHasSnowman = true, timeLimitAsSeconds = 300 },
        new LevelScheme() { couldHasObstacles = false, couldHasPickingUpItems = true, couldHasSnowman = true, timeLimitAsSeconds = 240 },
        new LevelScheme() { couldHasObstacles = true, couldHasPickingUpItems = true, couldHasSnowman = true, timeLimitAsSeconds = 240 },
        new LevelScheme() { couldHasObstacles = true, couldHasPickingUpItems = true, couldHasSnowman = true, timeLimitAsSeconds = 180 },
        new LevelScheme() { couldHasObstacles = true, couldHasPickingUpItems = true, couldHasSnowman = true, timeLimitAsSeconds = 120 },
        new LevelScheme() { couldHasObstacles = true, couldHasPickingUpItems = true, couldHasSnowman = true, timeLimitAsSeconds = 60 },
    };

    private ScoreManager scoreManager;
    private Timer countDownTimer;
    //
    private string playerName { get; set; } = "Player";
    private GameState gameState = GameState.NotInitialized;
    private int currentLevelIndex = 0;
    private List<GameObject> woodLogs = new List<GameObject>();
    private List<GameObject> snowmen = new List<GameObject>();
    private List<GameObject> pickingUpItems = new List<GameObject>();
    private List<GameObject> obstacles = new List<GameObject>();

    public ScoreManager getScoreManager()
    {
        return this.scoreManager;
    }

    public Timer getTimer()
    {
        return this.countDownTimer;
    }

    public GameState getGameState()
    {
        return this.gameState;
    }

    public bool startNewGame(string playerName)
    {
        this.playerName = playerName;
        this.gameState = GameState.GameIniting;
        return true;
    }

    public int getCurrentLevel()
    {
        return this.currentLevelIndex + 1;
    }

    public void snowmanHitCallback()
    {
        // add score for current player
        if (this.scoreManager != null)
        {
            this.scoreManager.addToCurrentPlayingScoreRecord(SNOWMAN_HIT_SCORE);
        }
    }

    public void itemsPickingUpCallback()
    {
        // add score for current player
        if (this.scoreManager != null)
        {
            // random score, maybe negative score for poison fruit
            this.scoreManager.addToCurrentPlayingScoreRecord(GOOD_FRUIT_SCORE);
        }
    }

    public void obstacleTouchingCallback()
    {
        if (this.gameState == GameState.Playing)
        {
            this.gameState = GameState.Losing;
        }
    }

    public void timeoverCallback()
    {
        if (this.gameState == GameState.Playing)
        {
            this.gameState = GameState.Losing;
        }
    }

    public void goalTouchingCallback()
    {
        if (this.gameState == GameState.Playing)
        {
            this.gameState = GameState.Winning;
        }
    }

    // create a bool mask list with len elements which has truelyAmount random true elements
    List<bool> makeRandomMask(int len, int truelyAmount)
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < len; i++)
        {
            indices.Add(i);
        }
        // shuffle indices
        for (int i = 0; i < len; i++)
        {
            int j = Random.Range(0, len);
            int temp = indices[i];
            indices[i] = indices[j];
            indices[j] = temp;
        }
        // create a new list having first truelyAmount elements from indices list
        List<int> randomTruelyIndices = new List<int>();
        for (int i = 0; i < truelyAmount; i++)
        {
            randomTruelyIndices.Add(indices[i]);
        }

        List<bool> mask = new List<bool>();
        for (int i = 0; i < len; i++)
        {
            mask.Add(false);
        }
        for (int i = 0; i < truelyAmount; i++)
        {
            mask[randomTruelyIndices[i]] = true;
        }

        return mask;
    }

    void prepareScene(LevelScheme scheme)
    {
        int woodLogsAmount = this.woodLogs.Count / 2;
        List<bool> woodLogsMask = makeRandomMask(this.woodLogs.Count, woodLogsAmount);
        for (int i = 0; i < this.woodLogs.Count; i++)
        {
            this.woodLogs[i].SetActive(scheme.couldHasSnowman && woodLogsMask[i]);
        }

        int snowmenAmount = this.snowmen.Count / 2;
        List<bool> snowmenMask = makeRandomMask(this.snowmen.Count, snowmenAmount);
        for (int i = 0; i < this.snowmen.Count; i++)
        {
            this.snowmen[i].SetActive(scheme.couldHasSnowman && snowmenMask[i]);
        }

        int pickingUpItemsAmount = this.pickingUpItems.Count / 2;
        List<bool> pickingUpItemsMask = makeRandomMask(this.pickingUpItems.Count, pickingUpItemsAmount);
        for (int i = 0; i < this.pickingUpItems.Count; i++)
        {
            this.pickingUpItems[i].SetActive(scheme.couldHasPickingUpItems && pickingUpItemsMask[i]);
        }

        int pickingDownItemsAmount = this.obstacles.Count / 2;
        List<bool> obstaclesMask = makeRandomMask(this.obstacles.Count, pickingDownItemsAmount);
        for (int i = 0; i < this.obstacles.Count; i++)
        {
            this.obstacles[i].SetActive(scheme.couldHasObstacles && obstaclesMask[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // find score manager
        if (this.scoreManagerObject != null)
        {
            this.scoreManager = this.scoreManagerObject.GetComponent<ScoreManager>();
        }
        // find timer by object name "GameTimer"
        if (this.timerObject != null)
        {
            this.countDownTimer = this.timerObject.GetComponent<Timer>();
        }
        // add all children game object of woodLogsHolder to woodLogs
        if (this.woodLogsHolder != null)
        {
            foreach (Transform child in this.woodLogsHolder.transform)
            {
                this.woodLogs.Add(child.gameObject);
            }
        }
        // add all children game object of snowmenHolder to snowmen
        if (this.snowmenHolder != null)
        {
            foreach (Transform child in this.snowmenHolder.transform)
            {
                this.snowmen.Add(child.gameObject);
            }
        }
        // add all children game object of pickingUpItemsHolder to pickingUpItems
        if (this.pickingUpItemsHolder != null)
        {
            foreach (Transform child in this.pickingUpItemsHolder.transform)
            {
                this.pickingUpItems.Add(child.gameObject);
            }
        }
        // add all children game object of obstaclesHolder to obstacles
        if (this.obstaclesHolder != null)
        {
            foreach (Transform child in this.obstaclesHolder.transform)
            {
                this.obstacles.Add(child.gameObject);
            }
        }

        this.gameState = GameState.NotInitialized;

        // test
        this.startNewGame("Player");
    }

    // Update is called once per frame
    void Update()
    {
        switch (this.gameState)
        {
            case GameState.NotInitialized:
                // actions
                {
                    //
                }
                // transition
                break;

            // setup new game
            case GameState.GameIniting:
                // actions
                {
                    this.currentLevelIndex = 0;

                    // init score manager
                    this.scoreManager.init(playerName);
                }
                // transition
                this.gameState = GameState.GameInited;
                break;

            case GameState.GameInited:
                // actions
                {
                    //
                }
                // transition
                this.gameState = GameState.LevelLoading;
                break;

            // setup new level
            case GameState.LevelLoading:
                // actions
                {
                    LevelScheme levelScheme = this.levelSchemes[this.currentLevelIndex];
                    // init timer
                    this.countDownTimer.init(levelScheme.timeLimitAsSeconds);
                    // prepare scene
                    this.prepareScene(levelScheme);
                }
                // transition
                this.gameState = GameState.Starting;
                break;

            // start level
            case GameState.Starting:
                // actions
                {
                    this.countDownTimer.startCountDown();
                }
                // transition
                this.gameState = GameState.Playing;
                break;

            case GameState.Playing:
                // actions
                {
                    //
                }
                // transition
                break;

            case GameState.Pausing:
                // actions
                {
                    this.countDownTimer.stopCountDown();
                }
                // transition
                this.gameState = GameState.Paused;
                break;

            case GameState.Paused:
                // actions
                {
                    //
                }
                // transition
                break;

            case GameState.Resuming:
                // actions
                {
                    this.countDownTimer.resumeCountDown();
                }
                // transition
                this.gameState = GameState.Playing;
                break;

            case GameState.Winning:
                // actions
                {
                    //
                }
                // transition
                this.gameState = GameState.LevelUpping;
                break;

            case GameState.Losing:
                // actions
                {
                    //
                }
                // transition
                this.gameState = GameState.Ending;
                break;

            case GameState.LevelUpping:
                // actions
                {
                    // up level
                    if (this.currentLevelIndex < this.levelSchemes.Count - 1) this.currentLevelIndex++;
                }
                // transition
                this.gameState = GameState.LevelLoading;
                break;

            case GameState.Ending:
                // actions
                {
                    this.countDownTimer.stopCountDown();
                }
                // transition
                this.gameState = GameState.Ended;
                break;

            case GameState.Ended:
                // actions
                {
                    //
                }
                // transition
                break;
        }
    }
}

//Pausing,
//Paused,
//Resuming,
//Winning,
//Losing,
//LevelUpping,
//Ending,
//Ended,