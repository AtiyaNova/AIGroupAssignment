using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{//using a class to store the grid information

    public TileType theType;
    public double reward, quality;
    public int x, y;

    public void SetProperties(TileType newType, double newReward, double newQuality, int newX, int newY)
    {
        theType = newType;
        reward = newReward;
        quality = newQuality;
        x = newX;
        y = newY;
    }
}
