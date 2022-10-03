using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    [SerializeField] private SceneName sceneNameGoTo = SceneName.HomeLand;
    [SerializeField] private Vector3 scenePositionGoTo = new Vector3();

    private void OnTriggerStay2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            //Calculate the player new position
            float xPos = Mathf.Approximately(scenePositionGoTo.x, 0f) ? player.transform.position.x : scenePositionGoTo.x;
            float yPos = Mathf.Approximately(scenePositionGoTo.y, 0f) ? player.transform.position.y : scenePositionGoTo.y;
            float zPos = 0f;

            //teleport to new pos in new scene
            SceneControllerManager.Instance.FadeAndLoadScene(sceneNameGoTo.ToString(), new Vector3(xPos, yPos, zPos));
        }
    }
}
