using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventorySlot : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Camera mainCamera;
    private Canvas parentCanvas;

    private Transform parentItem;
    private GridCursor gridCursor;
    private Cursor cursor;
    public GameObject draggedItem;

    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [HideInInspector] public bool isSelected = false;

    [SerializeField] private UIInventoryBar inventoryBar = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;
    [HideInInspector] public ItemDetails itemDetails;
    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public int itemQuantity;

    [SerializeField] private int slotNumber = 0;
    
    void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
        EventHandler.RemoveSelectedItemFromInventoryEvent += RemoveSelectedItemFromInventory;
        EventHandler.DropSelectedItemEvent += DropSelectedItemAtMousePosition;
    }

    void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
        EventHandler.RemoveSelectedItemFromInventoryEvent -= RemoveSelectedItemFromInventory;
        EventHandler.DropSelectedItemEvent -= DropSelectedItemAtMousePosition;
    }

    public void SceneLoaded()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        gridCursor = FindObjectOfType<GridCursor>();
        cursor = FindObjectOfType<Cursor>(); 
    }

    private void ClearCursors()
    {
        //Disable cursor
        gridCursor.DisableCursor();
        cursor.DisableCursor();

        //Set item type to none
        gridCursor.SelectedItemType = ItemType.none;
        cursor.SelectedItemType = ItemType.none;
    }

    private void RemoveSelectedItemFromInventory()
    {
        if(itemDetails != null && isSelected)
        {
            int itemCode = itemDetails.itemCode;

            //remove item from players inventory
            InventoryManager.Instance.RemoveItem(InventoryLocation.player, itemCode);

            //if no more of item then clear selected
            if(InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, itemCode) == -1)
            {
                ClearSelectedItem();
            }
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(itemDetails != null)
        {
            Player.Instance.DisablePlayerInputAndResetMovement();

            //Instantiate gameObject as Draggedf item
            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);

            //Get the image for dragged item
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;

            SetSelectedItem();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(draggedItem != null)
        {
            Destroy(draggedItem);

            //if drag ends over inventory bar, get item drag is over and swap them
            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>() != null)
            {
                //Get the slot number where the drag ended
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>().slotNumber;

                //Swap inventoryItem in inventory list
                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);

                DestroyInventoryTextBox();

                ClearSelectedItem();
            }

            //else attempt to drop the item if it can be dropped
            else
            {
                if (itemDetails.canBeDropped)
                {
                    DropSelectedItemAtMousePosition();
                }
            }

            //Enable player input
            Player.Instance.EnablePlayerMovement();
        }
    }

    //Drops the item (if selectected) at the current mouse position. Called by the Drop Item Event
    void DropSelectedItemAtMousePosition()
    {
        if(itemDetails != null && isSelected)
        {

            //if a valid cursor position
            if (gridCursor.CursorPositionIsValid)
            {
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1*mainCamera.transform.position.z));
                //Create Item from prefab at mouse position
                GameObject itemGameObject = Instantiate(itemPrefab, new Vector3(worldPosition.x, worldPosition.y/* use this if pivot of items is at the bottom worldPosition.y - Settings.gridCellSize/2f*/, worldPosition.z), Quaternion.identity, parentItem);
                Item item = itemGameObject.GetComponent<Item>();
                item.ItemCode = itemDetails.itemCode;

                //remove the item from player inventory
                InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);

                //if no more of item then clear selected
                if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, item.ItemCode) == -1)
                {
                    ClearSelectedItem();
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //populate text box with item details
        if(itemQuantity != 0)
        {
            //instantiate inventory text box
            inventoryBar.inventoryTextBoxGameObject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryBar.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();

            //Set itemtype descripytion
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            //populate text box
            inventoryTextBox.SetTextboxText(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            //Set text box position according to inventory bar position
            if (inventoryBar.IsInventoryBarPositionBottom)
            {
                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 24f, transform.position.z);
            }

            else
            {
                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f,1f);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y - 24f, transform.position.z);
            }
        }
    } 

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyInventoryTextBox();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if left click
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            //if(inventory slot currently selected then deselected)
            if (isSelected == true)
            {
                ClearSelectedItem();
            }
            else
            {
                if(itemQuantity > 0)
                {
                    SetSelectedItem();
                }
            }
        }
    }

    public void ClearSelectedItem()
    {

        ClearCursors();

        //clear currently highlighted items
        inventoryBar.ClearHighlightOnInventorySlots();

        isSelected = false;

        //set no item selected in inventory
        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);

        //clear carrying item
        Player.Instance.ClearCarriedItem();
    }

    //set this inventory slots item to be selected
    void SetSelectedItem()
    {
        //Clear currently highlighted items
        inventoryBar.ClearHighlightOnInventorySlots();

        //Highlightitem on inventory bar
        isSelected = true;

        //setHighlighted inventory slots
        inventoryBar.SetHighlightedInventorySlots();


        //Set use radius for cursors
        gridCursor.ItemUseGridRadius = itemDetails.itemUseGridRadius;
        cursor.ItemUseRadius = itemDetails.itemUseRadius;

        //if item requires a grid cursor then enable cursor
        if(itemDetails.itemUseGridRadius > 0)
        {
            gridCursor.EnableCursor();
        }
        else
        {
            gridCursor.DisableCursor();
        }

        //if item requires a cursor then enable cursor
        if (itemDetails.itemUseRadius > 0)
        {
            cursor.EnableCursor();
        }
        else
        {
            cursor.DisableCursor();
        }

        //set item type
        gridCursor.SelectedItemType = itemDetails.itemType;

        //set item selected in inventory
        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, itemDetails.itemCode);

        //show carry item if the item is carryable
        if(itemDetails.canBeCarried == true)
        {
            //show the carry item
            Player.Instance.ShowCarriedItem(itemDetails.itemCode);
        }
        else
        {
            //show nothing;
            Player.Instance.ClearCarriedItem();
        }
    }

    public void DestroyInventoryTextBox()
    {
        if(inventoryBar.inventoryTextBoxGameObject != null)
        {
            Destroy(inventoryBar.inventoryTextBoxGameObject);
        }
    }
}
