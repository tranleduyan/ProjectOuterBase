using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AStar))]
public class NPCManager : Singleton<NPCManager>
{
    [SerializeField] private SO_SceneRouteList so_SceneRouteList = null;
    private Dictionary<string, SceneRoute> sceneRouteDictionary;

    [HideInInspector]
    public NPC[] npcArray;
    private AStar aStar;

    private Dictionary<string, AnimationClip> animationClipDictionary;

    protected override void Awake()
    {
        base.Awake();

        //Create sceneRoute dictionary
        animationClipDictionary = new Dictionary<string, AnimationClip>();
        sceneRouteDictionary = new Dictionary<string, SceneRoute>();

        if (so_SceneRouteList.sceneRouteList.Count > 0)
        {
            foreach(SceneRoute so_sceneRoute in so_SceneRouteList.sceneRouteList)
            {
                //Check for duplicate routes in dictionary
                if(sceneRouteDictionary.ContainsKey(so_sceneRoute.fromSceneName.ToString() + so_sceneRoute.toSceneName.ToString()))
                {
                    Debug.Log("Duplicate scene route key found, check for duplicates routes in the scriptable object scene route list");
                    continue;
                }

                //Add route to dicationary
                sceneRouteDictionary.Add(so_sceneRoute.fromSceneName.ToString() + so_sceneRoute.toSceneName.ToString(), so_sceneRoute);
            }
        }

        aStar = GetComponent<AStar>();

        //Get NPC gameobjects in scene
        npcArray = FindObjectsOfType<NPC>();
        
        foreach (NPC npc in npcArray)
        {

            List<NPCScheduleEvent> npcScheduleEventList = npc.GetComponentInParent<NPCSchedule>().GetScheduleEventList().npcScheduleEventList;
            if (npcScheduleEventList != null)
            {
                Debug.Log(npcScheduleEventList[0].animationAtDestination.name);

                for (int i = 0; i < npcScheduleEventList.Count - 1; i++)
                 {
                     if (npcScheduleEventList[i].animationAtDestination != null)
                     {

                         if (!animationClipDictionary.ContainsKey(npcScheduleEventList[i].animationAtDestination.name))
                         {
                             animationClipDictionary.Add(npcScheduleEventList[i].animationAtDestination.name, npcScheduleEventList[i].animationAtDestination);
                         }
                     }
                 }
            }
        }
    }

    public AnimationClip GetAnimationClip(string animationClipName)
    {
        if(animationClipDictionary.TryGetValue(animationClipName, out AnimationClip animationClip))
        {
            return animationClip;
        }
        return null;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    private void AfterSceneLoad()
    {
        SetNPCsActiveStatus();
    }

    private void SetNPCsActiveStatus()
    {
        foreach(NPC npc in npcArray)
        {
            NPCMovement npcMovement = npc.GetComponent<NPCMovement>();

            if(npcMovement.npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
            {
                npcMovement.SetNPCActiveInScene();
            }
            else
            {
                npcMovement.SetNPCInactiveInScene();
            }
        }
    }

    public SceneRoute GetSceneRoute(string fromSceneName, string toSceneName)
    {
        SceneRoute sceneRoute;
        //Get sceneRoute from dictionary
        if(sceneRouteDictionary.TryGetValue(fromSceneName+toSceneName, out sceneRoute))
        {
            return sceneRoute;
        }
        else
        {
            return null;
        }
    }

    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        if(aStar.BuildPath(sceneName, startGridPosition, endGridPosition, npcMovementStepStack))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
