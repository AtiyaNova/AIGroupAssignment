using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Using enums to set the type of tile
public enum TileType
{ 
    building = 0, 
    road = 1, 
    delivery = 2,
    goal =3
}
public struct StateTransitions
{
    public int transition;
    public double reward;
    public double quality;

}

//Atiya Nova 2020/03/12
//Class that manages the construction and logic of the grid
public class GridManager : MonoBehaviour
{
    //the static variables
    static int rowSize, colSize = 12;
    static int stateSize = rowSize * colSize;
    static float distVal = 1.7f;
    public int maxEpochs = 1000;
    public float learningRate = 0.5f;
    public float gammaRate = 0.5f;
    //public variables for the tiles
    public static GridTile[,] theMaze = new GridTile[colSize, rowSize];
    public static StateTransitions[,] mazeTransitions = new StateTransitions[stateSize,stateSize];
    public GameObject tilePrefab;
    public Material roadMat; 
    public Mesh roadMesh;
    public Material goalMat;
    public Mesh goalMesh;
    public Material deliveryMat;
    public Mesh deliveryMesh;
    public Material[] buildingMats;
    public Mesh[] buildingMeshes;
    bool doneComp = false;
    int currentState = 0;
    int goalState = stateSize-1;
    void Start()
    {
       
        //Initializes and creates the grid
        for (int i = 0; i < colSize; i++)
        {
            for (int j = 0; j < rowSize; j++)
            {
                //This creates an aesthetic representation of the grid tile
                GameObject newTile = Instantiate(tilePrefab);
                newTile.transform.position = new Vector3(i * distVal, 0, j * distVal);
                newTile.transform.parent = transform;
                theMaze[i, j] = newTile.GetComponent<GridTile>();

                //TODO: set the values of the tiles properly
                //this is just placeholder
                if (i % 2 == 0 && j % 2 == 0) //this is where I'm placing the buildings--they're not traversable
                {
                    theMaze[i, j].SetProperties(TileType.building,i,j);
                }
                else //the roads are traversable thus they have a reward and quality
                {
                    theMaze[i, j].SetProperties(TileType.road,i,j);
                }

       
                SetAesthetic(newTile, theMaze[i, j].theType);
            }
        }

        theMaze[colSize - 1, rowSize - 1].SetProperties(TileType.goal, colSize - 1, rowSize - 1);

        //setting the delivery logic
        int deliveryMax = 5;
        int delCounter = 0;

        //setting the delivery points
        while (true)
        {
            int x = Random.Range(0, rowSize);
            int y = Random.Range(0, colSize);

            if (theMaze[x, y].theType == TileType.road )
            {
                print("New delivery point: " + x + " " + y);
                theMaze[x, y].SetProperties(TileType.delivery, x, y);
                delCounter++;
            }

            if (delCounter >= deliveryMax) break;
        }

        for (int i = 0; i < colSize; i++)
        {
            for (int j = 0; j < rowSize; j++)
            {
                if (j + 1 < rowSize)
                {
                    if (theMaze[i, j + 1].theType != TileType.building)
                    {
                        mazeTransitions[(j + (i * colSize)), ((j + 1) + (i * colSize))].transition = 1;
                        mazeTransitions[(j + (i * colSize)), ((j + 1) + (i * colSize))].reward = GetReward(theMaze[i, j + 1].theType);
                    }
                }

                if (i + 1 < colSize)
                {
                    if (theMaze[i + 1, j].theType != TileType.building)
                    {
                        mazeTransitions[(j + (i * colSize)), ((j) + ((i + 1) * colSize))].transition = 1;
                        mazeTransitions[(j + (i * colSize)), ((j) + ((i + 1) * colSize))].reward = GetReward(theMaze[i + 1, j].theType);
                        
                    }
                }

                if (j > 0)
                {
                    if (theMaze[i, j - 1].theType != TileType.building)
                    {
                        mazeTransitions[(j + (i * colSize)), ((j - 1) + (i * colSize))].transition = 1;
                        mazeTransitions[(j + (i * colSize)), ((j - 1) + (i * colSize))].reward = GetReward(theMaze[i, j-1].theType);
                    }

                }

                if (i > 0)
                {
                    if (theMaze[i - 1, j].theType != TileType.building )
                    {
                        mazeTransitions[(j + (i * colSize)), ((j) + ((i - 1) * colSize))].transition = 1;                      
                        mazeTransitions[(j + (i * colSize)), ((j) + ((i - 1) * colSize))].reward = GetReward(theMaze[i - 1, j].theType);
                       
                    }
                }

            }
        }
        Debug.Log("Starting Q Matrix Computation for " + maxEpochs + " Epochs");
        Train(stateSize - 1,gammaRate,learningRate,maxEpochs);
        Debug.Log("Q Matrix Complete");
        Debug.Log("Starting Walkthough");
        doneComp = true;
    }

