using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class SceneSave 
{
    public Dictionary<string, bool> boolDictionary; //string key is an identifier name we choose for this list
    public Dictionary<string, string> stringDictionary;
    public Dictionary<string, Vector3Serializable> vector3Dictionary;
    public List<SceneItem> listSceneItem;
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
    public List<InventoryItem>[] listInventoryItemArray;
    public Dictionary<string, int[]> intArrayDictionary; // to store capacity of item in inventory
    public Dictionary<string, int> intDictionary;
}
