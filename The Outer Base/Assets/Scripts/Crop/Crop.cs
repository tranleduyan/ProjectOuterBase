using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{

    private int harvestActionCount = 0;

    [Tooltip("This should be populated from child transform gameobject showing harvest effect spawn point")]
    [SerializeField] private Transform harvestActionEffectTransform = null;

    [Tooltip("This should be populated from child gameobject - Crop Harvested Sprite")]
    [SerializeField] private SpriteRenderer cropHarvestedSpriteRenderer = null;

    [HideInInspector]
    public Vector2Int cropGridPosition;


    public void ProcessToolAction(ItemDetails equippedItemDetails, bool isToolRight, bool isToolLeft, bool isToolDown, bool isToolUp)
    {
        //Get grid property details
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

        if (gridPropertyDetails == null)
        {
            return;
        }

        //get seed item details
        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if (seedItemDetails == null)
        {
            return;
        }

        //GetCrop details
        CropDetails cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if (cropDetails == null)
        {
            return;
        }

        //Get animator for crop if present
        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            if (isToolRight || isToolUp)
            {
                animator.SetTrigger("useToolRight");
            }
            else if (isToolLeft || isToolDown)
            {
                animator.SetTrigger("useToolLeft");
            }
        }

        //Trigger tool particle effect on Crop
        if (cropDetails.isHarvestActionEffect)
        {
            EventHandler.CallHarvestActionEffectEvent(harvestActionEffectTransform.position, cropDetails.harvestActionEffect);
        }

        //Get required harvest actions for tool
        int requiredHarvestActions = cropDetails.RequiredHarvestActionsForTool(equippedItemDetails.itemCode);
        if(requiredHarvestActions == -1)
        {
            return; // this tool can't be used to harvest this crop
        }

        //increment harvest action count
        harvestActionCount += 1;

        //check if rquired harvest actions made
        if (harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(isToolRight, isToolUp, cropDetails, gridPropertyDetails, animator, seedItemDetails);
        }
    }

    private void HarvestCrop(bool isUsingToolRight, bool isUsingToolUp, CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator, ItemDetails itemDetails)
    {

        //if there is a harvested animation
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            //if harvest sprite is not null then add to sprite renderer
            if (cropDetails.harvestedSprite != null)
            {
                if (cropHarvestedSpriteRenderer != null)
                {
                    cropHarvestedSpriteRenderer.sprite = cropDetails.harvestedSprite;
                }
            }

            if (isUsingToolRight || isUsingToolUp)
            {
                animator.SetTrigger("harvestRight");
            }
            else
            {
                animator.SetTrigger("harvestLeft");
            }
        }
        //Delete crop from grid properties
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        //if the item is need space x type then reset the two adjacents grids properties as well
        if (itemDetails.needSpaceX && itemDetails.needSpaceY == false)
        {
            GridPropertyDetails rightAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            GridPropertyDetails leftAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            if(rightAdjacentGridPropertyDetails != null && leftAdjacentGridPropertyDetails != null)
            {
                rightAdjacentGridPropertyDetails.seedItemCode = -1;
                rightAdjacentGridPropertyDetails.growthDays = -1;
                rightAdjacentGridPropertyDetails.daysSinceLastHarvest = -1;
                rightAdjacentGridPropertyDetails.daysSinceWatered = -1;

                leftAdjacentGridPropertyDetails.seedItemCode = -1;
                leftAdjacentGridPropertyDetails.growthDays = -1;
                leftAdjacentGridPropertyDetails.daysSinceLastHarvest = -1;
                leftAdjacentGridPropertyDetails.daysSinceWatered = -1;
            }
        }
        //if the item is need space y type then reset the up adjacent grid properties as well
        else if (itemDetails.needSpaceX == false && itemDetails.needSpaceY)
        {
            GridPropertyDetails upAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            if (upAdjacentGridPropertyDetails != null)
            {
                upAdjacentGridPropertyDetails.seedItemCode = -1;
                upAdjacentGridPropertyDetails.growthDays = -1;
                upAdjacentGridPropertyDetails.daysSinceLastHarvest = -1;
                upAdjacentGridPropertyDetails.daysSinceWatered = -1;
            }
        }
        //if the item is need space y and x type then reset the up adjacent grid properties as well
        else if (itemDetails.needSpaceX && itemDetails.needSpaceY)
        {
            GridPropertyDetails rightAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            GridPropertyDetails leftAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            GridPropertyDetails upAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            GridPropertyDetails upRightAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY + 1);
            GridPropertyDetails upLeftAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY + 1);
            if (rightAdjacentGridPropertyDetails != null && leftAdjacentGridPropertyDetails != null && upAdjacentGridPropertyDetails != null && upRightAdjacentGridPropertyDetails != null && upLeftAdjacentGridPropertyDetails != null)
            {
                rightAdjacentGridPropertyDetails.seedItemCode = -1;
                rightAdjacentGridPropertyDetails.growthDays = -1;
                rightAdjacentGridPropertyDetails.daysSinceLastHarvest = -1;
                rightAdjacentGridPropertyDetails.daysSinceWatered = -1;

                leftAdjacentGridPropertyDetails.seedItemCode = -1;
                leftAdjacentGridPropertyDetails.growthDays = -1;
                leftAdjacentGridPropertyDetails.daysSinceLastHarvest = -1;
                leftAdjacentGridPropertyDetails.daysSinceWatered = -1;

                upAdjacentGridPropertyDetails.seedItemCode = -1;
                upAdjacentGridPropertyDetails.growthDays = -1;
                upAdjacentGridPropertyDetails.daysSinceLastHarvest = -1;
                upAdjacentGridPropertyDetails.daysSinceWatered = -1;

                upRightAdjacentGridPropertyDetails.seedItemCode = -1;
                upRightAdjacentGridPropertyDetails.growthDays = -1;
                upRightAdjacentGridPropertyDetails.daysSinceLastHarvest = -1;
                upRightAdjacentGridPropertyDetails.daysSinceWatered = -1;

                upLeftAdjacentGridPropertyDetails.seedItemCode = -1;
                upLeftAdjacentGridPropertyDetails.growthDays = -1;
                upLeftAdjacentGridPropertyDetails.daysSinceLastHarvest = -1;
                upLeftAdjacentGridPropertyDetails.daysSinceWatered = -1;
            }
        }

        //should the crop be hidden before the harvested animation
        if (cropDetails.hideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        //check to see Should box colliders be disabled before harvest or not
        if (cropDetails.disableCropCollidersBeforeHarvestedAnimation)
        {
            //disable any box colliders
            Collider2D[] collider2Ds = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider2D in collider2Ds)
            {
                collider2D.enabled = false;
            }
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        //is there a harvested animation - destroy this crop gameObject after animation completed
        if(cropDetails.isHarvestedAnimation && animator != null)
        {
            StartCoroutine(ProcessHarvestActionsAfterAnimation(cropDetails, gridPropertyDetails, animator));
        }
        else
        {
            HarvestActions(cropDetails, gridPropertyDetails);
        }
    }


    private IEnumerator ProcessHarvestActionsAfterAnimation(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            UIManager.Instance.gridCursor.DisableCursor();
            yield return null;
        }

        HarvestActions(cropDetails, gridPropertyDetails);
    }

    private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItems(cropDetails);

        if(cropDetails.harvestedTransformItemCode > 0)
        {
            CreateHarvestedTransformCrop(cropDetails, gridPropertyDetails);
        }
        Destroy(gameObject);
        GridPropertyDetails rightAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        GridPropertyDetails leftAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        GridPropertyDetails upAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        GridPropertyDetails downAdjacentGridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        
        //updated all the current crops in scene
        GridPropertiesManager.Instance.CallClearDisplayGridPropertyDetails();
        GridPropertiesManager.Instance.CallDisplayGridPropertyDetails();
        UIManager.Instance.gridCursor.EnableCursor();
    }

    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        //Spawn the item(s) to be produced
        for (int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            int cropsToProduce;

            // calculate how many crops to produce
            if (cropDetails.cropProducedMinQuantity[i] == cropDetails.cropProducedMaxQuantity[i] ||
                cropDetails.cropProducedMaxQuantity[i] < cropDetails.cropProducedMinQuantity[i])
            {
                cropsToProduce = cropDetails.cropProducedMinQuantity[i];
            }
            else
            {
                cropsToProduce = Random.Range(cropDetails.cropProducedMinQuantity[i], cropDetails.cropProducedMaxQuantity[i] + 1);
            }

            for(int j = 0; j < cropsToProduce; j++)
            {
                Vector3 spawnPosition;
                if (cropDetails.spawnCropProducedAtPlayerPosition)
                {
                    // add item to the players inventory
                    InventoryManager.Instance.AddItem(InventoryLocation.player, cropDetails.cropProducedItemCode[i]);
                }
                else
                {
                    //random position
                    spawnPosition = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f), 0f);
                    SceneItemsManager.Instance.InstantiateSceneItem(cropDetails.cropProducedItemCode[i], spawnPosition); 
                }
            }
        }
    }

    private void CreateHarvestedTransformCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        //Update crop in grid properties
        gridPropertyDetails.seedItemCode = cropDetails.harvestedTransformItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        //display planted crop
        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }
}
