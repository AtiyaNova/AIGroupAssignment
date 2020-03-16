using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Using enums to set the type of tile
public enum TileType
{ 
    building = 0, 
    road = 1, 
}


//Atiya Nova 2020/03/12
//Class that manages the construction and logic of the grid
public class GridManager : MonoBehaviour
{
    //the static variables
    static int rowSize = 12, colSize = 12;
    static float distVal = 1.7f;

    //public variables for the tiles
    public static GridTile[,] theMaze = new GridTile[rowSize, colSize];
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
                //This creates an aesthetic representation of the grid tile
                GameObject newTile = GameObject.Instantiate(tilePrefab);
                newTile.transform.position = new Vector3(i * distVal, 0, j * distVal);
                newTile.transform.parent = this.transform;
                theMaze[i, j] = newTile.GetComponent<GridTile>();

                //TODO: set the values of the tiles properly
                //this is just placeholder
                if (i % 2 == 0 && j % 2 == 0) //this is where I'm placing the buildings--they're not traversable
                {
                    theMaze[i, j].SetProperties(TileType.building, 0, 0,i,j);
                }
                else //the roads are traversable thus they have a reward and quality
                {
                    theMaze[i, j].SetProperties(TileType.road, -0.1, 12,i,j);
                }

       
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
                //some random joociness
                theObject.GetComponent<Renderer>().material = buildingMats[Random.Range(0, buildingMats.Length)];
                theObject.GetComponent<MeshFilter>().mesh = buildingMeshes[Random.Range(0, buildingMeshes.Length)];
                break;
        }
    }

    //TODO: Create a function that returns a list of possible moves
    static List<GridTile> GetNextStates(int x, int y)
    {
        //the potential moves will be stored in a list of gridtiles
        List<GridTile> result = new List<GridTile>();

        //Cities are in a gridlike format--the AI will either go horizontally, or vertically
        //TODO: Clean this up!
        if ((x+1)<rowSize-1)
        {
            if (theMaze[x + 1, y].theType == TileType.road) result.Add(theMaze[x + 1, y]);
        }

        if ((x-1)>0)
        {
            if (theMaze[x - 1, y].theType == TileType.road) result.Add(theMaze[x - 1, y]);
        }

        if ((y + 1) < colSize-1)
        {
            if (theMaze[x, y+1].theType == TileType.road) result.Add(theMaze[x, y+1]);
        }

        if ((y - 1) > 0)
        {
            if (theMaze[x, y - 1].theType == TileType.road) result.Add(theMaze[x, y - 1]);
        }

        return result;
    }

    //TODO: Create a function that sometimes gets a random move
    //Because Q-Learning occaisonally takes advantage of random moves
    static GridTile GetRandomState(int x, int y)
    {
        List<GridTile> nextStates = GetNextStates(x,y);
        int count = nextStates.Count;
        int chosen = Random.Range(0, count);
        return nextStates[chosen];
    }

    //TODO: Create a function that trains the agent
    static void Train(int goal, double gamma, double lrnRate, int epochs)
    {
        //epoch is a fancy word for iterations
        for (int i = 0; i < epochs; ++i)
        {
            //we're supposed to set this to a random value
            //I'm just setting it to 0, 0 for now?
            Vector2 cState = new Vector2(0, 0);

            while (true)
            {
                //we find a random next state
                GridTile nextState = GetRandomState((int)cState.x, (int)cState.y);
                List<GridTile> possFutureStates = GetNextStates((int)nextState.x, (int)nextState.y);
                double maxQ = double.MinValue;

                //TODO: Implement rest of the learning
            }
        }
    }

    //TODO: Create a function that picks the most optimal delivery route
    //the Q-Learning optimization will go here
    void PerformDeliveries()
    {
        
    }

}
