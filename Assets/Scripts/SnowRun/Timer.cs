using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUI
{
    public string timeText { get; set; } = "";
    public bool shouldShow { get; set; } = false;
}

public class Timer : MonoBehaviour
{
    [SerializeField]
    private int timeLimitAsSeconds { get; set; } = 10;

    private GameManager gameManager;
    private TimerUI ui;

    private bool isCountingDown = false;
    private int remainingTime = 0;
    private long lastChecknTime = 0;

    public void startCountDown()
    {
        this.remainingTime = timeLimitAsSeconds;
        this.isCountingDown = true;
        this.lastChecknTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        // call second counting down handler
        secondCountingDownHandler();
    }

    public int getRemainingTime()
    {
        return this.remainingTime;
    }

    public int getTime()
    {
        return this.timeLimitAsSeconds - this.remainingTime;
    }

    public void init(int timeLimitAsSeconds, TimerUI _ui)
    {
        this.timeLimitAsSeconds = timeLimitAsSeconds;
        this.remainingTime = timeLimitAsSeconds;
        this.isCountingDown = false;
        this.lastChecknTime = 0;
        this.ui = _ui;
    }

    public void stopCountDown()
    {
        this.isCountingDown = false;
    }

    public void resumeCountDown()
    {
        this.isCountingDown = true;
    }

    void timerOverHandler()
    {
        // handle timer over
        if (gameManager != null)
        {
            gameManager.timeoverCallback(this);
        }
    }

    void secondCountingDownHandler()
    {
        // handle second counting down
        Debug.Log("Second counting down: " + this.remainingTime);
        if (ui != null)
        {
            ui.timeText = this.remainingTime.ToString();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        remainingTime = timeLimitAsSeconds;

        // find game manager by object name "GameManager"
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        long ms = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

        if (!this.isCountingDown)
        {
            this.lastChecknTime = ms;
            return;
        }

        if (ms - this.lastChecknTime > 1000)
        {
            // count down
            this.remainingTime -= 1;
            this.lastChecknTime = ms;
            secondCountingDownHandler();
        }

        if (this.remainingTime <= 0)
        {
            this.isCountingDown = false;
            timerOverHandler();
        }
    }
}
