using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NPCPath))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent (typeof(BoxCollider2D))]
public class NPCMovement : MonoBehaviour
{
    public SceneName npcCurrentScene;
    [HideInInspector] public SceneName npcTargetScene;
    [HideInInspector] public Vector3Int npcCurrentGridPosition;
    [HideInInspector] public Vector3Int npcTargetGridPosition;
    [HideInInspector] public Vector3 npcTargetWorldPosition;
    public Direction npcFacingDirectionAtDestination;

    private SceneName npcPreviousMovementStepScene;
    private Vector3Int npcNextGridPosition;
    private Vector3 npcNextWorldPosition;

    [Header("NPC Movement")]
    public float npcNormalSpeed = 2f;

    [SerializeField] private float npcMinSpeed = 1f;
    [SerializeField] private float npcMaxSpeed = 3f;
    private bool npcIsMoving = false;

    [HideInInspector] public AnimationClip npcTargetAnimationClip;

    [Header("NPC Animation")]
    [SerializeField] private AnimationClip blankAnimation = null;

    private Grid grid;
    private Rigidbody2D rigidBody2D;
    private BoxCollider2D boxCollider2D;
    private WaitForFixedUpdate waitForFixedUpdate;
    private Animator animator;
    private AnimatorOverrideController animatorOverrideController;
    private int lastMoveAnimationParameter;
    private NPCPath npcPath;
    private bool npcInitialized = false;
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public bool npcActiveInScene = false;

    private bool sceneLoaded = false;

