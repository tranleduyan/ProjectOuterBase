using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_NPCScheduleEventList", menuName = "Scriptable Objects/NPC/NPC Schedule Event List")]
public class SO_NPCScheduleEventList : ScriptableObject
{
    [SerializeField]
    public List<NPCScheduleEvent> npcScheduleEventList;
}
