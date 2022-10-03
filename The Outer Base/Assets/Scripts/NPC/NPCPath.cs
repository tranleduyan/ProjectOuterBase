using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
public class NPCPath : MonoBehaviour
{
    public Stack<NPCMovementStep> npcMovementStepStack;

    private NPCMovement npcMovement;

    private void Awake()
    {
        npcMovement = GetComponent<NPCMovement>();
        npcMovementStepStack = new Stack<NPCMovementStep>();
    }

    public void ClearPath()
    {
        npcMovementStepStack.Clear();
    }

    public void BuildPath(NPCScheduleEvent npcScheduleEvent)
    {
        ClearPath();

        //If Schedule event is for the same scene as the current npc scene
        if(npcScheduleEvent.toSceneName == npcMovement.npcCurrentScene)
        {
            Vector2Int npcCurrentGridPosition = (Vector2Int) npcMovement.npcCurrentGridPosition;

            Vector2Int npcTargetGridPosition = (Vector2Int)npcScheduleEvent.toGridCoordinate;

            //BuildPath and add movement steps to movement step stack
            NPCManager.Instance.BuildPath(npcScheduleEvent.toSceneName, npcCurrentGridPosition, npcTargetGridPosition, npcMovementStepStack);

        }

        else if(npcScheduleEvent.toSceneName != npcMovement.npcCurrentScene)
        {
            SceneRoute sceneRoute;

            //Get scene route matchingSchedule
            sceneRoute = NPCManager.Instance.GetSceneRoute(npcMovement.npcCurrentScene.ToString(), npcScheduleEvent.toSceneName.ToString());

            //has a valid sceneRoute bene found?
            if(sceneRoute != null)
            {
                //loop through scene paths in reverse order

                for(int i = sceneRoute.scenePathList.Count -1; i >= 0; i--)
                {
                    int toGridX, toGridY, fromGridX, fromGridY; 
                    
                    ScenePath scenePath = sceneRoute.scenePathList[i];

                    //Check if this is the final destination
                    if(scenePath.toGridCell.x >= Settings.maxGridWidth || scenePath.toGridCell.y >= Settings.maxGridHeight)
                    {
                        //if so we use final destiantion grid cell
                        toGridX = npcScheduleEvent.toGridCoordinate.x;
                        toGridY = npcScheduleEvent.toGridCoordinate.y;
                    }
                    else
                    {
                        //else use scene path to position
                        toGridX = scenePath.toGridCell.x;
                        toGridY = scenePath.toGridCell.y;
                    }

                    //check if this is the starting position
                    if(scenePath.fromGridCell.x >= Settings.maxGridWidth || scenePath.fromGridCell.y >= Settings.maxGridHeight)
                    {
                        //if so we use npc position
                        fromGridX = npcMovement.npcCurrentGridPosition.x;
                        fromGridY = npcMovement.npcCurrentGridPosition.y;
                    }
                    else
                    {
                        //else use scene path from position
                        fromGridX = scenePath.fromGridCell.x;
                        fromGridY = scenePath.fromGridCell.y;
                    }

                    Vector2Int fromGridPosition = new Vector2Int(fromGridX, fromGridY);

                    Vector2Int toGridPosition = new Vector2Int(toGridX, toGridY);

                    //build path and add movement steps to movement step stack
                    NPCManager.Instance.BuildPath(scenePath.sceneName, fromGridPosition, toGridPosition, npcMovementStepStack);
                }
                //Debug.Log(npcScheduleEvent.toSceneName);
            }
        }

        //if stack count > 1, update times and then pop off 1st item which is the starting position
        if (npcMovementStepStack.Count > 1)
        {
            UpdateTimesOnPath();
            npcMovementStepStack.Pop(); // discard starting step

            //set schedule event details in NPC movement
            npcMovement.SetScheduleEventDetails(npcScheduleEvent);
        }
    }

    //Update the path movement steps with expected gametime

    public void UpdateTimesOnPath()
    {
        //GetCurrent game time
        TimeSpan currentGameTime = TimeManager.Instance.GetGameTime();

        NPCMovementStep previousNPCMovementStep = null;

        foreach(NPCMovementStep npcMovementStep in npcMovementStepStack)
        {
            if(previousNPCMovementStep == null)
            {
                previousNPCMovementStep = npcMovementStep;
            }

            npcMovementStep.hour = currentGameTime.Hours;
            npcMovementStep.minute = currentGameTime.Minutes;
            npcMovementStep.second = currentGameTime.Seconds;

            TimeSpan movementTimeStep;

            //if diagonal
            if(MovementIsDiagonal(npcMovementStep, previousNPCMovementStep))
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / Settings.secondsPerGameSecond / npcMovement.npcNormalSpeed));
            }
            else
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Settings.gridCellSize / Settings.secondsPerGameSecond / npcMovement.npcNormalSpeed));
            }

            currentGameTime = currentGameTime.Add(movementTimeStep);

            previousNPCMovementStep = npcMovementStep;
        }
    }

    //return true if the previous movement step is diagonal to movement step, else return false.
    private bool MovementIsDiagonal(NPCMovementStep npcMovementStep, NPCMovementStep previousNPCMovementStep)
    {
        if((npcMovementStep.gridCoordinate.x != previousNPCMovementStep.gridCoordinate.x) && (npcMovementStep.gridCoordinate.y != previousNPCMovementStep.gridCoordinate.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
