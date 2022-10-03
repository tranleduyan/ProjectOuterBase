using UnityEngine;

[System.Serializable]
public class CropDetails
{

    [ItemCodeDescription]
    // this is the item code for the corresponding seed
    public int seedItemCode;
    // days growth for each stage
    public int[] growthDays;
    // prefab to use when instantiating growth stages
    public GameObject[] growthPrefab;
    // growth sprite
    public Sprite[] growthSprite;
    //growth seasons
    public Season[] seasons;
    //sprite used once harvested
    public Sprite harvestedSprite;

    [ItemCodeDescription]
    // if the item transform into another item when harvested this code will be populate
    public int harvestedTransformItemCode;
    //if the crop should be disabled before the harvested animation
    public bool hideCropBeforeHarvestedAnimation;
    // if colliders on crop should be disabled to avoid the harvested animation effecting any other game objects
    public bool disableCropCollidersBeforeHarvestedAnimation;
    // true if harvested animation to be played on final growth stage prefab
    public bool isHarvestedAnimation;
    // flag to determine whether there is a harvest action event
    public bool isHarvestActionEffect = false;
    public bool spawnCropProducedAtPlayerPosition;
    //the harvest action effect for the crop
    public HarvestActionEffect harvestActionEffect;

    [ItemCodeDescription]
    //array of item codes for the tool that can harvest or 0 array elemts if no tool required
    public int[] harvestToolItemCode;
    //number of harvest actions required for corresponding tool in harvest tool item code array
    public int[] requiredHarvestActions;

    [ItemCodeDescription]
    //array of item codes produced for the harvested crop
    public int[] cropProducedItemCode;
    //array of minimum quantities produced for the harvested crop
    public int[] cropProducedMinQuantity;
    //if max quantity is > min quantity then a random number of crops between min and max are produced
    public int[] cropProducedMaxQuantity;
    //days to regrow nex crop or -1 if a single crop
    public int daysToRegrow;

    public bool isConnectedCrop;

    //returns true if the tool item code can be used to harvest this crop, else return false
    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        if(RequiredHarvestActionsForTool(toolItemCode) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //returns -1 if the tool cant be used to harvest this crop, else returns the number of harvest actions required by this tool
    public int RequiredHarvestActionsForTool(int toolItemCode)
    {
        for(int i =0; i < harvestToolItemCode.Length; i++)
        {
            if(harvestToolItemCode[i] == toolItemCode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }
}
