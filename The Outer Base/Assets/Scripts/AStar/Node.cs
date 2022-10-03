using System;
using UnityEngine;

public class Node : IComparable<Node>
{

    public Vector2Int gridPosition;
    public int gCost = 0; // distance from starting to node
    public int hCost = 0; //distance from node to finish
    public bool isObstacle = false;
    public int movementPenalty;
    public Node parentNode;

    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        parentNode = null;
    }

    //Fcost getter
    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        //Compare will be < 0 if this instance FCost is less than nodeToCompare.FCost
        //Compare will be >0 if this Instance FCost is greater than nodeToCompare.FCost
        //Compare will be == 0 if the values are the same

        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return compare;
    }
}
