using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : Singleton<GridPropertiesManager>, ISaveable
{
    private Transform cropParentTransform;
    private Tilemap dugGround;
    private Tilemap waterGround;
    private bool isFirstTimeSceneLoaded = true;
    private Grid grid;
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;
    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;
    [SerializeField] private SO_GridProperties[] so_gridPropertiesArray = null;
    [SerializeField] private Tile[] dugGroundTile = null;
    [SerializeField] private Tile[] wateredGroundTile = null;

    private string _iSaveableUniqueID;

    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;

    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent += AdvanceDay;
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent -= AdvanceDay;
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            //Restore Data for Current Scene
            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

    private void AfterSceneLoaded()
    {
        //Get Grid
        grid = GameObject.FindObjectOfType<Grid>();

        //Get tilemaps
        dugGround = GameObject.FindGameObjectWithTag(Tags.dugGround).GetComponent<Tilemap>();
        waterGround = GameObject.FindGameObjectWithTag(Tags.waterGround).GetComponent<Tilemap>();

        if(GameObject.FindGameObjectWithTag(Tags.CropsParentTransform) != null)
        {
            cropParentTransform = GameObject.FindGameObjectWithTag(Tags.CropsParentTransform).transform;
        }
        else
        {
            cropParentTransform = null;
        }
    }

    private void Start()
    {
        InitialiseGridProperties();
    }

    private void ClearDisplayGroundDecoration()
    {
        //remove ground decorations;
        dugGround.ClearAllTiles();
        waterGround.ClearAllTiles();
    }

    private void ClearDisplayAllPlantedCrops()
    {
        //Destroy all crops in scene
        Crop[] cropArray;
        cropArray = FindObjectsOfType<Crop>();

        foreach(Crop crop in cropArray)
        {
            Destroy(crop.gameObject);
        }
    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecoration();

        ClearDisplayAllPlantedCrops();
    }

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        //Dug
        if (gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }

    public void DisplayWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        //water
        if (gridPropertyDetails.daysSinceWatered > -1)
        {
            ConnectWateredGround(gridPropertyDetails);
        }
    }

    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        //Select tile based on surrounding dug tiles

        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        dugGround.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);

        //set 4 tiles if dug surrounding current tile - up down left right now that this central tile has been dug

        GridPropertyDetails adjacentGridPropertyDetails;

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if(adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            dugGround.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), dugTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile2 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            dugGround.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), dugTile2);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX -1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile3 = SetDugTile(gridPropertyDetails.gridX-1, gridPropertyDetails.gridY);
            dugGround.SetTile(new Vector3Int(gridPropertyDetails.gridX -1, gridPropertyDetails.gridY, 0), dugTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile4 = SetDugTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            dugGround.SetTile(new Vector3Int(gridPropertyDetails.gridX+1, gridPropertyDetails.gridY, 0), dugTile4);
        }
    }

    private void ConnectWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        //Select tile based on surrounding watered tiles
        Tile WateredTile0 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        waterGround.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), WateredTile0);

        //set 4 tiles if watered surrounding current tile - up down left right now that this central tile has been watered

        GridPropertyDetails adjacentGridPropertyDetails;

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile WateredTile1 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            waterGround.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), WateredTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile WateredTile2 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            waterGround.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), WateredTile2);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile WateredTile3 = SetWateredTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            waterGround.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), WateredTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
        {
            Tile WateredTile4 = SetWateredTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            waterGround.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), WateredTile4);
        }

    }

    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.seedItemCode > -1)
        {
            //get crop details
            CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

            if (cropDetails != null && cropDetails.isConnectedCrop == false)
            {
                //prefab to use
                GameObject cropPrefab;

                //instantiate crop prefab at grid location
                int growthStages = cropDetails.growthDays.Length;

                int currentGrowthStage = 0;

                for (int i = growthStages - 1; i >= 0; i--)
                {
                    if (gridPropertyDetails.growthDays >= cropDetails.growthDays[i])
                    {
                        currentGrowthStage = i;
                        break;
                    }
                }

                cropPrefab = cropDetails.growthPrefab[currentGrowthStage];

                Sprite growthSprite = cropDetails.growthSprite[currentGrowthStage];

                Vector3 worldPosition = waterGround.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));

                worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2, worldPosition.y, worldPosition.z);

                GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);

                cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;
                cropInstance.transform.SetParent(cropParentTransform);
                cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            }
            else if(cropDetails != null && cropDetails.isConnectedCrop == true)
            {
                //prefab to use
                GameObject cropPrefab;

                int growthStages = cropDetails.growthDays.Length;

                int currentGrowthStage = 0;

                int growthPrefabStage = SetCropPrefabs(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

                gridPropertyDetails.growthDays = growthPrefabStage;

                for(int i = growthStages -1; i >= 0; i--)
                {
                    if(gridPropertyDetails.growthDays >= cropDetails.growthDays[i])
                    {
                        currentGrowthStage = i;
                        break;
                    }
                }

                cropPrefab = cropDetails.growthPrefab[currentGrowthStage];

                Sprite growthSprite = cropDetails.growthSprite[currentGrowthStage];

                Vector3 worldPosition = waterGround.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));

                worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2, worldPosition.y, worldPosition.z);

                GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);

                cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;
                cropInstance.transform.SetParent(cropParentTransform);
                cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            }
            
        }
    }

    private int SetCropPrefabs(int xGrid, int yGrid, GridPropertyDetails gridPropertyDetails)
    {
        bool upCrop = IsConnectedCrop(xGrid, yGrid + 1, gridPropertyDetails.seedItemCode);
        bool downCrop = IsConnectedCrop(xGrid, yGrid - 1, gridPropertyDetails.seedItemCode);
        bool leftCrop = IsConnectedCrop(xGrid - 1, yGrid, gridPropertyDetails.seedItemCode);
        bool rightCrop = IsConnectedCrop(xGrid + 1, yGrid, gridPropertyDetails.seedItemCode);

        #region Set appropriate tile based on whether surrounding tiles are dug or not
        if(!upCrop && !downCrop && !rightCrop && !leftCrop)
        {
            return 0;
        }
        else if(!upCrop && downCrop && rightCrop && !leftCrop)
        {
            return 1;
        }
        else if (!upCrop && downCrop && rightCrop && leftCrop)
        {
            return 2;
        }
        else if (!upCrop && downCrop && !rightCrop && leftCrop)
        {
            return 3;
        }
        else if (!upCrop && downCrop && !rightCrop && !leftCrop)
        {
            return 4;
        }
        else if (upCrop && downCrop && rightCrop && !leftCrop)
        {
            return 5;
        }
        else if (upCrop && downCrop && rightCrop && leftCrop)
        {
            return 6;
        }
        else if (upCrop && downCrop && !rightCrop && leftCrop)
        {
            return 7;
        }
        else if (upCrop && downCrop && !rightCrop && !leftCrop)
        {
            return 8;
        }
        else if (upCrop && !downCrop && rightCrop && !leftCrop)
        {
            return 9;
        }
        else if (upCrop && !downCrop && rightCrop && leftCrop)
        {
            return 10;
        }
        else if (upCrop && !downCrop && !rightCrop && leftCrop)
        {
            return 11;
        }
        else if(upCrop && !downCrop && !rightCrop && !leftCrop)
        {
            return 12;
        }
        else if(!upCrop && !downCrop && rightCrop && !leftCrop)
        {
            return 13;
        }
        else if(!upCrop && !downCrop && rightCrop && leftCrop)
        {
            return 14;
        }
        else if(!upCrop && !downCrop && !rightCrop && leftCrop)
        {
            return 15;
        }
        return -1;
        #endregion
    }

    private bool IsConnectedCrop(int xGrid, int yGrid, int itemCode)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if (gridPropertyDetails.seedItemCode == itemCode)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private Tile SetDugTile(int xGrid, int yGrid)
    {
        //Get whether surrounding tiles (up down left and right) are dug are not;
        bool upDug = IsGridSquareDug(xGrid, yGrid + 1);
        bool downDug = IsGridSquareDug(xGrid, yGrid - 1);
        bool leftDug = IsGridSquareDug(xGrid - 1, yGrid);
        bool rightDug = IsGridSquareDug(xGrid + 1, yGrid);

        #region Set appropriate tile based on whether surrounding tiles are dug or not

        if (!upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGroundTile[0];
        }
        else if (!upDug && downDug && rightDug && !leftDug)
        {
            return dugGroundTile[1];
        }
        else if (!upDug && downDug && rightDug && leftDug)
        {
            return dugGroundTile[2];
        }
        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return dugGroundTile[3];
        }
        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return dugGroundTile[4];
        }
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return dugGroundTile[5];
        }
        else if(upDug && downDug && rightDug && leftDug)
        {
            return dugGroundTile[6];
        }
        else if(upDug && downDug && !rightDug && leftDug)
        {
            return dugGroundTile[7];
        }
        else if(upDug && downDug && !rightDug && !leftDug)
        {
            return dugGroundTile[8];
        }
        else if(upDug && !downDug && rightDug && !leftDug)
        {
            return dugGroundTile[9];
        }
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return dugGroundTile[10];
        }
        else if(upDug && !downDug && !rightDug && leftDug)
        {
            return dugGroundTile[11];
        }
        else if(upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGroundTile[12];
        }
        else if(!upDug && !downDug && rightDug && !leftDug)
        {
            return dugGroundTile[13];
        }
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return dugGroundTile[14];
        }
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return dugGroundTile[15];
        }
        return null;
        #endregion SetAppropriate tile based on whether surrounding tiles are dug or not
    }

    private bool IsGridSquareDug(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if(gridPropertyDetails == null)
        {
            return false;
        }
        else if(gridPropertyDetails.daysSinceDug > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Tile SetWateredTile(int xGrid, int yGrid)
    {
        //Get whether surrounding tiles (up down left and right) are watered are not;
        bool upWatered = IsGridSquareWatered(xGrid, yGrid + 1);
        bool downWatered = IsGridSquareWatered(xGrid, yGrid - 1);
        bool leftWatered = IsGridSquareWatered(xGrid - 1, yGrid);
        bool rightWatered = IsGridSquareWatered(xGrid + 1, yGrid);

        #region Set appropriate tile based on whether surrounding tiles are watered or not

        if (!upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGroundTile[0];
        }
        else if (!upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGroundTile[1];
        }
        else if (!upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGroundTile[2];
        }
        else if (!upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGroundTile[3];
        }
        else if (!upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGroundTile[4];
        }
        else if (upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGroundTile[5];
        }
        else if (upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGroundTile[6];
        }
        else if (upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGroundTile[7];
        }
        else if (upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGroundTile[8];
        }
        else if (upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGroundTile[9];
        }
        else if (upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGroundTile[10];
        }
        else if (upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGroundTile[11];
        }
        else if (upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGroundTile[12];
        }
        else if (!upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGroundTile[13];
        }
        else if (!upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGroundTile[14];
        }
        else if (!upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGroundTile[15];
        }
        return null;
        #endregion SetAppropriate tile based on whether surrounding tiles are watered or not
    }

    private bool IsGridSquareWatered(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if (gridPropertyDetails == null)
        {
            return false;
        }

        else if (gridPropertyDetails.daysSinceWatered > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CallClearDisplayGridPropertyDetails()
    {
        ClearDisplayGridPropertyDetails();
    }

    public void CallDisplayGridPropertyDetails()
    {
        DisplayGridPropertyDetails();
    }

    private void DisplayGridPropertyDetails()
    {
        //loop through all grid items
        foreach(KeyValuePair<string, GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;

            DisplayDugGround(gridPropertyDetails);
            DisplayWateredGround(gridPropertyDetails);

            DisplayPlantedCrop(gridPropertyDetails);
        }
    }

    

    //This initialises the grid property dictionary with the values from the SO_GridProperties assets and stores the values for each scene in GameObjectSave sceneData
    private void InitialiseGridProperties()
    {
        //loop through all gridproperties in the array
        foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            //create dictionary of grid property details
            Dictionary<string, GridPropertyDetails> gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();

            //populate grid property dictionary - iterate through all the grid properties in the so_gridproperties list
            foreach (GridProperty gridProperty in so_GridProperties.gridPropertyList)
            {
                GridPropertyDetails gridPropertyDetails;
                gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDictionary);

                if (gridPropertyDetails == null)
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }

                switch (gridProperty.gridBoolProperty)
                {
                    case GridBoolProperty.diggable:
                        gridPropertyDetails.isDiggable = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.canDropItem:
                        gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.canPlaceFurniture:
                        gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.isPath:
                        gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                        break;
                    case GridBoolProperty.isNPCObstacle:
                        gridPropertyDetails.isNPCObstacle = gridProperty.gridBoolValue;
                        break;
                    default:
                        break;
                }

                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, gridPropertyDictionary);
            }

            //Create scene save for this gameObject
            SceneSave sceneSave = new SceneSave();

            //add grid property dictionary to scene save data
            sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

            //If starting scene set the gridPropertyDictionary memeber variable to the current iteration
            if (so_GridProperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this.gridPropertyDictionary = gridPropertyDictionary;
            }

            sceneSave.boolDictionary = new Dictionary<string, bool>();
            sceneSave.boolDictionary.Add("isFirstimeSceneLoaded", true);

            //Add scene save to game object scene data
            GameObjectSave.sceneData.Add(so_GridProperties.sceneName.ToString(), sceneSave);
        }
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridPropertyDictionary);
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        //construct key from coordinate
        string key = "x" + gridX + "y" + gridY;
        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        //Set Value
        gridPropertyDictionary[key] = gridPropertyDetails;
    }

    //return the gridPropertyDetails at the gridLocation for the supplied dictionary, or null if no properties exist at that location
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        //Construct key from coordinate
        string key = "x" + gridX + "y" + gridY;

        GridPropertyDetails gridPropertyDetails;

        //Check if grid property details exist for coordinate and retrieve
        if(!gridPropertyDictionary.TryGetValue(key, out gridPropertyDetails))
        {
            //if not found
            return null;
        }
        else
        {
            return gridPropertyDetails;
        }
    }

    //return the crop object at the gridx, gridy position or null if no crop was found
    public Crop GetCropObjectAtGridLocation(GridPropertyDetails gridPropertyDetails)
    {
        Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(worldPosition);

        //loop through colliders to get crop game object
        Crop crop = null;

        for (int i = 0; i < collider2DArray.Length; i++)
        {
            crop = collider2DArray[i].gameObject.GetComponentInParent<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
            {
                break;
            }
            crop = collider2DArray[i].gameObject.GetComponentInChildren<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
            {
                break;
            }
        }
        return crop;
    }

    public CropDetails GetCropDetails(int seedItemCode)
    {
        return so_CropDetailsList.GetCropDetails(seedItemCode);
    }

    //for sceneName this method returns a Vector2Int with the grid dimensions for that scene or Vector2Int.zero if scene not found
    public bool GetGridDimensions(SceneName sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin)
    {
        gridDimensions = Vector2Int.zero;
        gridOrigin = Vector2Int.zero;

        //loop through scenes
        foreach(SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            if(so_GridProperties.sceneName == sceneName)
            {
                gridDimensions.x = so_GridProperties.gridWidth;
                gridDimensions.y = so_GridProperties.gridHeight;

                gridOrigin.x = so_GridProperties.originX;
                gridOrigin.y = so_GridProperties.originY;

                return true;
            }
        }

        return false;
    }

    //get the gridpropertydetails for the tile at gridx, gridy. if no grid property details exist null is returned and can assume that all grid property details values are null or false
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, gridPropertyDictionary);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        //Get sceneSave for scene - it exists since we created it in initialise
        if(GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            //get grid property details dictionary - it exists since we created it in initialise
            if(sceneSave.gridPropertyDetailsDictionary != null)
            {
                gridPropertyDictionary = sceneSave.gridPropertyDetailsDictionary;
            }

            if(sceneSave.boolDictionary != null && sceneSave.boolDictionary.TryGetValue("isFirstimeSceneLoaded", out bool storedIsFirstTimeSceneLoaded))
            {
                isFirstTimeSceneLoaded = storedIsFirstTimeSceneLoaded;
            }
            if (isFirstTimeSceneLoaded)
            {
                EventHandler.CallInstantiateCropPrefabsEvent();
            }
            //if(gridProperties exist
            if(gridPropertyDictionary.Count > 0)
            {
                //Grid property details found for the current scene destroy existing ground decoration
                ClearDisplayGridPropertyDetails();

                //Instantiate gridProperty details for current scene
                DisplayGridPropertyDetails();
            }

            if(isFirstTimeSceneLoaded == true)
            {
                isFirstTimeSceneLoaded = false;
            }
        }
    }

    public GameObjectSave ISaveableSave()
    {
        //Store current scene data
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

        return GameObjectSave;
    }
    public void ISaveableStoreScene(string sceneName)
    {
        //Remove sceneSave for scene
        GameObjectSave.sceneData.Remove(sceneName);

        //create sceneSave for scene
        SceneSave sceneSave = new SceneSave();

        //create & add dict grid property details dictionary
        sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

        //create and add bool dictionary for the first time scene loaded
        sceneSave.boolDictionary = new Dictionary<string, bool>();
        sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded", isFirstTimeSceneLoaded);

        //add scene save to game object scene data
        GameObjectSave.sceneData.Add(sceneName, sceneSave); 
    }

    private void AdvanceDay(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        //Clear display all grid property details
        ClearDisplayGridPropertyDetails();

        //Loop through all scenes by looping through all gridproperties in the array
        foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            //Get gridpropertydetails dictionary for scene
            if (GameObjectSave.sceneData.TryGetValue(so_GridProperties.sceneName.ToString(), out SceneSave sceneSave))
            {
                if (sceneSave.gridPropertyDetailsDictionary != null)
                {
                    for (int i = sceneSave.gridPropertyDetailsDictionary.Count - 1; i >= 0; i--)
                    {
                        KeyValuePair<string, GridPropertyDetails> item = sceneSave.gridPropertyDetailsDictionary.ElementAt(i);

                        GridPropertyDetails gridPropertyDetails = item.Value;

                        #region update all grid properties to reflect the advance in the day

                        //if ground is watered then clear water
                        if (gridPropertyDetails.daysSinceWatered > -1)
                        {
                            gridPropertyDetails.daysSinceWatered = -1;
                        }

                        //If a crop is planted
                        if(gridPropertyDetails.growthDays > -1)
                        {
                            gridPropertyDetails.growthDays += 1;
                        }

                        //Set gridpropertydetails
                        SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave.gridPropertyDetailsDictionary);

                        #endregion update all grid properties to reflect the advance in the day
                    }
                }
            }
        }

        //Display gridproperty details to reflect changed values
        DisplayGridPropertyDetails();
    }
}
