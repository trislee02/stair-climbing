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

    // function that will be called when the gameobject active state is changed
    private void OnEnable()
    {
        List<ScoreRecord> top = gameManager.getScoreManager().getTopN(topN);
        string namesText = "";
        string scoresText = "";
        for (int i = 0; i < top.Count; i++)
        {
            namesText += top[i].name + "\n";
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
