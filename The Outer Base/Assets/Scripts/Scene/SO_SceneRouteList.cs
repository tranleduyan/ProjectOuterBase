using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_SceneRouteList", menuName = "Scriptable Objects/Scene/Scene Route List")]
public class SO_SceneRouteList : ScriptableObject
{
    public List<SceneRoute> sceneRouteList;
}
