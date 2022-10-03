using UnityEngine;
using Cinemachine;

public class SwitchConfineBoundingShape : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SwitchBoundingShape;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SwitchBoundingShape;
    }

    //Switch the collider that cinemachine uses to define the edges of the screen
    void SwitchBoundingShape()
    {
        //get the polygon collider on the 'boundsconfiner' gameObject which is used by cinemachine to prevent the camera going beyong the screen edges
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();

        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();

        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;

        //since the confiner bounds have change, need to call this to clear cache;
        cinemachineConfiner.InvalidatePathCache();
    }
}
