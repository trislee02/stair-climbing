using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private string scoreRecordsPath = "";

    private Dictionary<string, int> scoreRecordsMap = new Dictionary<string, int>();// name => score
    private ScoreRecord currentPlayingScoreRecord = new ScoreRecord();

    List<ScoreRecord> readScoreRecordsFromFile(string path)
    {
        if (scoreRecordsPath == "")
        {
            Debug.LogError("Score log path is not set");
            return new List<ScoreRecord>();
        }

        // handle if file does not exist
        if (!File.Exists(path))
        {
            return new List<ScoreRecord>();
        }

        var serializer = new JsonSerializer();
        List<ScoreRecord> records = new();
        using (var streamReader = new StreamReader(path))
        using (var textReader = new JsonTextReader(streamReader))
        {
            records = serializer.Deserialize<List<ScoreRecord>>(textReader);
        }
        return records;
    }

    Dictionary<string, int> convertScoreRecordsListToMap(List<ScoreRecord> list)
    {
        if (list == null)
        {
            return new Dictionary<string, int>();
        }

        Dictionary<string, int> map = new Dictionary<string, int>();
        foreach (var record in list)
        {
            map[record.name] = record.score;
        }
        return map;
    }

    List<ScoreRecord> convertScoreRecordsMapToList(Dictionary<string, int> map)
    {
        if (map == null)
        {
            return new List<ScoreRecord>();
        }

        List<ScoreRecord> list = new List<ScoreRecord>();
        foreach (var pair in map)
        {
            list.Add(new ScoreRecord { name = pair.Key, score = pair.Value });
        }
        return list;
    }

    void storeScoreRecordsMapToFile(string path)
    {
        if (scoreRecordsPath == "")
        {
            return;
        }

        List<ScoreRecord> records = convertScoreRecordsMapToList(scoreRecordsMap);

        var serializer = new JsonSerializer();
        using (var streamWriter = new StreamWriter(path))
        using (var textWriter = new JsonTextWriter(streamWriter))
        {
            serializer.Serialize(textWriter, records);
        }
    }

    void addScoreRecordToMap(ScoreRecord record)
    {
        if (record == null) return;
        if (scoreRecordsMap.ContainsKey(record.name))
        {
            scoreRecordsMap[record.name] = Mathf.Max(scoreRecordsMap[record.name], record.score);
        }
        else
        {
            scoreRecordsMap[record.name] = record.score;
        }
    }

    public void createNewPlayingScoreRecord(string playerName)
    {
        this.currentPlayingScoreRecord = new ScoreRecord();
        this.currentPlayingScoreRecord.name = playerName;
        this.currentPlayingScoreRecord.score = 0;
    }

    public void addToCurrentPlayingScoreRecord(int score)
    {
        if (this.currentPlayingScoreRecord == null) return;
        this.currentPlayingScoreRecord.score += score;
    }

    public void saveCurrentPlayingScoreRecord()
    {
        if (this.currentPlayingScoreRecord == null) return;
        addScoreRecordToMap(this.currentPlayingScoreRecord);
        storeScoreRecordsMapToFile(scoreRecordsPath);
    }

    public ScoreRecord getHighestScore()
    {
        // Find the highest score record in the map
        ScoreRecord highestScoreRecord = new ScoreRecord();
        foreach (var pair in scoreRecordsMap)
        {
            if (pair.Value > highestScoreRecord.score)
            {
                highestScoreRecord.name = pair.Key;
                highestScoreRecord.score = pair.Value;
            }
        }

        return highestScoreRecord.name != null ? highestScoreRecord : null;
    }

    public List<ScoreRecord> getTopN(int n)
    {
        List<ScoreRecord> topN = new List<ScoreRecord>();
        var sortedRecords = scoreRecordsMap.OrderByDescending(pair => pair.Value).ToList();
        for (int i = 0; i < Mathf.Min(n, sortedRecords.Count); i++)
        {
            topN.Add(new ScoreRecord { name = sortedRecords[i].Key, score = sortedRecords[i].Value });
        }
        return topN;
    }

    public ScoreRecord getCurrentPlayingScore()
    {
        return this.currentPlayingScoreRecord;
    }

    public void init(string playerName)
    {
        this.createNewPlayingScoreRecord(playerName);
    }

    // Start is called before the first frame update
    void Start()
    {
        var records = readScoreRecordsFromFile(scoreRecordsPath);
        scoreRecordsMap = convertScoreRecordsListToMap(records);

        //Debug.Log("Score records:");
        //foreach (var pair in scoreRecordsMap)
        //{
        //    Debug.Log(pair.Key + ": " + pair.Value);
        //}

        //// Add a new score record
        //scoreRecordsMap["Player1"] = 100;

        //storeScoreRecordsMapToFile(scoreRecordsPath);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
