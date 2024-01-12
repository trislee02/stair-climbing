using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

[Serializable]
public class Data
{
    public float roll1;
    public float roll2;
}

[Serializable]
public class DataMessage
{
    public string op; // Operator
    public Data info;

    public static DataMessage CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<DataMessage>(jsonString);
    }
}