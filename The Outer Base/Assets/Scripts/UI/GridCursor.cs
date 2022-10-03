using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;
    [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;

    private bool _cursorPositionIsValid = false;
    
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private int _itemUseGridRadius = 0;

    public int ItemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value; }

    private ItemType _selectedItemType;
    
    public ItemType SelectedItemType {  get => _selectedItemType; set => _selectedItemType = value; }

    private bool _cursorIsEnabled = false;

    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

    private void SceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
    }

    private Vector3Int DisplayCursor()
    {
        if(grid != null)
        {

            //get grid position for cursor
            Vector3Int gridPosition = GetGridPositionForCursor();

            //Get grid position for player
            Vector3Int playerGridPosition = GetGridPositionForPlayer();

            //Set cursor sprite
            SetCursorValidity(gridPosition, playerGridPosition);

            //Get rect transform position for our cursor
            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

            return gridPosition;
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    //Called in the UIInventorySlot.ClearCursors() method when an inventory slot item is no longer selected
    public void DisableCursor() 
    {
        cursorImage.color = Color.clear;
        CursorIsEnabled = false; 
    }

    //called in the UIInventorySlot.SetSelectedItem() method when an inventory slot item is selected and its itemUseGridRadius > 0
    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnabled = true;
    }

    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid();

        //Check item use radius is valid
        if(Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius || Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            SetCursorToInvalid();
            return;
        }

        //GetSelectedItemDetails
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if(itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        //Get gridProperty details at cursor position
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if(gridPropertyDetails != null)
        {

            //Determine cursor validity based on inventory item selected and grid property details
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (!IsCursorValidForSeed(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                case ItemType.Commodity:
                    if (!IsCursorValidForCommodity(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.Furniture:
                    if (!IsCursorValidForFurniture(gridPropertyDetails, itemDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.Tree_Seed:
                    if(!IsCursorValidForTreeSeed(gridPropertyDetails, itemDetails))
                    {
                        SetCursorToInvalid();
                    }
                    break;
                case ItemType.Watering_tool:
                case ItemType.Breaking_tool:
                case ItemType.Chopping_tool:
                case ItemType.Reaping_tool:
                case ItemType.Collecting_tool:
                case ItemType.Hoeing_tool:
                    if(!IsCursorValidForTool(gridPropertyDetails, itemDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.none:
                    break;

                case ItemType.count:
                    break;

                default:
                    break;
            }
        }
        else
        {
            SetCursorToInvalid();
            return;
        }
    }

    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }
    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }

    //Test cursor validity for a commodity for the target gridPropertyDetails. return true if valid, false if invalid
    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    //Test cursor validity for a seed for the target gridPropertyDetails. return true if valid, false if invalid
    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }
    private bool IsCursorValidForFurniture(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if(gridPropertyDetails.seedItemCode == -1 && gridPropertyDetails != null)
        {
            if(itemDetails.needSpaceX == false && itemDetails.needSpaceY == false)
                return gridPropertyDetails.canPlaceFurniture;
            else if(itemDetails.needSpaceX == true && itemDetails.needSpaceY == false && gridPropertyDetails.canPlaceFurniture)
            {
                GridPropertyDetails rightAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
                GridPropertyDetails leftAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
                if(rightAdjacentGridPropertyDetails != null && leftAdjacentGridPropertyDetails != null && rightAdjacentGridPropertyDetails.seedItemCode == -1 &&
                    leftAdjacentGridPropertyDetails.seedItemCode == -1 && leftAdjacentGridPropertyDetails.canPlaceFurniture && rightAdjacentGridPropertyDetails.canPlaceFurniture)
                {
                    return true;
                }
                else {
                    return false; 
                }
            }
            else if (itemDetails.needSpaceX == false && itemDetails.needSpaceY == true && gridPropertyDetails.canPlaceFurniture)
            {
                GridPropertyDetails upAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
                if (upAdjacentGridPropertyDetails != null && upAdjacentGridPropertyDetails.seedItemCode == -1 || upAdjacentGridPropertyDetails != null
                && upAdjacentGridPropertyDetails.seedItemCode == -1 && upAdjacentGridPropertyDetails.isExtendFurnitureWall)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if(itemDetails.needSpaceX == true && itemDetails.needSpaceY == true && gridPropertyDetails.canPlaceFurniture)
            {
                GridPropertyDetails rightAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
                GridPropertyDetails leftAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
                GridPropertyDetails upAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
                GridPropertyDetails upRightAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY + 1);
                GridPropertyDetails upLeftAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY + 1);
                if (rightAdjacentGridPropertyDetails != null && rightAdjacentGridPropertyDetails.seedItemCode == -1
                && leftAdjacentGridPropertyDetails != null && leftAdjacentGridPropertyDetails.seedItemCode == -1
                && upAdjacentGridPropertyDetails != null && upAdjacentGridPropertyDetails.seedItemCode == -1
                && upRightAdjacentGridPropertyDetails != null && upRightAdjacentGridPropertyDetails.seedItemCode == -1
                && upLeftAdjacentGridPropertyDetails != null && upLeftAdjacentGridPropertyDetails.seedItemCode == -1
                || rightAdjacentGridPropertyDetails != null && rightAdjacentGridPropertyDetails.seedItemCode == -1
                && leftAdjacentGridPropertyDetails != null && leftAdjacentGridPropertyDetails.seedItemCode == -1
                && upAdjacentGridPropertyDetails != null && upAdjacentGridPropertyDetails.seedItemCode == -1 && upAdjacentGridPropertyDetails.isExtendFurnitureWall
                && upRightAdjacentGridPropertyDetails != null && upRightAdjacentGridPropertyDetails.seedItemCode == -1 && upRightAdjacentGridPropertyDetails.isExtendFurnitureWall
                && upLeftAdjacentGridPropertyDetails != null && upLeftAdjacentGridPropertyDetails.seedItemCode == -1 && upLeftAdjacentGridPropertyDetails.isExtendFurnitureWall)
                {
                    return true;
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
        }
        else
        {
            return false;
        }
    }
    private bool IsCursorValidForTreeSeed(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if(itemDetails.itemType == ItemType.Tree_Seed)
        {
            if (gridPropertyDetails.isDiggable == true && gridPropertyDetails.daysSinceDug == -1 && gridPropertyDetails.seedItemCode == -1)
            {
                #region Need to get any items at location so we can check if they are reapable
                //Get world position for cursor
                Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                //Get List of items at cursor location
                List<Item> itemList = new List<Item>();

                HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition, Settings.cursorSize, 0f);
                #endregion

                // loop through items found to see if any are reapable type - we are not going to let the player dig where there are reapable scenary items
                bool foundReapable = false;
                foreach (Item item in itemList)
                {
                    if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                    {
                        foundReapable = true;
                        break;
                    }
                }
                if (foundReapable)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        return false;
    }
        //Sets the cursor as either valid or invalid for the tool for the target gridPropertyDetails. return true if valid and false if invalid
        private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        //Switch on Tool
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                if(gridPropertyDetails.isDiggable == true && gridPropertyDetails.daysSinceDug == -1 && gridPropertyDetails.seedItemCode == -1)
                {
                    #region Need to get any items at location so we can check if they are reapable
                    //Get world position for cursor
                    Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                    //Get List of items at cursor location
                    List<Item> itemList = new List<Item>();

                    HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition, Settings.cursorSize, 0f);
                    #endregion

                    //loop through items found to see if any are reapable type - we are not going to let the player dig where there are reapable scenary items
                    bool foundReapable = false;

                    foreach(Item item in itemList)
                    {
                        if(InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                        {
                            foundReapable = true;
                            break;
                        }
                    }

                    if (foundReapable)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            case ItemType.Watering_tool:
                if(gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.daysSinceWatered == -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case ItemType.Breaking_tool:
                //check if item can be harvested with item selected, check item is fully grown
                //check if seed planted
                if (gridPropertyDetails.seedItemCode != -1)
                {
                    //get crop details for seed
                    CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

                    //if crop details found
                    if (cropDetails != null)
                    {
                        //check if crop fully grown
                        if (gridPropertyDetails.growthDays >= cropDetails.growthDays[cropDetails.growthDays.Length - 1] && cropDetails.isConnectedCrop == false)
                        {
                            //check if crop can be harvested with tool selected
                            if (cropDetails.CanUseToolToHarvestCrop(itemDetails.itemCode))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if(cropDetails.isConnectedCrop == true)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return false;
            case ItemType.Chopping_tool:
            case ItemType.Collecting_tool:
                //check if item can be harvested with item selected, check item is fully grown
                //check if seed planted
                if(gridPropertyDetails.seedItemCode != -1)
                {
                    //get crop details for seed
                    CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

                    //if crop details found
                    if(cropDetails != null)
                    {
                        //check if crop fully grown
                        if (gridPropertyDetails.growthDays >= cropDetails.growthDays[cropDetails.growthDays.Length -1])
                        {
                            //check if crop can be harvested with tool selected
                            if (cropDetails.CanUseToolToHarvestCrop(itemDetails.itemCode))
                            {
                                return true;
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
                    }
                }
                return false;

            default:
                return false;
        }
    }


    public Vector3Int GetGridPositionForCursor()
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z)); //z is how far the object are in front of camera, camera is at -10 so objects are (-)-10 int front = 10
        return grid.WorldToCell(worldPosition);
    }

    public Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(Player.Instance.transform.position);
    }

    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas); //return the pixel value of the related current cursor and rectransform position 
    }

    public Vector3 GetWorldPositionForCursor()
    {
        return grid.CellToWorld(GetGridPositionForCursor());
    }
}
