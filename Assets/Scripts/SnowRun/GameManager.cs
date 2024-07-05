using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    NotInitialized,
    GameIniting,
    GameInited,
    LevelLoading,
    Preparing,
    Prepared,
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
    private GameObject woodlogsHolder;
    [SerializeField]
    private List<GameObject> woodLogSamples;
    [SerializeField]
    private GameObject snowmenHolder;
    [SerializeField]
    private List<GameObject> snowmanSamples;
    [SerializeField]
    private GameObject pickingUpItemsHolder;
    [SerializeField]
    private List<GameObject> pickingUpItemSamples;
    [SerializeField]
    private GameObject obstaclesHolder;
    [SerializeField]
    private List<GameObject> obstacleSamples;
    [SerializeField]
    private GameObject playerObject;
    [SerializeField]
    private NewStairLegAnimation stairLegAnimation;
    [SerializeField]
    private MenuHandler menuHandler;
    [SerializeField]
    private GameObject readyTimerObject;

    private static readonly int SNOWMAN_HIT_SCORE = 10;
    private static readonly int GOOD_FRUIT_SCORE = 2;

    // define level schemes
    private List<LevelScheme> levelSchemes = new List<LevelScheme>() {
        new LevelScheme() { couldHasObstacles = true, couldHasPickingUpItems = true, couldHasSnowman = true, timeLimitAsSeconds = 60 },
        new LevelScheme() { couldHasObstacles = false, couldHasPickingUpItems = true, couldHasSnowman = true, timeLimitAsSeconds = 240 },
        new LevelScheme() { couldHasObstacles = true, couldHasPickingUpItems = true, couldHasSnowman = true, timeLimitAsSeconds = 240 },
        new LevelScheme() { couldHasObstacles = true, couldHasPickingUpItems = true, couldHasSnowman = true, timeLimitAsSeconds = 180 },
        new LevelScheme() { couldHasObstacles = true, couldHasPickingUpItems = true, couldHasSnowman = true, timeLimitAsSeconds = 120 },
        new LevelScheme() { couldHasObstacles = true, couldHasPickingUpItems = true, couldHasSnowman = true, timeLimitAsSeconds = 60 },
    };

    private ScoreManager scoreManager;
    private Timer countDownTimer;
    private Timer readyTimer;
    //
    private UnityEngine.Vector3 startingPlayerPosition;
    private UnityEngine.Quaternion startingPlayerRotation;
    //
    private string playerName { get; set; } = "Player";
    private GameState gameState = GameState.NotInitialized;
    private int currentLevelIndex = 0;
    //
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

    public bool isPlaying()
    {
        return this.gameState == GameState.Playing;
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

    public void timeoverCallback(Timer timer)
    {
        if (timer == this.countDownTimer)
        {
            if (this.gameState == GameState.Playing)
            {
                this.gameState = GameState.Losing;
            }
        }
        else if (timer == this.readyTimer)
        {
            if (this.gameState == GameState.Prepared)
            {
                this.gameState = GameState.Starting;
            }
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
            mask.Add(true);
        }
        for (int i = 0; i < truelyAmount; i++)
        {
            mask[randomTruelyIndices[i]] = true;
        }

        return mask;
    }

    void prepareScene(LevelScheme scheme)
    {
        //////////////////////////////////// clear scene //////////////////////////////////
        // destroy all wood logs
        foreach (var woodLog in this.woodLogs)
        {
            Destroy(woodLog);
        }
        this.woodLogs.Clear();
        // destroy all snowmen
        foreach (var snowman in this.snowmen)
        {
            Destroy(snowman);
        }
        this.snowmen.Clear();
        // destroy all picking up items
        foreach (var pickingUpItem in this.pickingUpItems)
        {
            Destroy(pickingUpItem);
        }
        this.pickingUpItems.Clear();
        // destroy all obstacles
        foreach (var obstacle in this.obstacles)
        {
            Destroy(obstacle);
        }
        this.obstacles.Clear();

        ////////////////////////////////// create new scene by scheme //////////////////////////////////
        int woodLogsAmount = this.woodLogSamples.Count / 2;
        List<bool> woodLogsMask = makeRandomMask(this.woodLogSamples.Count, woodLogsAmount);
        for (int i = 0; i < this.woodLogSamples.Count; i++)
        {
            if (scheme.couldHasSnowman && woodLogsMask[i])
            {
                // clone sample
                GameObject woodLog = Instantiate(this.woodLogSamples[i]);
                woodLog.SetActive(true);
                woodLog.transform.parent = this.woodlogsHolder.transform;
                this.woodLogs.Add(woodLog);
            }
        }
        //
        int snowmenAmount = this.snowmanSamples.Count / 2;
        List<bool> snowmenMask = makeRandomMask(this.snowmanSamples.Count, snowmenAmount);
        for (int i = 0; i < this.snowmanSamples.Count; i++)
        {
            if (scheme.couldHasSnowman && snowmenMask[i])
            {
                // clone sample and add to scene
                GameObject snowman = Instantiate(this.snowmanSamples[i]);
                snowman.SetActive(true);
                snowman.transform.parent = this.snowmenHolder.transform;
                this.snowmen.Add(snowman);
            }
        }
        //
        int pickingUpItemsAmount = this.pickingUpItemSamples.Count / 2;
        List<bool> pickingUpItemsMask = makeRandomMask(this.pickingUpItemSamples.Count, pickingUpItemsAmount);
        for (int i = 0; i < this.pickingUpItemSamples.Count; i++)
        {
            if (scheme.couldHasPickingUpItems && pickingUpItemsMask[i])
            {
                // clone sample and add to scene
                GameObject item = Instantiate(this.pickingUpItemSamples[i]);
                item.SetActive(true);
                item.transform.parent = this.pickingUpItemsHolder.transform;
                this.pickingUpItems.Add(item);
            }
        }
        //
        int obstaclesAmount = this.obstacleSamples.Count / 2;
        List<bool> obstaclesMask = makeRandomMask(this.obstacleSamples.Count, obstaclesAmount);
        for (int i = 0; i < this.obstacleSamples.Count; i++)
        {
            if (scheme.couldHasObstacles && obstaclesMask[i])
            {
                // clone sample and add to scene
                GameObject obstacle = Instantiate(this.obstacleSamples[i]);
                obstacle.SetActive(true);
                obstacle.transform.parent = this.obstaclesHolder.transform;
                this.obstacles.Add(obstacle);
            }
        }
    }

    void inactiveAllSamples()
    {
        foreach (var woodLogSample in this.woodLogSamples)
        {
            woodLogSample.SetActive(false);
        }
        foreach (var snowmanSample in this.snowmanSamples)
        {
            snowmanSample.SetActive(false);
        }
        foreach (var pickingUpItemSample in this.pickingUpItemSamples)
        {
            pickingUpItemSample.SetActive(false);
        }
        foreach (var obstacleSample in this.obstacleSamples)
        {
            obstacleSample.SetActive(false);
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
        // find timer
        if (this.timerObject != null)
        {
            this.countDownTimer = this.timerObject.GetComponent<Timer>();
        }
        // copy starting transform of playerObject to playerStartingTransform
        if (this.playerObject != null)
        {
            this.startingPlayerPosition = this.playerObject.transform.position;
            this.startingPlayerRotation = this.playerObject.transform.rotation;
        }
        // find ready timer
        if (this.readyTimerObject != null)
        {
            this.readyTimer = this.readyTimerObject.GetComponent<Timer>();
        }

        inactiveAllSamples();// inactive all samples

        this.gameState = GameState.NotInitialized;

        // test
        this.startNewGame("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("GameManager.Update: " + this.gameState);
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
                    if (readyTimer) this.readyTimer.init(3);
                    // prepare scene
                    this.prepareScene(levelScheme);
                    // reset player position (ovr)
                    if (this.playerObject != null)
                    {
                        this.playerObject.transform.position = this.startingPlayerPosition;
                        this.playerObject.transform.rotation = this.startingPlayerRotation;
                    }
                }
                // transition
                this.gameState = GameState.Preparing;
                break;

            case GameState.Preparing:
                // actions
                {
                    if (readyTimer) this.readyTimer.startCountDown();
                }
                // transition
                this.gameState = GameState.Prepared;
                break;

            case GameState.Prepared:
                // actions
                {
                    //
                }
                // transition
                if (!readyTimer) this.gameState = GameState.Starting;
                break;

            // start level
            case GameState.Starting:
                // actions
                {
                    this.countDownTimer.startCountDown();
                    if (this.stairLegAnimation) this.stairLegAnimation.setCouldMove(true);
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
                    if (this.stairLegAnimation) this.stairLegAnimation.setCouldMove(false);
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
                    if (this.stairLegAnimation) this.stairLegAnimation.setCouldMove(true);
                }
                // transition
                this.gameState = GameState.Playing;
                break;

            case GameState.Winning:
                // actions
                {
                    if (this.stairLegAnimation) this.stairLegAnimation.setCouldMove(false);
                }
                // transition
                this.gameState = GameState.LevelUpping;
                break;

            case GameState.Losing:
                // actions
                {
                    if (this.stairLegAnimation) this.stairLegAnimation.setCouldMove(false);
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
                    int score = this.scoreManager.saveCurrentPlayingScoreRecord();
                    this.menuHandler.showGameOver(score, true);
                }
                // transition
                this.gameState = GameState.Ended;
                break;

            case GameState.Ended:
                // actions
                {
                    
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