using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *Artificial Intelligence Group Assignment 
 * Matthew Paraskevakos 100592704
 * Angela Tabafunda 100622426
 *Atiya Nova 100620165
 */
public class GridTile : MonoBehaviour
{//using a class to store the grid information

    public TileType theType;
    public int x, y;

    public void SetProperties(TileType newType, int newX, int newY)
    {
        theType = newType;
        x = newX;
        y = newY;
    }
}
