using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInventoryBar : MonoBehaviour
{

    [SerializeField] private Sprite blank16x16sprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlot = null;
    [SerializeField] private Image gameClock = null;
    [SerializeField] private TextMeshProUGUI timeText = null;
    [SerializeField] private TextMeshProUGUI dateText = null;
    [SerializeField] private TextMeshProUGUI seasonText = null;
    [SerializeField] private TextMeshProUGUI yearText = null;
    public GameObject inventoryBarDraggedItem;
    public GameObject inventoryTextBoxGameObject;

    private RectTransform rectTransform;
    private Image image;

    private bool _isInventoryBarPositionBottom = true;

    public bool IsInventoryBarPositionBottom
    {
        get=> _isInventoryBarPositionBottom;
        set => _isInventoryBarPositionBottom = value;
        
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>(); 
    }

    void Update()
    {
        SwitchInventoryBarPosition();
    }

    public void ClearHighlightOnInventorySlots()
    {
        if (inventorySlot.Length > 0)
        {
            //loop through inventory slots and clear highlight sprites
            for(int i = 0; i < inventorySlot.Length; i++)
            {
                if (inventorySlot[i].isSelected)
                {
                    inventorySlot[i].isSelected = false;
                    inventorySlot[i].inventorySlotHighlight.color = new Color(0f, 0f, 0f, 0f);

                    //update inventory to show item as not selected
                    InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
                }
            }
        }
    }

    //set the selected highlight if set on all inventory item position
    public void SetHighlightedInventorySlots()
    {
        if (inventorySlot.Length > 0)
        {
            //loop through inventory sots and clear highlight sprites
            for (int i = 0; i < inventorySlot.Length; i++)
            {
                SetHighlightedInventorySlots(i);
            }
        }
    }

    //set the selected highlight if set on an inventory item for a given slot item position
    public void SetHighlightedInventorySlots(int itemPosition)
    {
        if(inventorySlot.Length > 0 && inventorySlot[itemPosition].itemDetails != null)
        {
            if (inventorySlot[itemPosition].isSelected)
            {
                inventorySlot[itemPosition].inventorySlotHighlight.color = new Color(1f, 1f, 1f, 1f);

                //Update inventory to show item as selected
                InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, inventorySlot[itemPosition].itemDetails.itemCode);
            }
        }
    }

    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewportPosition = Player.Instance.GetPlayerViewportPosition();

        if(playerViewportPosition.y > 0.3f && IsInventoryBarPositionBottom == false)
        {
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 5f);

            IsInventoryBarPositionBottom = true;

            gameClock.enabled = true;
            timeText.enabled = true;
            dateText.enabled = true;
            seasonText.enabled = true;
            yearText.enabled = true;
        }
        else if(playerViewportPosition.y <= 0.3f && IsInventoryBarPositionBottom == true)
        {
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -5f);

            IsInventoryBarPositionBottom = false;
            gameClock.enabled = false;
            timeText.enabled = false;
            dateText.enabled = false;
            seasonText.enabled = false;
            yearText.enabled = false;
        }
    }

    public void HideInventoryBar()
    {
        image.enabled = false;
        if (inventorySlot.Length > 0)
        {
            //loop through iunventory slots and hide the items;
            for (int i = 0; i < inventorySlot.Length; i++)
            {
                inventorySlot[i].inventorySlotImage.enabled = false;
                inventorySlot[i].textMeshProUGUI.enabled = false;
            }
        }
    }

    public void ShowInventoryBar()
    {
        image.enabled = true;
        if (inventorySlot.Length > 0)
        {
            //loop through iunventory slots and hide the items;
            for (int i = 0; i < inventorySlot.Length; i++)
            {
                inventorySlot[i].inventorySlotImage.enabled = true;
                inventorySlot[i].textMeshProUGUI.enabled = true;
            }
        }
    }

    private void ClearInventorySlots()
    {
        if(inventorySlot.Length > 0)
        {
            //loop through iunventory slots and update with blankj sprite;
            for(int i = 0; i < inventorySlot.Length; i++)
            {
                inventorySlot[i].inventorySlotImage.sprite = blank16x16sprite;
                inventorySlot[i].textMeshProUGUI.text = "";
                inventorySlot[i].itemDetails = null;
                inventorySlot[i].itemQuantity = 0;
                SetHighlightedInventorySlots(i);
            }
        }
    }

    private void InventoryUpdated(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if(inventoryLocation == InventoryLocation.player)
        {
            ClearInventorySlots();

            if(inventorySlot.Length > 0 && inventoryList.Count > 0)
            {
                //loop through inventory slots and update with coresponding inventory list item
                for(int i = 0; i < inventorySlot.Length; i++)
                {
                    if (i < inventoryList.Count)
                    {
                        int itemCode = inventoryList[i].itemCode;
                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                        if(itemDetails != null)
                        {
                            //add images and details to inventory item slot
                            inventorySlot[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                            inventorySlot[i].textMeshProUGUI.text = inventoryList[i].itemQuantity.ToString();
                            inventorySlot[i].itemDetails = itemDetails;
                            inventorySlot[i].itemQuantity = inventoryList[i].itemQuantity;
                            SetHighlightedInventorySlots(i);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += InventoryUpdated;
    }
    
    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= InventoryUpdated;
    }

    public void DestroyCurrentlyDraggedItems()
    {
        for(int i = 0; i < inventorySlot.Length; i++)
        {
            if(inventorySlot[i].draggedItem != null)
            {
                Destroy(inventorySlot[i].draggedItem);
            }
        }
    }

    public void ClearCurrentlySelectedItems()
    {
        for(int i = 0; i < inventorySlot.Length; i++)
        {
            inventorySlot[i].ClearSelectedItem();
        }
    }
}
