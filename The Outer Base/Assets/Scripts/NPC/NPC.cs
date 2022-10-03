using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(NPCMovement))]
[RequireComponent(typeof(GenerateGUID))]
public class NPC : MonoBehaviour, ISaveable
{
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;

    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    private NPCMovement npcMovement;

    private void OnEnable()
    {
        ISaveableRegister();
    }

    private void OnDisable()
    {
        ISaveableDeregister();
    }

    private void Awake()
    {
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
        
    }

    private void Start()
    {
        //Get NPC Movement component
        npcMovement = GetComponent<NPCMovement>();
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        //Get gameobject save
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            //Get sceneSave
            if(GameObjectSave.sceneData.TryGetValue(Settings.PreLoadScene, out SceneSave sceneSave))
            {
                //if dictionaries are not null
                if(sceneSave.vector3Dictionary != null && sceneSave.stringDictionary != null)
                {
                    //target grid position
                    if(sceneSave.vector3Dictionary.TryGetValue("npcTargetGridPosition", out Vector3Serializable savedNPCTargetGridPosition))
                    {
                        npcMovement.npcTargetGridPosition = new Vector3Int((int)savedNPCTargetGridPosition.x, (int)savedNPCTargetGridPosition.y, (int)savedNPCTargetGridPosition.z);
                        npcMovement.npcCurrentGridPosition = npcMovement.npcTargetGridPosition;
                    }

                    //target world position
                    if(sceneSave.vector3Dictionary.TryGetValue("npcTargetWorldPosition", out Vector3Serializable savedNPCTargetWorldPosition))
                    {
                        npcMovement.npcTargetWorldPosition = new Vector3(savedNPCTargetWorldPosition.x, savedNPCTargetWorldPosition.y, savedNPCTargetWorldPosition.z);
                        transform.position = npcMovement.npcTargetWorldPosition;
                    }

                    //target scene
                    if (sceneSave.stringDictionary.TryGetValue("npcTargetScene", out string savedTargetScene))
                    {
                        if (Enum.TryParse<SceneName>(savedTargetScene, out SceneName sceneName))
                        {
                            npcMovement.npcTargetScene = sceneName;
                            npcMovement.npcCurrentScene = npcMovement.npcTargetScene;
                        }
                    }
                    // Current Animation
                    if (sceneSave.stringDictionary.TryGetValue("currentPlayingAnimation", out string savedPlayingAnimation))
                    {
                        npcMovement.npcTargetAnimationClip = NPCManager.Instance.GetAnimationClip(savedPlayingAnimation);
                        Debug.Log(savedPlayingAnimation);

                    }
                    if (npcMovement.npcTargetAnimationClip == null)
                    {
                        // Clear any current NPC movement
                        npcMovement.CancelNPCMovement();
                    }
                    else
                    {
                        // Start the animation if there is an animation present
                        npcMovement.SetNPCEventAnimation();
                    }
                }
            }
        }
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        //nothing require here since on preload scene
    }

    public GameObjectSave ISaveableSave()
    {
        //Remove current scene save
        GameObjectSave.sceneData.Remove(Settings.PreLoadScene);

        //Create scenesave
        SceneSave sceneSave = new SceneSave();

        //Create vector3 serializable dictionary
        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>();

        //Create string dictionary
        sceneSave.stringDictionary = new Dictionary<string, string>();

        //store target grid position, target world position
        sceneSave.vector3Dictionary.Add("npcTargetGridPosition", new Vector3Serializable(npcMovement.npcTargetGridPosition.x, npcMovement.npcTargetGridPosition.y, npcMovement.npcTargetGridPosition.z));
        sceneSave.vector3Dictionary.Add("npcTargetWorldPosition", new Vector3Serializable(npcMovement.npcTargetWorldPosition.x, npcMovement.npcTargetWorldPosition.y, npcMovement.npcTargetWorldPosition.z));
        sceneSave.stringDictionary.Add("npcTargetScene", npcMovement.npcTargetScene.ToString());
        sceneSave.stringDictionary.Add("currentPlayingAnimation", npcMovement.npcTargetAnimationClip.name);
        //AddScenesave to gameobject
        GameObjectSave.sceneData.Add(Settings.PreLoadScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableStoreScene(string sceneName)
    {
        //nothing here since in preload scene
    }
}
