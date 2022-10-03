using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Item item = other.GetComponent<Item>();

        if (item != null)
        {
            //Get Item Details
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            //check to see if item is can be picked  up;
            if(itemDetails.canBePickedUp == true)
            {
                InventoryManager.Instance.AddItem(InventoryLocation.player, item, other.gameObject);
            }
        }
    }
}
