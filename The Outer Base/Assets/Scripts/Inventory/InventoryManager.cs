using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>, ISaveable
{
    private UIInventoryBar inventoryBar;

    private Dictionary<int, ItemDetails> itemDetailsDictionary;

    private int[] selectedInventoryItem; // the index ofd the array is the inventory list and the value is the item code

    public List<InventoryItem>[] inventoryLists;
    [HideInInspector] public int[] inventoryListCapacityIntArray; // the index of the array is the inventory list (from the InventoryLocation enum), and the value is the capacity of that inventory list

    [SerializeField]
    private SO_ItemList itemList = null;

    private string _iSaveableUniqueID;

    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        CreateInventoryLists();
        CreateItemDetailsDictionary();

        //Initialize selected inventory item array
        selectedInventoryItem = new int[(int)InventoryLocation.count];
        for(int i = 0; i > selectedInventoryItem.Length; i++)
        {
            selectedInventoryItem[i] -= 1;
        }

        //Get unique Id for gameObject and create save data object
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;

        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISaveableRegister();
    }

    private void OnDisable()
    {
        ISaveableDeregister();
    }

    private void Start()
    {
        inventoryBar = FindObjectOfType<UIInventoryBar>();
    }
    private void CreateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];
        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }

        //initialize inventory list capacity array
        inventoryListCapacityIntArray = new int[(int)InventoryLocation.count];

        //initialize player inventory list capacity
        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
    }

    //Populate the itemDetailsDictionary from the scriptable object item list
    private void CreateItemDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        foreach (ItemDetails itemDetails in itemList.itemDetails)
        {
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }

    //overload method of AddItem to the inventory  list for the inventory location and then destroy the ganeobject that we pick up;
    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject gameObjectToDelete)
    {
        if(InventoryIsFull(inventoryLocation, item) == false)
        {
            AddItem(inventoryLocation, item);
            Destroy(gameObjectToDelete);
        }
    }

    //Add an item to the inventory list for the inventory location
    public void AddItem(InventoryLocation inventoryLocation, Item item)
    {
        int itemCode = item.ItemCode;
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        //Check if inventory already contains the item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);
        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }

        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    //Swap item at fromItem index with item at toItem index in InventoryLocaiton inventory list

    public void SwapInventoryItems(InventoryLocation inventoryLocation, int fromItem, int toItem)
    {

        //if fromItem index and toItem index are within the bound of the list, not the same, and greater or equal to zero
        if (fromItem < inventoryLists[(int)inventoryLocation].Count && toItem < inventoryLists[(int)inventoryLocation].Count && fromItem != toItem && fromItem >= 0 && toItem >=0)
        {
            InventoryItem fromInventoryItem = inventoryLists[(int)inventoryLocation][fromItem];
            InventoryItem toInventoryItem = inventoryLists[(int)inventoryLocation][toItem];

            inventoryLists[(int)inventoryLocation][toItem] = fromInventoryItem;
            inventoryLists[(int)inventoryLocation][fromItem] = toInventoryItem;

            EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);

        }
    }

    //find if an itemCode is already in the inventory. returns the item position in the inventory list, or -1 if the item is not in the inventory;
    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];
        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }

        return -1;
    }

    public bool InventoryIsFull(InventoryLocation inventoryLocation, Item item)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];
        int itemCode = item.ItemCode;
        int currentCapacity = 0;
        for(int i = 0; i < inventoryList.Count; i++)
        {
            currentCapacity++;
        }
        if (currentCapacity <= inventoryListCapacityIntArray[(int)inventoryLocation] && FindItemInInventory(inventoryLocation, itemCode) != -1 ||
            currentCapacity < inventoryListCapacityIntArray[(int)inventoryLocation])
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void AddItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        //Check if inventory already contains the item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);
        
        if(itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }

        //Sed event that inventory has been updated
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    //add Item to the end of the inventory
    void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;
        inventoryList.Add(inventoryItem);


        //DebugPrintInventoryList(inventoryList);
    }

    //overload method for add item to position in the inventory
    void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity + 1;
        inventoryItem.itemQuantity = quantity;
        inventoryItem.itemCode = itemCode;
        inventoryList[position] = inventoryItem;

        //DebugPrintInventoryList(inventoryList);
    }

    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;
        if(itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }
    
    //returns the itemDetails (from the SO_ItemList) for the currently selected item in the inventory location, or null if an item isn't selected
    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);

        if(itemCode == -1)
        {
            return null;
        }
        else
        {
            return GetItemDetails(itemCode);
        }
    }

    //Get the selected item for inventoryLocation - returns itemCode or -1 if nothing is selected
    private int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInventoryItem[(int)inventoryLocation];
    }

    public string GetItemTypeDescription (ItemType itemType)
    {
        string itemTypeDescription;
        switch (itemType)
        {
            case ItemType.Breaking_tool:
                itemTypeDescription = Settings.BreakingTool;
                break;
            case ItemType.Chopping_tool:
                itemTypeDescription = Settings.ChoppingTool;
                break;
            case ItemType.Hoeing_tool:
                itemTypeDescription = Settings.HoeingTool;
                break;
            case ItemType.Reaping_tool:
                itemTypeDescription = Settings.ReapingTool;
                break;
            case ItemType.Watering_tool:
                itemTypeDescription = Settings.WateringTool;
                break;
            case ItemType.Collecting_tool:
                itemTypeDescription = Settings.CollectingTool;
                break;
            default:
                itemTypeDescription = itemType.ToString();
                break;
        }
        return itemTypeDescription;
    }

    //Remove an Item from the inventory, and create a game object at the position it was dropped
    public void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        //Check if inventory already contains the item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if(itemPosition != -1)
        {
            RemoveItemAtPosition(inventoryList, itemCode, itemPosition);

            //Send event that inventory has been updated
            EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
        }
    }
    
    private void RemoveItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();
        int quantity = inventoryList[position].itemQuantity - 1;
        if(quantity > 0)
        {
            inventoryItem.itemQuantity = quantity;
            inventoryItem.itemCode = itemCode;
            inventoryList[position] = inventoryItem;
        }

        else
        {
            inventoryList.RemoveAt(position);
        }
    }

    public void ClearSelectedInventoryItem (InventoryLocation inventoryLocation)
    {
        selectedInventoryItem[(int)inventoryLocation] = -1;
    }

    //Set the selected inventory item for Inventory Location to itemCode
    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        selectedInventoryItem[(int)inventoryLocation] = itemCode;
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
        //Create new scene Save
        SceneSave sceneSave = new SceneSave();

        //Remove any existing scene save for preload scene for this gameobject
        GameObjectSave.sceneData.Remove(Settings.PreLoadScene);

        //Add inventory lists array to preload scene save
        sceneSave.listInventoryItemArray = inventoryLists;

        //add inventoryList capacity array to preload scene save
        sceneSave.intArrayDictionary = new Dictionary<string, int[]>();
        sceneSave.intArrayDictionary.Add("inventoryListCapactiyArray", inventoryListCapacityIntArray);

        //Add scenesave for gameobject
        GameObjectSave.sceneData.Add(Settings.PreLoadScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            //need to find inventory lists - start by trying to locate saveScene for gameObject
            if(gameObjectSave.sceneData.TryGetValue(Settings.PreLoadScene, out SceneSave sceneSave))
            {
                //list inventory items array exists for preload scene
                if(sceneSave.listInventoryItemArray != null)
                {
                    inventoryLists = sceneSave.listInventoryItemArray;

                    //Send events that inventory has been updated
                    for(int i = 0; i < (int) InventoryLocation.count; i++)
                    {
                        EventHandler.CallInventoryUpdatedEvent((InventoryLocation)i, inventoryLists[i]);
                    }

                    //clear any items player was carrying
                    Player.Instance.ClearCarriedItem();

                    //Clear any highlights on inventory bar
                    inventoryBar.ClearHighlightOnInventorySlots();
                }

                //int array dictionary exists for scene
                if (sceneSave.intArrayDictionary != null && sceneSave.intArrayDictionary.TryGetValue("inventoryListCapacityArra", out int[] inventoryCapacityArray))
                {
                    inventoryListCapacityIntArray = inventoryCapacityArray;
                }
            }
        }

    }

    public void ISaveableStoreScene(string sceneName)
    {
        //nothing to do here sinece the inventory manager is on a preload scene
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        //nothing to do here sinece the inventory manager is on a preload scene
    }
}
