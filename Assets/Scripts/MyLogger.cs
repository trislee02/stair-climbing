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

    public void Start()
    {
        startTicks = DateTime.Now.Ticks;

        string formattedDatTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filePath = baseFilePath + "." + formattedDatTime + ".csv";

        Thread writingThread = new Thread(() => {
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            StreamWriter fileWriter = new StreamWriter(fileStream);
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
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