    private Coroutine moveToGridPositionRoutine;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloaded;
    }

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        npcPath = GetComponent<NPCPath>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        //Initialise target world position, target grid position & target scene to current
        npcTargetScene = npcCurrentScene;
        npcTargetGridPosition = npcCurrentGridPosition;
        npcTargetWorldPosition = transform.position;
    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();

        SetIdleAnimation();
    }

    private void FixedUpdate()
    {
        if (sceneLoaded)
        {
            if(npcIsMoving == false)
            {
                //SetNpc current and next grid location to take into account the npc might be animating
                npcCurrentGridPosition = GetGridPosition(transform.position);
                npcNextGridPosition = npcCurrentGridPosition;

                if(npcPath.npcMovementStepStack.Count > 0)
                {
                    NPCMovementStep npcMovementStep = npcPath.npcMovementStepStack.Peek();

                    npcCurrentScene = npcMovementStep.sceneName;

                    //If npc is abpout to move to a new scene, reset position to starting point in new scene and update the step time
                    if(npcCurrentScene != npcPreviousMovementStepScene)
                    {
                        npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        npcNextGridPosition = npcCurrentGridPosition;
                        transform.position = GetWorldPosition(npcCurrentGridPosition);
                        npcPreviousMovementStepScene = npcCurrentScene;
                        npcPath.UpdateTimesOnPath();
                    }

                    //if npc is in current scene then set npc to active to make visible, pop the movement step off the stack and then call the method to move npc
                    if(npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
                    {
                        SetNPCActiveInScene();
                        npcMovementStep = npcPath.npcMovementStepStack.Pop();
                        npcNextGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.hour, npcMovementStep.minute, npcMovementStep.second);
                        MoveToGridPosition(npcNextGridPosition, npcMovementStepTime, TimeManager.Instance.GetGameTime());
                    }

                    //else if npc is not in current scene then set npc to be invisible
                    //once the movement step time is less than game time(in the past) then pop movement step off the stack and set NPC position to movement step position
                    else
                    {
                        SetNPCInactiveInScene();
                        npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        npcNextGridPosition = npcCurrentGridPosition;
                        transform.position = GetWorldPosition(npcCurrentGridPosition);

                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.hour, npcMovementStep.minute, npcMovementStep.second);

                        TimeSpan gameTime = TimeManager.Instance.GetGameTime();

                        if(npcMovementStepTime < gameTime)
                        {
                            npcMovementStep = npcPath.npcMovementStepStack.Pop();
                            npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                            npcNextGridPosition = npcCurrentGridPosition;
                            transform.position = GetWorldPosition(npcCurrentGridPosition);
                        }
                    }
                }

                //else if no more npc movement steps
                else
                {
                    ResetMoveAnimation();

                    SetNPCFacingDirection();

                    SetNPCEventAnimation();   
                }
            }
        }
    }

    public void SetScheduleEventDetails(NPCScheduleEvent npcScheduleEvent)
    {
        npcTargetScene = npcScheduleEvent.toSceneName;
        npcTargetGridPosition = (Vector3Int)npcScheduleEvent.toGridCoordinate;
        npcTargetWorldPosition = GetWorldPosition(npcTargetGridPosition);
        npcFacingDirectionAtDestination = npcScheduleEvent.npcFacingDirectionAtDestination;
        npcTargetAnimationClip = npcScheduleEvent.animationAtDestination;
        ClearNPCEventAnimation();
    }
    public void SetNPCEventAnimation()
    {
        if(npcTargetAnimationClip != null)
        {
            ResetIdleAnimation();
            animatorOverrideController[blankAnimation] = npcTargetAnimationClip;
            animator.SetBool(Settings.eventAnimation, true);
        }
        else
        {
            animatorOverrideController[blankAnimation] = blankAnimation;
            animator.SetBool(Settings.eventAnimation, false);
        }
    }

    public void ClearNPCEventAnimation()
    {
        animatorOverrideController[blankAnimation] = blankAnimation;
        animator.SetBool(Settings.eventAnimation, false);

        //Clear any rotation on npc
        transform.rotation = Quaternion.identity;
    }

    private void SetNPCFacingDirection()
    {
        ResetIdleAnimation();

        switch (npcFacingDirectionAtDestination)
        {
            case Direction.Up:
                animator.SetBool(Settings.idleUp, true);
                break;

            case Direction.Down:
                animator.SetBool(Settings.idleDown, true);
                break;

            case Direction.Left:
                animator.SetBool(Settings.idleLeft, true);
                break;

            case Direction.Right:
                animator.SetBool(Settings.idleRight, true);
                break;

            case Direction.none:
                break;

            default:
                break;
        }
    }

    public void SetNPCActiveInScene()
    {
        spriteRenderer.enabled = true;
        boxCollider2D.enabled = true;
        npcActiveInScene = true;
    }

    public void SetNPCInactiveInScene()
    {
        spriteRenderer.enabled = false;
        boxCollider2D.enabled = false;
        npcActiveInScene = false;
    }

    private void AfterSceneLoad()
    {
        grid = GameObject.FindObjectOfType<Grid>();

        if (!npcInitialized)
        {
            InitializeNPC();
            npcInitialized = true;
        }

        sceneLoaded = true;
    }

    private void BeforeSceneUnloaded()
    {
        sceneLoaded = false;
    }

    //return the grid position given the world position
    private Vector3Int GetGridPosition(Vector3 worldPosition)
    {
        if (grid != null)
        {
            return grid.WorldToCell(worldPosition);
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    //returns the world position( center of grid square) from grid position
    public Vector3 GetWorldPosition(Vector3Int gridPosition)
    {
        Vector3 worldPosition = grid.CellToWorld(gridPosition);

        //GetCenter of grid square
        return new Vector3(worldPosition.x + Settings.gridCellSize / 2f, worldPosition.y + Settings.gridCellSize / 2f, worldPosition.z);
    }

    public void CancelNPCMovement()
    {
        npcPath.ClearPath();
        npcNextGridPosition = Vector3Int.zero;
        npcNextWorldPosition = Vector3.zero;
        npcIsMoving = false;

        if(moveToGridPositionRoutine != null)
        {
            StopCoroutine(moveToGridPositionRoutine);
        }

        //Reset move animation
        ResetMoveAnimation();

        //Clear event animation
        ClearNPCEventAnimation();
        //npcTargetAnimationClip = null;

        //Reset idel animation
        ResetIdleAnimation();

        //Set idle animation
        SetIdleAnimation();
    }

    private void InitializeNPC()
    {
        //Active in Scene
        if (npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
        {
            SetNPCActiveInScene();
        }
        else
        {
            SetNPCInactiveInScene();
        }

        npcPreviousMovementStepScene = npcCurrentScene;

        //Get NPC CurrentGrid position

        npcCurrentGridPosition = GetGridPosition(transform.position);

        //SetNext Grid position and target grid position to current grid position
        npcNextGridPosition = npcCurrentGridPosition;
        npcTargetGridPosition = npcCurrentGridPosition;
        npcTargetWorldPosition = GetWorldPosition(npcTargetGridPosition);

        //Get Npc world position
        npcNextWorldPosition = GetWorldPosition(npcCurrentGridPosition);
    }
  
    private void MoveToGridPosition(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        moveToGridPositionRoutine = StartCoroutine(MoveToGridPositionRoutine(gridPosition, npcMovementStepTime, gameTime));
    }

    private IEnumerator MoveToGridPositionRoutine(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        npcIsMoving = true;

        SetMoveAnimation(gridPosition);

        npcNextWorldPosition = GetWorldPosition(gridPosition);

        //ifMovement step time is in the future, otherwise skip and move NPC immediately to position
        if (npcMovementStepTime > gameTime)
        {
            //Calculate time difference in seconds
            float timeToMove = (float)(npcMovementStepTime.TotalSeconds - gameTime.TotalSeconds);

            //CalculateSpeed
            float npcCalculatedSpeed = Mathf.Max(npcMinSpeed, Vector3.Distance(transform.position, npcNextWorldPosition) / timeToMove / Settings.secondsPerGameSecond);

            //if speed is at least npc min speed and less than npc max speed then process, other wise skip and move npc immediateluy to position
            if (npcCalculatedSpeed <= npcMaxSpeed)
            {
                while (Vector3.Distance(transform.position, npcNextWorldPosition) > Settings.pixelSize)
                {
                    Vector3 unitVector = Vector3.Normalize(npcNextWorldPosition - transform.position);
                    Vector2 move = new Vector2(unitVector.x * npcCalculatedSpeed * Time.fixedDeltaTime, unitVector.y * npcCalculatedSpeed * Time.fixedDeltaTime);

                    rigidBody2D.MovePosition(rigidBody2D.position + move);

                    yield return waitForFixedUpdate;
                }
            }
        }
        rigidBody2D.position = npcNextWorldPosition;
        npcCurrentGridPosition = gridPosition;
        npcNextGridPosition = npcCurrentGridPosition;
        npcIsMoving = false;
    }

    private void SetMoveAnimation(Vector3Int gridPosition)
    {
        //Reset idle animation
        ResetIdleAnimation();

        //ResetMoveAnimation
        ResetMoveAnimation();

        //get world position
        Vector3 toWorldPosition = GetWorldPosition(gridPosition);

        //GetVector
        Vector3 directionVector = toWorldPosition - transform.position;

        if (Math.Abs(directionVector.x) >= Mathf.Abs(directionVector.y))
        {
            //use left/right animation
            if (directionVector.x > 0)
            {
                animator.SetBool(Settings.walkRight, true);
            }
            else
            {
                animator.SetBool(Settings.walkLeft, true);
            }
        }
        else
        {
            //use up down animation
            if (directionVector.y > 0)
            {
                animator.SetBool(Settings.walkUp, true);
            }
            else
            {
                animator.SetBool(Settings.walkDown, true);
            }
        }
    }

    private void SetIdleAnimation()
    {
        animator.SetBool(Settings.idleDown, true);
    }
    
    private void ResetMoveAnimation()
    {
        animator.SetBool(Settings.walkRight, false);
        animator.SetBool(Settings.walkLeft, false);
        animator.SetBool(Settings.walkUp, false);
        animator.SetBool(Settings.walkDown, false);
    }

    private void ResetIdleAnimation()
    {
        animator.SetBool(Settings.idleRight, false);
        animator.SetBool(Settings.idleLeft, false);
        animator.SetBool(Settings.idleUp, false);
        animator.SetBool(Settings.idleDown, false);
    }
}