    void Update()
    {
        if (doneComp)
        {
            Walk();
        }
        Debug.Log((currentState % colSize) + " , "+ Mathf.Floor(currentState / rowSize));

    }
    double GetReward(TileType type)
    {
        switch (type)
        {
            default:
                break;
            case TileType.building:
                return 0;
            case TileType.road:
                return -0.1;
            case TileType.goal:
                return 25.0;
            case TileType.delivery:
                return 5.0;
        }
        return 0.0;
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
            case TileType.goal:
                //some random joociness
                theObject.GetComponent<Renderer>().material = goalMat;
                theObject.GetComponent<MeshFilter>().mesh = goalMesh;

                break;
            case TileType.delivery:
                //some random joociness
                theObject.GetComponent<Renderer>().material = deliveryMat;
                theObject.GetComponent<MeshFilter>().mesh = deliveryMesh;

                break;


                //TODO: add aesthetics for goal and delivery;
        }
    }

    //TODO: Create a function that returns a list of possible moves
    static List<int> GetNextState(int cState)
    {
        List<int> result = new List<int>();
        for (int j = 0; j < mazeTransitions.Length; ++j)
        {
            if (mazeTransitions[cState, j].transition == 1)
            {
                result.Add(j);
            }
        }
        return result;
    }

    //TODO: Create a function that sometimes gets a random move
    //Because Q-Learning occaisonally takes advantage of random moves
    static int GetRandomState(int cState)
    {
        List<int> nextStates = GetNextState(cState);
        int count = nextStates.Count;
        int chosen = Random.Range(0, count);
        return nextStates[chosen];
    }

    

    //TODO: Create a function that trains the agent
    static void Train(int goal, double gamma, double lrnRate, int epochs)
    {
        //epoch is a fancy word for iterations
        for (int i = 0; i < epochs; i++)
        {
            //we're supposed to set this to a random value
            //I'm just setting it to 0, 0 for now?
            Random.InitState((int)Time.deltaTime);
            int cState = Random.Range(0, theMaze.Length);


            while (true)
            {
                //we find a random next state
                int nextState = GetRandomState(cState);
                List<int> possFutureStates = GetNextState(nextState);
                double maxQ = double.MinValue;
                for (int j = 0; j < possFutureStates.Count; ++j)
                {
                    int futureState = possFutureStates[j];
                    double q = mazeTransitions[nextState, futureState].quality;
                    if (q > maxQ)
                    {
                        maxQ = q;
                    }

                }
                mazeTransitions[cState,nextState].quality =((1 - lrnRate) * mazeTransitions[cState,nextState].quality)+(lrnRate * (mazeTransitions[cState,nextState].reward + (gamma * maxQ)));
                cState = nextState;
                if (cState == goal) break;

            }
            Debug.Log("Progress: "+ ((((float)i)/((float)epochs))*100.0f)+"%");
        }
    }

     void Walk()
    {
        int nextState;
        if (currentState != goalState)
        {
            double[] possStatesQuality = new double[stateSize];
            for(int i = 0; i < possStatesQuality.Length; i++)
            {
                possStatesQuality[i] = mazeTransitions[currentState, i].quality;
            }
            nextState = ArgMax(possStatesQuality);
            currentState = nextState;
            /*currentState.x = nextState % colSize;
            currentState.y =Mathf.Floor( nextState/rowSize);*/
        }
        
    }

     int ArgMax(double[] quality)
    {
        double maxValue = quality[0];
        int index = 0;
        for (int i = 0; i < quality.Length; ++i)
        {
            if (quality[i] > maxValue)
            {
                maxValue = quality[i];
                index = i;
            }
        }
        return index;
    }


    

}
