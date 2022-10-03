using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Player : Singleton<Player>, ISaveable
{
    public GameObject player;

    private Camera mainCamera;

    private bool _playerMovementIsDisabled = false;
    public bool PlayerMovementIsDisabled { get => _playerMovementIsDisabled; set => _playerMovementIsDisabled = value; }

    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null;

    private PlayerController playerController;

    private Direction playerDirection;

    private string _iSaveableUniqueID;

    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObejctSave;

    public GameObjectSave GameObjectSave { get { return _gameObejctSave; } set { _gameObejctSave = value; } }

    public Animator playerAnim;

    public Vector3 playerLastPosition;

    private bool _playerOnOtherObject = false;

    public bool PlayerOnOtherObject { get =>_playerOnOtherObject; set => _playerOnOtherObject = value; }
    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;

        playerController = GetComponent<PlayerController>();

        //Get unieuq id for gameobject and create save data object
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;

        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneLoadFadeInEvent += EnablePlayerMovement;
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.BeforeSceneUnloadFadeOutEvent -= DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneLoadFadeInEvent -= EnablePlayerMovement;
    }

    public void ClearCarriedItem()
    {
        equippedItemSpriteRenderer.sprite = null;
        equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        GetComponent<PlayerController>().isCarrying = false;
    }

    public void ShowCarriedItem(int itemCode)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);
        if(itemDetails != null)
        {
            equippedItemSpriteRenderer.sprite = itemDetails.itemSprite;
            equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);

            GetComponent<PlayerController>().isCarrying = true;
        }
    }

    public Vector3 GetPlayerViewportPosition()
    {
        //Vector3 viewport position for player((0,0) viewport bottom left, (1,1) viewport top right
        return mainCamera.WorldToViewportPoint(transform.position);
    }

    public Vector3 GetPlayerCenterPosition()
    {
        return new Vector3(transform.position.x, transform.position.y /*+ Settings.playerCenterYOffSet*/, transform.position.z);
    }

    public void DisablePlayerInputAndResetMovement()
    {
        DisablePlayerMovement();
        GetComponent<PlayerController>().ResetMovement();
    }

    public void DisablePlayerMovement()
    {
        PlayerMovementIsDisabled = true;
    }

    public void EnablePlayerMovement()
    {
        PlayerMovementIsDisabled = false;
    }

    //for test purpose only
    public void PlayerTestInput()
    {
        //TriggerAdvanceTime
        if (Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }

        //trigger advance day
        if (Input.GetKeyDown(KeyCode.G))
        {
            TimeManager.Instance.AdvanceGameDay();
        }

    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        //Delete saveScene for game object if it already exists
        GameObjectSave.sceneData.Remove(Settings.PreLoadScene);

        //Create saveScene for gameObject
        SceneSave sceneSave = new SceneSave();

        //Create Vector3 Dictionary
        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>();

        //Create string dictionary 
        sceneSave.stringDictionary = new Dictionary<string, string>();

        //Add player position to Vector3 dictionary
        if(PlayerOnOtherObject == true)
        {
            Vector3Serializable vector3Serializable = new Vector3Serializable(Player.Instance.playerLastPosition.x, Player.Instance.playerLastPosition.y, Player.Instance.playerLastPosition.z);
            sceneSave.vector3Dictionary.Add("playerPosition", vector3Serializable);
        }
        else
        {
            Vector3Serializable vector3Serializable = new Vector3Serializable(transform.position.x, transform.position.y, transform.position.z);
            sceneSave.vector3Dictionary.Add("playerPosition", vector3Serializable);

        }
        //Add current scene name to string dictionary
        sceneSave.stringDictionary.Add("currentScene", SceneManager.GetActiveScene().name);

        //Add player direction to string dictionary
        sceneSave.stringDictionary.Add("playerDirection", playerController.direction.ToString());

        //Add scenesave data for player game object
        GameObjectSave.sceneData.Add(Settings.PreLoadScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            //Get save data dicitonary for scene
            if(gameObjectSave.sceneData.TryGetValue(Settings.PreLoadScene, out SceneSave sceneSave))
            {
                //Get player position
                if(sceneSave.vector3Dictionary != null && sceneSave.vector3Dictionary.TryGetValue("playerPosition", out Vector3Serializable playerPosition))
                {
                    transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
                }

                if (PlayerOnOtherObject == true)
                {
                    PlayerOnOtherObject = false;
                }

                //Get string dictionary
                if(sceneSave.stringDictionary != null)
                {
                    //Get player scene
                    if(sceneSave.stringDictionary.TryGetValue("currentScene", out string currentScene))
                    {
                        SceneControllerManager.Instance.FadeAndLoadScene(currentScene, transform.position);
                    }

                    //get playerDirection
                    if(sceneSave.stringDictionary.TryGetValue("playerDirection", out string playerDir))
                    {
                        bool playerDirFound = Enum.TryParse<Direction>(playerDir, true, out Direction direction);

                        if (playerDirFound)
                        {
                            playerDirection = direction;
                            SetPlayerDirection(playerDirection);
                        }
                    }
                }
            }
        }
    }

    public void ChangeSortingLayer(int layerOrder)
    {
        GetComponent<SortingGroup>().sortingOrder = layerOrder;
    }
    private void SetPlayerDirection(Direction playerDirection)
    {
        switch(playerDirection)
        {
            case Direction.Up:
                //set idle up
                playerController.SetPlayerAnimationUp();
                break;
            case Direction.Down:
                //set idle down
                playerController.SetPlayerAnimationDown();
                break;
            case Direction.Right:
                //set idle right
                playerController.SetPlayerAnimationRight();
                break;
            case Direction.Left:
                //set idle left
                playerController.SetPlayerAnimationLeft();
                break;
            default:
                //Set idle down
                playerController.SetPlayerAnimationDown();
                break;
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        //Nothing required here since the player is on a preload scene
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        //nothing required here since the player is on a preload scene
    }
}
