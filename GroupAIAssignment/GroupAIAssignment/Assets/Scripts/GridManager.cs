using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *Artificial Intelligence Group Assignment 
 * Matthew Paraskevakos 100592704
 * Angela Tabafunda 100622426
 *Atiya Nova 100620165
 * Code based on: https://docs.microsoft.com/en-us/archive/msdn-magazine/2018/august/test-run-introduction-to-q-learning-using-csharp
 */

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
    public int transition;//is this state traversable
    public double reward;//the reward for traversing this state
    public double quality;//the q value of this state

}

//Atiya Nova 2020/03/12
//Class that manages the construction and logic of the grid
public class GridManager : MonoBehaviour
{
    //the static variables
    static int rowSize, colSize = 12;//length and width of our grid
    static int stateSize = rowSize * colSize;//size of our grid and total number of states
    static float distVal = 1.7f;
    public int maxEpochs = 1000;//number of iterations
    public float learningRate = 0.5f;//our learning rate changes the weight of our current and future results at the cost of the past results
    public float gammaRate = 0.5f;//changes the weight of future results
    //public variables for the tiles
    public static GridTile[,] theMaze = new GridTile[colSize, rowSize];//physical grid tile
    public static StateTransitions[,] mazeTransitions = new StateTransitions[stateSize,stateSize];//transtions between all states
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

        theMaze[colSize - 1, rowSize - 1].SetProperties(TileType.goal, colSize - 1, rowSize - 1);//set our goal

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
        //check all neighbooring tiles to see if the are traversable and what their reward is
        for (int i = 0; i < colSize; i++)
        {
            for (int j = 0; j < rowSize; j++)
            {
                if (j + 1 < rowSize)//if not out of range
                {
                    if (theMaze[i, j + 1].theType != TileType.building)//if not a building
                    {
                        mazeTransitions[(j + (i * colSize)), ((j + 1) + (i * colSize))].transition = 1;//set to 1 if traverable
                        mazeTransitions[(j + (i * colSize)), ((j + 1) + (i * colSize))].reward = GetReward(theMaze[i, j + 1].theType);//return reward based on tile type
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

    void Update()//update
    {
        if (doneComp)//if matrix has been computed
        {
            Walk();
            Debug.Log((currentState % colSize) + " , "+ Mathf.Floor(currentState / rowSize));
        }
       

    }
    double GetReward(TileType type)//return reward based in tile type
    {
        switch (type)
        {
            default:
                break;
            case TileType.building://buildings cant be traversed so the return nothing
                return 0;
            case TileType.road://we want to take as few roads as possible so the return a negative reward
                return -0.1;
            case TileType.goal://the goal is where we want to end up so it return a high reward
                return 25.0;
            case TileType.delivery://we want to visit as many delivery points as possible along the way so they return a reward as well
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

  
    static List<int> GetNextState(int cState) // returns a list of possible moves from the current state
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

    
    static int GetRandomState(int cState)//gets a random state from the list of possible states
    {
        List<int> nextStates = GetNextState(cState);
        int count = nextStates.Count;
        int chosen = Random.Range(0, count);
        return nextStates[chosen];
    }

    

   
    static void Train(int goal, double gamma, double lrnRate, int epochs) // trains the agent
    {
        //epoch is a fancy word for iterations
        for (int i = 0; i < epochs; i++)
        {
            //we're supposed to set this to a random value
            Random.InitState((int)Time.deltaTime);
            int cState = Random.Range(0, theMaze.Length);//current state


            while (true)
            {
                //we find a random next state
                int nextState = GetRandomState(cState);
                List<int> possFutureStates = GetNextState(nextState);//possible states
                double maxQ = double.MinValue;//initialize as some super minimum number
                for (int j = 0; j < possFutureStates.Count; ++j)//for each possible state
                {
                    int futureState = possFutureStates[j];//get the future state from the next one
                    double q = mazeTransitions[nextState, futureState].quality;//get the quality score
                    if (q > maxQ)//if it is less that the max quality
                    {
                        maxQ = q;
                    }

                }
                mazeTransitions[cState,nextState].quality =((1 - lrnRate) * mazeTransitions[cState,nextState].quality)+(lrnRate * (mazeTransitions[cState,nextState].reward + (gamma * maxQ)));//our bellman equation
                cState = nextState;
                if (cState == goal) break;//end if it reached the end

            }
            Debug.Log("Progress: "+ ((((float)i)/((float)epochs))*100.0f)+"%");
        }
    }

     void Walk()//moves the agent accross the board
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
            
        }
        
    }

     int ArgMax(double[] quality)//gets the best choice from the possible states
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
