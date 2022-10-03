using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCPath))]
public class NPCSchedule : MonoBehaviour
{
    [SerializeField] private SO_NPCScheduleEventList so_NPCScheduleEventList = null;
    private SortedSet<NPCScheduleEvent> npcScheduleEventSet;
    private NPCPath npcPath;


    private void Awake()
    {
        //Load npc schedule event list into a sorted set
        npcScheduleEventSet = new SortedSet<NPCScheduleEvent>(new NPCScheduleEventSort());

        foreach (NPCScheduleEvent npcScheduleEvent in so_NPCScheduleEventList.npcScheduleEventList)
        {
            npcScheduleEventSet.Add(npcScheduleEvent);
        }

        //get npc path component
        npcPath = GetComponent<NPCPath>();
    }

    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += GameTimeSystem_AdvanceMinute;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= GameTimeSystem_AdvanceMinute;
    }

    private void GameTimeSystem_AdvanceMinute(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        int time = (gameHour * 100) + gameMinute;

        //Attemp to get matching schedule
        NPCScheduleEvent matchingNPCScheduleEvent = null;

        foreach(NPCScheduleEvent npcScheduleEvent in npcScheduleEventSet)
        {
            if(npcScheduleEvent.Time == time)
            {
                //Time match now check if parameter match
                if (npcScheduleEvent.day != 0 && npcScheduleEvent.day != gameDay)
                    continue;
                if (npcScheduleEvent.season != Season.none && npcScheduleEvent.season != gameSeason)
                    continue;
                if (npcScheduleEvent.weather != Weather.none && npcScheduleEvent.weather != GameManager.Instance.currentWeather)
                    continue;

                //Schedule matches
                matchingNPCScheduleEvent = npcScheduleEvent;
                break;
            }

            else if(npcScheduleEvent.Time > time)
            {
                break;
            }
        }
        //now test is matching schedule != null and do something
        if(matchingNPCScheduleEvent != null)
        {
            //build path for matching schedule
            npcPath.BuildPath(matchingNPCScheduleEvent);
        }
    }
    
    public SO_NPCScheduleEventList GetScheduleEventList()
    {
        return so_NPCScheduleEventList;

    }
}
