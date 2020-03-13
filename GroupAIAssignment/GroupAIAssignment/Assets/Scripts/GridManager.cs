using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Using enums to set the type of tile
public enum TileType
{ 
    building = 0, 
    road = 1, 
}

//using a struct to store the grid information
public struct GridTile
{
    public TileType theType;
    public double reward, quality;

    public GridTile(TileType newType, double newReward, double newQuality)
    {
        theType = newType;
        reward = newReward;
        quality = newQuality;
    }
}

//Atiya Nova 2020/03/12
//Class that manages the construction and logic of the grid
public class GridManager : MonoBehaviour
{
    //the static variables
    static int rowSize = 12, colSize = 12;
    static float distVal = 1.7f;

    //public variables for the tiles
    public GridTile[,] theMaze = new GridTile[rowSize, colSize];
    public GameObject tilePrefab;
    public Material roadMat; 
    public Mesh roadMesh;
    public Material[] buildingMats;
    public Mesh[] buildingMeshes;

    void Start()
    {
        //Initializes and creates the grid
        for (int i = 0; i < rowSize; i++)
        {
            for (int j = 0; j < colSize; j++)
            {
                //TODO: set the values of the tiles properly
                //this is just placeholder
                if (i % 2 == 0 && j % 2 == 0) //this is where I'm placing the buildings--they're not traversable
                {
                    theMaze[i, j] = new GridTile(TileType.building, 0, 0);
                }
                else //the roads are traversable thus they have a reward and quality
                {
                    theMaze[i, j] = new GridTile(TileType.road, -0.1, 12);
                }

                //This creates an aesthetic representation of the grid tile
                GameObject newTile = GameObject.Instantiate(tilePrefab);
                newTile.transform.position = new Vector3(i * distVal, 0, j * distVal);
                newTile.transform.parent = this.transform;
                SetAesthetic(newTile, theMaze[i, j].theType);
            }
        }
    }
    
    //This sets the aesthetic of the grid
    void SetAesthetic(GameObject theObject, TileType type)
    {
        switch (type) 
        {
            case TileType.road:
                theObject.GetComponent<Renderer>().material = roadMat;
                theObject.GetComponent<MeshFilter>().mesh = roadMesh;
                break;
            case TileType.building:
                int chosenBuilding = Random.Range(0, buildingMeshes.Length); //randomly generates which building mesh is used
                theObject.GetComponent<Renderer>().material = buildingMats[chosenBuilding];
                theObject.GetComponent<MeshFilter>().mesh = buildingMeshes[chosenBuilding];
                break;
        }
    }
}
