using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ItemCodeDescriptionAttribute))]
public class ItemCodeDescriptionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //Change the returned property height to be double to cater for the additional item code description that we will draw 
        return EditorGUI.GetPropertyHeight(property) * 2;
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Using BeginProperty / EndProperty on the parent property means that prefab override logic works on the entire property
        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            EditorGUI.BeginChangeCheck(); // start check for changed values

            //Draw Item Code
            var newValue = EditorGUI.IntField(new Rect(position.x, position.y, position.width, position.height / 2), label, property.intValue);

            //Draw Item Description/name
            EditorGUI.LabelField(new Rect(position.x, position.y + position.height / 2, position.width, position.height / 2), "Item Description", GetItemDescription(property.intValue));


            //If item code value has changed then set value to new value
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
        }

        EditorGUI.EndProperty();
    }

    private string GetItemDescription(int itemCode)
    {
        SO_ItemList so_itemList;
        so_itemList = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Object Assets/Item/so_ItemList.asset", typeof(SO_ItemList)) as SO_ItemList;

        List<ItemDetails> itemDetailsList = so_itemList.itemDetails;
        ItemDetails itemDetails = itemDetailsList.Find(x => x.itemCode == itemCode);

        if(itemDetails != null)
        {
            return itemDetails.itemDescription;
        }
        else
        {
            return "";
        }

    }
}
