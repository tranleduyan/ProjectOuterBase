using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    [Header("Tiles & Tilemap References")]
    [Header("Options")]
    [SerializeField] private bool observeMovementPenalties = true;

    [Range(0, 20)]
    [SerializeField] private int pathMovementPenalty = 0;
    [Range(0, 20)]
    [SerializeField] private int defaultMovementPenalty = 0;

    private GridNodes gridNodes;
    private Node startNode;
    private Node targetNode;
    private int gridWidth;
    private int gridHeight;
    private int originX;
    private int originY;

    private List<Node> openNodeList;
    private HashSet<Node> closedNodeList;

    private bool pathFound = false;

    /// <summary>
    /// Builds a path for the given SceneName from the startGridPosition to the endGridPosition, and adds movements steps to the passed in npcMovementStack. also returns true if path found
    /// or false if no path found
    /// </summary>
    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        pathFound = false;

        if(PopulateGridNodesFromGridPropertiesDictionary(sceneName, startGridPosition, endGridPosition))
        {
            if (FindShortestPath())
            {
                UpdatePathOnNPCMovementStepStack(sceneName, npcMovementStepStack);

                return true;
            }
        }
        return false;
    }

    private bool FindShortestPath()
    {
        //Add start node to open list
        openNodeList.Add(startNode);

        //Loop through open node list until empty
        while (openNodeList.Count > 0)
        {
            //sort list
            openNodeList.Sort();

            //current node = the node in th eopen list with the lowest fCost
            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            //Add current node to the closed list
            closedNodeList.Add(currentNode);

            //if the current node = target node
            // then finish
            if (currentNode == targetNode)
            {
                pathFound = true;
                break;
            }

            //evaulate fCost for each neighbor of the current node
            EvaluateCurrentNodeNeighbors(currentNode);
        }

        if (pathFound)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void EvaluateCurrentNodeNeighbors(Node currentNode)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighborNode;

        //loop through all directions
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                if(i == 0 && j == 0)
                {
                    continue;
                }
                validNeighborNode = GetValidNodeNeighbor(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j);
                if(validNeighborNode != null)
                {
                    //calculate new gCost for neighbor
                    int newCostToNeighbor;

                    if (observeMovementPenalties)
                    {
                        newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, validNeighborNode) + validNeighborNode.movementPenalty;
                    }
                    else
                    {
                        newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, validNeighborNode);
                    }

                    bool isValidNeighborNodeInOpenList = openNodeList.Contains(validNeighborNode);

                    if(newCostToNeighbor < validNeighborNode.gCost || !isValidNeighborNodeInOpenList)
                    {
                        validNeighborNode.gCost = newCostToNeighbor;
                        validNeighborNode.hCost = GetDistance(validNeighborNode, targetNode);

                        validNeighborNode.parentNode = currentNode;

                        if (!isValidNeighborNodeInOpenList)
                        {
                            openNodeList.Add(validNeighborNode);
                        }
                    }
                }
            }
        }
    }

    private Node GetValidNodeNeighbor(int neighborNodeXPosition, int neighborNodeYPosition)
    {
        //if neighbornode position is beyond grid then reutrn null
        if (neighborNodeXPosition >= gridWidth || neighborNodeXPosition < 0 || neighborNodeYPosition >= gridHeight || neighborNodeYPosition < 0)
        {
            return null;
        }

        //if neighbor is an obstacle or neighbor is in the closed list then skip
        Node neighborNode = gridNodes.GetGridNode(neighborNodeXPosition, neighborNodeYPosition);

        if (neighborNode.isObstacle || closedNodeList.Contains(neighborNode))
        {
            return null;
        }
        else
        {
            return neighborNode;
        }
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int distanceY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if(distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
    private void UpdatePathOnNPCMovementStepStack(SceneName sceneName, Stack<NPCMovementStep> npcMovementStepStack)
    {
        Node nextNode = targetNode;
        while(nextNode != null)
        {
            NPCMovementStep npcMovementStep = new NPCMovementStep();

            npcMovementStep.sceneName = sceneName;
            npcMovementStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);

            npcMovementStepStack.Push(npcMovementStep);

            nextNode = nextNode.parentNode;
        }
    }

    private bool PopulateGridNodesFromGridPropertiesDictionary(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition)
    {
        //Get grid properties dictionary for the scene
        SceneSave sceneSave;

        if(GridPropertiesManager.Instance.GameObjectSave.sceneData.TryGetValue(sceneName.ToString(), out sceneSave))
        {
            //Get dict grid propertyDetails
            if(sceneSave.gridPropertyDetailsDictionary != null)
            {

                //Get grid height and width
                if(GridPropertiesManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
                {
                    //Create nodes grid based on grid properties dictionary
                    gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                    gridWidth = gridDimensions.x;
                    gridHeight = gridDimensions.y;
                    originX = gridOrigin.x;
                    originY = gridOrigin.y;

                    //Create openNodeList
                    openNodeList = new List<Node>();

                    //Create closed nodeList
                    closedNodeList = new HashSet<Node>();
                }
                else
                {
                    return false;
                }

                //populate start node
                startNode = gridNodes.GetGridNode(startGridPosition.x - gridOrigin.x, startGridPosition.y - gridOrigin.y);

                //populate target node
                targetNode = gridNodes.GetGridNode(endGridPosition.x - gridOrigin.x, endGridPosition.y - gridOrigin.y);

                //populate obstacle and path info for grid
                for(int x = 0; x < gridDimensions.x; x++)
                {
                    for(int y = 0; y < gridDimensions.y; y++)
                    {
                        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(x + gridOrigin.x, y + gridOrigin.y, sceneSave.gridPropertyDetailsDictionary);

                        if(gridPropertyDetails != null)
                        {
                            //if npc obstacle
                            if (gridPropertyDetails.isNPCObstacle == true)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.isObstacle = true;
                            }
                            else if (gridPropertyDetails.isPath == true)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.movementPenalty = pathMovementPenalty;
                            }
                            else
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.movementPenalty = defaultMovementPenalty;
                            }
                        }
                    }
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        return true;
    }
}
