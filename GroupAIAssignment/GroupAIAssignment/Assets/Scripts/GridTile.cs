﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{//using a class to store the grid information

    public TileType theType;
    public double reward, quality;

    public void SetProperties(TileType newType, double newReward, double newQuality)
    {
        theType = newType;
        reward = newReward;
        quality = newQuality;
    }
}