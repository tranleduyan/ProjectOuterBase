using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D playerRB;
    private Animator playerAnim;
    public bool isCarrying;
    private GridCursor gridCursor;
    private Cursor cursor;

    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds useToolAnimationPause;
    private bool playerToolUseDisabled = false;

    private WaitForSeconds afterLiftToolAnimationPause;
    private WaitForSeconds liftToolAnimationPause;
    private WaitForSeconds afterPickAnimationPause;
    private WaitForSeconds pickAnimationPause;

    [HideInInspector] public Direction direction;

    private bool isPickingRight, isPickingLeft, isPickingDown, isPickingUp;
    private bool isUseToolRight, isUseToolLeft, isUseToolDown, isUseToolUp;

    [HideInInspector] public float lastMoveX, lastMoveY;

    private void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        gridCursor = FindObjectOfType<GridCursor>();
        cursor = FindObjectOfType<Cursor>();
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
        pickAnimationPause = new WaitForSeconds(Settings.pickAnimationPause);
        afterPickAnimationPause = new WaitForSeconds(Settings.afterPickAnimationPause);

    }

    private void Update()
    {
        PlayerClickInput();

        //for test
        GetComponent<Player>().PlayerTestInput();
    }

    private void FixedUpdate()
    {
        if (!GetComponent<Player>().PlayerMovementIsDisabled)
        {
            Move();
        }
        playerAnim.SetBool("isCarrying", isCarrying);

    }

    private void Move()
    {
        playerRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * speed * Time.fixedDeltaTime;
        
        playerAnim.SetFloat("moveX", playerRB.velocity.x);
        playerAnim.SetFloat("moveY", playerRB.velocity.y);

        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
        {
            lastMoveX = Input.GetAxisRaw("Horizontal");
            lastMoveY = Input.GetAxisRaw("Vertical");
            playerAnim.SetFloat("lastMoveX", Input.GetAxisRaw("Horizontal"));
            playerAnim.SetFloat("lastMoveY", Input.GetAxisRaw("Vertical"));

        }
        switch (Input.GetAxisRaw("Vertical"))
        {
            case (1):
                direction = Direction.Up;
                break;
            case (-1):
                direction = Direction.Down;
                break;
        }
        switch (Input.GetAxisRaw("Horizontal"))
        {
            case (1):
                direction = Direction.Right;
                break;
            case (-1):
                direction = Direction.Left;
                break;
        }
    }

    public void SetPlayerAnimationUp()
    {
        playerAnim.SetFloat("lastMoveX", 0);
        playerAnim.SetFloat("lastMoveY", 1);
    }

    public void SetPlayerAnimationDown()
    {
        playerAnim.SetFloat("lastMoveX", 0);
        playerAnim.SetFloat("lastMoveY", -1);
    }

    public void SetPlayerAnimationRight()
    {
        playerAnim.SetFloat("lastMoveX", 1);
        playerAnim.SetFloat("lastMoveY", 0);
    }

    public void SetPlayerAnimationLeft()
    {
        playerAnim.SetFloat("lastMoveX", -1);
        playerAnim.SetFloat("lastMoveY", 0);
    }

    private void PlayerClickInput()
    {
        if (!playerToolUseDisabled)
        {
            if (Input.GetMouseButton(0))
            {
                if (gridCursor.CursorIsEnabled || cursor.CursorIsEnabled)
                {
                    //Get Cursor grid position
                    Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();

                    //Get player grid position
                    Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();
                    ProcessPlayerClickInput(cursorGridPosition, playerGridPosition);
                }
            }
        }
    }

    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        ResetMovement();

        Vector3Int playerDirection = GetPlayerClickDirection(cursorGridPosition, playerGridPosition);

        //Get grid property Details at cursor position(the GridCursor validation routine ensures that grid property details are not null)
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        //GetSelectedItemDetails
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if(itemDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputSeed(gridPropertyDetails, itemDetails);
                    }
                    break;
                case ItemType.Tree_Seed:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputTreeSeed(gridPropertyDetails, itemDetails);
                    }
                    break;
                case ItemType.Furniture:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputFurniture(gridPropertyDetails, itemDetails);
                    }
                    break;
                case ItemType.Commodity:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputCommodity(itemDetails);
                    }
                    break;
                case ItemType.Breaking_tool:
                case ItemType.Watering_tool:
                case ItemType.Chopping_tool:
                case ItemType.Hoeing_tool:
                case ItemType.Reaping_tool:
                case ItemType.Collecting_tool:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemDetails, playerDirection);
                    break;

                case ItemType.none:
                    break;

                case ItemType.count:
                    break;

                default:
                    break;
            }
        }
    }

    private Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if (cursorGridPosition.x > playerGridPosition.x)
        {
            playerAnim.SetFloat("lastMoveX", 1);
            playerAnim.SetFloat("lastMoveY", 0);
            return Vector3Int.right;
        }
        else if (cursorGridPosition.x < playerGridPosition.x)
        {
            playerAnim.SetFloat("lastMoveX", -1);
            playerAnim.SetFloat("lastMoveY", 0);
            return Vector3Int.left;
        }
        else if(cursorGridPosition.y > playerGridPosition.y)
        {
            playerAnim.SetFloat("lastMoveX", 0);
            playerAnim.SetFloat("lastMoveY", 1);
            return Vector3Int.up;
        }
        else
        {
            playerAnim.SetFloat("lastMoveX", 0);
            playerAnim.SetFloat("lastMoveY", -1);
            return Vector3Int.down;
        }
    }

    private Vector3Int GetPlayerDirection(Vector3 cursorPosition, Vector3 playerPosition)
    {
        if (
            cursorPosition.x > playerPosition.x
            &&
            cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2f)
            &&
            cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2f)
            )
        {
            return Vector3Int.right;
        }
        else if(
            cursorPosition.x < playerPosition.x
            &&
            cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2f)
            &&
            cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2f)
            )
            {
            return Vector3Int.left;
        }
        else if (cursorPosition.y > playerPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            playerAnim.SetFloat("lastMoveX", 0);
            playerAnim.SetFloat("lastMoveY", -1);
            return Vector3Int.down;
        }
    }

    private void ProcessPlayerClickInputSeed(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {

        if(itemDetails.canBeDropped && gridCursor.CursorPositionIsValid && gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.seedItemCode == -1)
        {
            PlantSeedAtCursor(gridPropertyDetails, itemDetails);
        }

        else if(itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputTreeSeed(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {

        if (itemDetails.canBeDropped != true && gridCursor.CursorPositionIsValid && gridPropertyDetails.daysSinceDug <= -1 && gridPropertyDetails.seedItemCode == -1)
        {
            PlantSeedAtCursor(gridPropertyDetails, itemDetails);
        }

        else if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputFurniture(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {

        if (itemDetails.canBeDropped && itemDetails.needSpaceX == false && itemDetails.needSpaceY == false && gridCursor.CursorPositionIsValid && gridPropertyDetails.seedItemCode == -1)
        {
            PlantSeedAtCursor(gridPropertyDetails, itemDetails);
        }

        //for any items that take space x direction
        else if (itemDetails.canBeDropped && itemDetails.needSpaceX && itemDetails.needSpaceY == false && gridCursor.CursorPositionIsValid && gridPropertyDetails.seedItemCode == -1)
        {
            GridPropertyDetails rightAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            GridPropertyDetails leftAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);

            //conditions to set the grid properties
            if (rightAdjacentGridPropertyDetails != null && rightAdjacentGridPropertyDetails.seedItemCode == -1 && leftAdjacentGridPropertyDetails != null && leftAdjacentGridPropertyDetails.seedItemCode == -1)
            {
                PlantSeedAtCursor(gridPropertyDetails, itemDetails);

                //set the seed item code for two adjacents grid
                rightAdjacentGridPropertyDetails.seedItemCode = 0;
                leftAdjacentGridPropertyDetails.seedItemCode = 0;
                rightAdjacentGridPropertyDetails.growthDays = 0;
                leftAdjacentGridPropertyDetails.growthDays = 0;
            }
        }

        //for any items that take space y direction
        else if (itemDetails.canBeDropped && itemDetails.needSpaceX == false && itemDetails.needSpaceY == true && gridCursor.CursorPositionIsValid && gridPropertyDetails.seedItemCode == -1)
        {
            GridPropertyDetails upAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);

            //conditions to set the grid properties
            if (upAdjacentGridPropertyDetails != null && upAdjacentGridPropertyDetails.seedItemCode == -1 || upAdjacentGridPropertyDetails != null
                && upAdjacentGridPropertyDetails.seedItemCode == -1 && upAdjacentGridPropertyDetails.isExtendFurnitureWall)
            {
                PlantSeedAtCursor(gridPropertyDetails, itemDetails);

                //set the seed item code for the adjacent grid
                upAdjacentGridPropertyDetails.seedItemCode = 0;
                upAdjacentGridPropertyDetails.growthDays = 0;
            }
        }

        else if (itemDetails.canBeDropped && itemDetails.needSpaceX == true && itemDetails.needSpaceY == true && gridCursor.CursorPositionIsValid && gridPropertyDetails.seedItemCode == -1)
        {
            GridPropertyDetails rightAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            GridPropertyDetails leftAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            GridPropertyDetails upAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            GridPropertyDetails upRightAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY + 1);
            GridPropertyDetails upLeftAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY + 1);
            
            //conditions to set the grid properties
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
                PlantSeedAtCursor(gridPropertyDetails, itemDetails);

                //set the seed item code for the adjacent grids
                rightAdjacentGridPropertyDetails.seedItemCode = 0;
                leftAdjacentGridPropertyDetails.seedItemCode = 0;
                upAdjacentGridPropertyDetails.seedItemCode = 0;
                upRightAdjacentGridPropertyDetails.seedItemCode = 0;
                upLeftAdjacentGridPropertyDetails.seedItemCode = 0;
                rightAdjacentGridPropertyDetails.growthDays = 0;
                leftAdjacentGridPropertyDetails.growthDays = 0;
                upAdjacentGridPropertyDetails.growthDays = 0;
                upRightAdjacentGridPropertyDetails.growthDays = 0;
                upLeftAdjacentGridPropertyDetails.growthDays = 0;
            }
        }
        //for dropping furniture items
        /*else if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }*/
    }

    private void PlantSeedAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        //process if we have crop details for the seed
        if (GridPropertiesManager.Instance.GetCropDetails(itemDetails.itemCode) != null)
        {
            //update grid properties with seed details
            gridPropertyDetails.seedItemCode = itemDetails.itemCode;
            gridPropertyDetails.growthDays = 0;

            if (GridPropertiesManager.Instance.GetCropDetails(itemDetails.itemCode).isConnectedCrop == false)
            {
                //Display planted crop at grid property details
                GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
            }

            else if(GridPropertiesManager.Instance.GetCropDetails(itemDetails.itemCode).isConnectedCrop == true)
            {
                GridPropertiesManager.Instance.CallClearDisplayGridPropertyDetails();
                GridPropertiesManager.Instance.CallDisplayGridPropertyDetails();
            }

            //remove item from inventory
            EventHandler.CallRemoveSelectedItemFromInventoryEvent();
            
        }
    }

    private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        //Switch on tool
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    HoeGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;
            case ItemType.Watering_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    WaterGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;
            case ItemType.Chopping_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    ChopInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            case ItemType.Collecting_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    CollectInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            case ItemType.Breaking_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    BreakInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            case ItemType.Reaping_tool:
                if (cursor.CursorIsEnabled)
                {
                    playerDirection = GetPlayerDirection(cursor.GetWorldPositionForCursor(), GetComponent<Player>().GetPlayerCenterPosition());
                    ReapInPlayerDirectionAtCursor(itemDetails, playerDirection);
                }
                break;
            default:
                break;
        }
    }

    private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        //Trigger animation
        StartCoroutine(HoeGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }

        private IEnumerator HoeGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        GetComponent<Player>().PlayerMovementIsDisabled = true;
        playerToolUseDisabled = true;

        //Set tool animation to hoe
        
        playerAnim.SetTrigger("isUsingHoe"); // for tools part


        yield return useToolAnimationPause;

        //Set gridpropertydetails for dug ground
        if(gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
        }

        //Set grid property to dug
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails);

        yield return afterUseToolAnimationPause;

        GetComponent<Player>().PlayerMovementIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        StartCoroutine(WaterGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }

    private IEnumerator WaterGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        GetComponent<Player>().PlayerMovementIsDisabled = true;
        playerToolUseDisabled = true;

        //SetTool animation to watering can
        
        playerAnim.SetTrigger("isUsingWateringCan"); // for tools part

        //TODO: if there is water in the watering can
        //toolEffect = ToolEffect.watering;
        yield return liftToolAnimationPause;

        //Set gridpropertydetails for watered ground
        if (gridPropertyDetails.daysSinceWatered == -1)
        {
            gridPropertyDetails.daysSinceWatered = 0;
        }

        //Set grid property to watered
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        GridPropertiesManager.Instance.DisplayWateredGround(gridPropertyDetails);

        //after animation pause
        yield return afterUseToolAnimationPause;

        GetComponent<Player>().PlayerMovementIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void ChopInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        //Trigger animation
        StartCoroutine(ChopInPlayerDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }

    private IEnumerator ChopInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        GetComponent<Player>().PlayerMovementIsDisabled = true;
        playerToolUseDisabled = true;
        //Set tool animation to hoe
        playerAnim.SetTrigger("isUsingAxe"); // for tools part

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return useToolAnimationPause;

        //After animation pause
        yield return afterUseToolAnimationPause;

        GetComponent<Player>().PlayerMovementIsDisabled = false;
        playerToolUseDisabled = false;

    }

    private void CollectInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(CollectInPlayerDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }

    private IEnumerator CollectInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        GetComponent<Player>().PlayerMovementIsDisabled = true;
        playerToolUseDisabled = true;

        playerAnim.SetTrigger("isPickingUp"); // for  animation
        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return pickAnimationPause;

        //afteranimationpause
        yield return afterPickAnimationPause;

        GetComponent<Player>().PlayerMovementIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void BreakInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(BreakInPlayerDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }

    private IEnumerator BreakInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        GetComponent<Player>().PlayerMovementIsDisabled = true;
        playerToolUseDisabled = true;
        //Set tool animation to hoe
        playerAnim.SetTrigger("isUsingPickAxe"); // for tools part

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return useToolAnimationPause;
        //update surroundings isconnected crop when breaking

        //After animation pause
        yield return afterUseToolAnimationPause;

        GetComponent<Player>().PlayerMovementIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void ProcessCropWithEquippedItemInPlayerDirection(Vector3Int playerDirection, ItemDetails equippedItemDetails, GridPropertyDetails gridPropertyDetails)
    {
        switch (equippedItemDetails.itemType)
        {
            case ItemType.Chopping_tool:
            case ItemType.Breaking_tool:
                if (playerDirection == Vector3Int.right)
                {
                    isUseToolRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isUseToolLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isUseToolUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isUseToolDown = true;
                }
                break;

            case ItemType.Collecting_tool:
                if (playerDirection == Vector3Int.right)
                {
                    isPickingRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isPickingLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isPickingUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isPickingDown = true;
                }
                break;
            case ItemType.none:
                break;
        }

        //GetCrop at cursor grid location
        Crop crop = GridPropertiesManager.Instance.GetCropObjectAtGridLocation(gridPropertyDetails);

        //Execute process tool action for crop
        if(crop!= null)
        {
            switch (equippedItemDetails.itemType)
            {
                case ItemType.Breaking_tool:
                case ItemType.Chopping_tool:
                    crop.ProcessToolAction(equippedItemDetails, isUseToolRight, isUseToolLeft, isUseToolUp, isUseToolDown);
                    break;
                case ItemType.Collecting_tool:
                    crop.ProcessToolAction(equippedItemDetails, isPickingRight, isPickingLeft, isPickingDown, isPickingUp);
                    break;
            }
        }
    }

    private void ReapInPlayerDirectionAtCursor(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(ReapInPlayerDirectionAtCursorRoutine(itemDetails, playerDirection));
    }

    private IEnumerator ReapInPlayerDirectionAtCursorRoutine(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        GetComponent<Player>().PlayerMovementIsDisabled = true;
        playerToolUseDisabled = true;

        //SetTool animation to using scythe
         // for character
        playerAnim.SetTrigger("isUsingScythe"); // for tools part

        //reap in player direction
        UseToolInPlayerDirection(itemDetails, playerDirection);

        yield return useToolAnimationPause;

        GetComponent<Player>().PlayerMovementIsDisabled = false;
        playerToolUseDisabled = false;

    }

    private void UseToolInPlayerDirection(ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        if (Input.GetMouseButton(0))
        {
            switch (equippedItemDetails.itemType)
            {
                case ItemType.Reaping_tool:
                    if(playerDirection == Vector3Int.right)
                    {
                        playerAnim.SetFloat("lastMoveX", 1);
                        playerAnim.SetFloat("lastMoveY", 0);
                    }
                    else if(playerDirection == Vector3Int.left)
                    {
                        playerAnim.SetFloat("lastMoveX", -1);
                        playerAnim.SetFloat("lastMoveY", 0);
                    }
                    else if (playerDirection == Vector3Int.up)
                    {
                        playerAnim.SetFloat("lastMoveX", 0);
                        playerAnim.SetFloat("lastMoveY", 1);
                    }
                    else if(playerDirection == Vector3Int.down)
                    {
                        playerAnim.SetFloat("lastMoveX", 0);
                        playerAnim.SetFloat("lastMoveY", -1);
                    }
                    break;
            }

            //Define center point of square which will be used for collision testing
            Vector2 point = new Vector2(GetComponent<Player>().GetPlayerCenterPosition().x + (playerDirection.x * (equippedItemDetails.itemUseRadius / 2f)),
                GetComponent<Player>().GetPlayerCenterPosition().y + playerDirection.y * (equippedItemDetails.itemUseRadius / 2f));

            //Define size of the square which will be used for collision testing
            Vector2 size = new Vector2(equippedItemDetails.itemUseRadius, equippedItemDetails.itemUseRadius);

            //Get item component with 2D Collider located in the square at the center point defined (2d collider tested limited to maxColliderToTestPerReapSwing)
            Item[] itemArray = HelperMethods.GetComponentsAtBoxLocationNonAlloc<Item>(Settings.maxCollidersToTestPerReapSwing, point, size, 0f);
            int reapableItemCount = 0;

            //loop through all items retrieved
            for(int i = itemArray.Length - 1; i>= 0; i--)
            {
                if(itemArray[i] != null)
                {
                    //destroy item game object if reapable
                    if(InventoryManager.Instance.GetItemDetails(itemArray[i].ItemCode).itemType == ItemType.Reapable_scenary)
                    {
                        //effect position
                        Vector3 effectPosition = new Vector3(itemArray[i].transform.position.x, itemArray[i].transform.position.y + Settings.gridCellSize / 2f, 
                            itemArray[i].transform.position.z);
                        //trigger reaping effect
                        EventHandler.CallHarvestActionEffectEvent(effectPosition, HarvestActionEffect.reaping);

                        Destroy(itemArray[i].gameObject);

                        reapableItemCount++;
                        if (reapableItemCount >= Settings.maxTargetComponentsToDestroyPerReapSwing)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    public void ResetMovement()
    {
        playerRB.velocity = new Vector2(0, 0).normalized * speed * Time.fixedDeltaTime;
        playerAnim.SetFloat("moveX", 0);
        playerAnim.SetFloat("moveY", 0);
        lastMoveX = Input.GetAxisRaw("Horizontal");
        lastMoveY = Input.GetAxisRaw("Vertical");
        playerAnim.SetFloat("lastMoveX", lastMoveX);
        playerAnim.SetFloat("lastMoveY", lastMoveY);
    }

}
