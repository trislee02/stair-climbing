using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScheme
{
    public bool couldHasObstacles { get; set; } = false;
    public bool couldHasPickingUpItems { get; set; } = false;
    public bool couldHasSnowman { get; set; } = false;
    public int timeLimitAsSeconds { get; set; } = 180;
}
