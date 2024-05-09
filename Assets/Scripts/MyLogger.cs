using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using UnityEngine;

public class NumbersLog
{
    public List<float> numbers { get; set; }
    public long timestamp { get; set; }

    public NumbersLog(List<float> _n, long _t)
    {
        this.numbers = _n;
        this.timestamp = _t;
    }
}

public class MyLogger
{
    private long startTicks;
    //
    private int logCount;
    private int autoSaveAfterLogAmount;
    //
    private bool saved;
    //
    private string baseFilePath;
    //
    private ConcurrentQueue<NumbersLog> logsQueue = new ConcurrentQueue<NumbersLog>();

    public MyLogger(string _filePath, int _autoSaveAfterLogAmount=-1)
    {
        startTicks = 0;
        logCount = 0;
        autoSaveAfterLogAmount = _autoSaveAfterLogAmount;
        saved = false;
        baseFilePath = _filePath;
    }

    public void Start(string[] headers)
    {
        GameObject configGameObject = GameObject.Find("MyLoggerConfig");
        MyLoggerConfig config = configGameObject != null ? configGameObject.GetComponent<MyLoggerConfig>() : null;

        startTicks = DateTime.Now.Ticks;

        // Study_Condition_NămThángNgày_GiờPhút_IDParticipant_Group_FootLog_Left.csv
        string study = config != null ? config.study : "study";
        string condition = config != null ? config.condition : "condition";
        string participantID = config != null ? config.participantID : "pid";
        string group = config != null ? config.group : "group";
        string formattedDatTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        //
        string[] baseFilePathSplt = baseFilePath.Split('/');
        string fileName = baseFilePathSplt[baseFilePathSplt.Length - 1];
        string directory = baseFilePath.Replace(fileName, "");
        //
        string filePath = (directory + study + "_" + condition + "_" + formattedDatTime + "_" + participantID + "_" + group + "_" + fileName);

        Thread writingThread = new Thread(() => {
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            StreamWriter fileWriter = new StreamWriter(fileStream);
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                // set headers
                string headerLine = "";
                for (int i = 0; i < headers.Length; i++)
                {
                    if (i > 0) headerLine += ",";
                    headerLine += headers[i];
                }
                writer.WriteLine(headerLine);

                while (!saved)
                {
                    if (logsQueue.IsEmpty) continue;
                    NumbersLog log;
                    if (logsQueue.TryDequeue(out log))
                    {
                        string numberLine = "" + log.timestamp;
                        for (int i = 0; i < log.numbers.Count; i++)
                        {
                            numberLine += ",";
                            numberLine += log.numbers[i];
                        }
                        // Debug.Log("logger: " + log.timestamp + " " + numberLine);
                        fileWriter.WriteLine(numberLine);
                        logCount++;
                        if (autoSaveAfterLogAmount > 0 && logCount > autoSaveAfterLogAmount)
                        {
                            Save();
                            break;
                        }
                    }
                }
            }
        });
        writingThread.IsBackground = true;
        writingThread.Start();
    }

    public void Push(List<float> nums)
    {
        if (saved) { return ; }
        //TimeSpan elapse = new TimeSpan(DateTime.Now.Ticks - startTicks);
        //long timestamp = (long)Math.Floor(elapse.TotalMilliseconds);
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        NumbersLog log = new NumbersLog(nums, timestamp);
        logsQueue.Enqueue(log);
    }

    public void Save()
    {
        saved = true;
        Debug.Log("Save logger!!!");
    }
}
