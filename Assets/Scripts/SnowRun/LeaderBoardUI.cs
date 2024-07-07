using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoardUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI names;
    [SerializeField]
    private TextMeshProUGUI scores;
    [SerializeField]
    private int topN = 5;

    private GameManager gameManager;

    public int getTopN()
    {
        return topN;
    }

    public void updateScore(List<ScoreRecord> top)
    {
        string namesText = "";
        string scoresText = "";
        for (int i = 0; i < top.Count; i++)
        {
            namesText += (i+1).ToString() + ". " + top[i].name + "\n";
            scoresText += top[i].score + "\n";
        }
        if (names) names.text = namesText;
        if (scores) scores.text = scoresText;
    }

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
}
