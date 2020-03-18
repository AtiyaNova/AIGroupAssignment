using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{//using a class to store the grid information

    public TileType theType;
    public double reward, quality;
    public int x, y;

    public void SetProperties(TileType newType, int newX, int newY)
    {
        theType = newType;
        x = newX;
        y = newY;
    }
}
