using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to a crop prefab to set the values in the grid property dictionary
/// </summary>

public class CropInstantiator : MonoBehaviour
{
    private Grid grid;
    [SerializeField] private int daysSinceDug = -1;
    [SerializeField] private int daysSinceWatered = -1;
    [ItemCodeDescription]
    [SerializeField] private int seedItemCode = 0;
    [SerializeField] private int growthDays = 0;

    private void OnEnable()
    {
        EventHandler.InstantiateCropPrefabsEvent += InstantiateCropPrefabs;
    }

    private void OnDisable()
    {
        EventHandler.InstantiateCropPrefabsEvent -= InstantiateCropPrefabs;
    }

    private void InstantiateCropPrefabs()
    {
        //Get grid gameObject
        grid = GameObject.FindObjectOfType<Grid>();

        //Get grid position for crop
        Vector3Int cropGridPosition = grid.WorldToCell(transform.position);

        //Set Crop Grid Properties
        SetCropGridProperties(cropGridPosition);

        //Destroy this gameObject
        Destroy(gameObject);
    }

    private void SetCropGridProperties(Vector3Int cropGridPosition)
    {
        if(seedItemCode > 0)
        {
            GridPropertyDetails gridPropertyDetails;

            gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

            if(gridPropertyDetails == null)
            {
                gridPropertyDetails = new GridPropertyDetails();
            }

            gridPropertyDetails.daysSinceDug = daysSinceDug;
            gridPropertyDetails.daysSinceWatered = daysSinceWatered;
            gridPropertyDetails.seedItemCode = seedItemCode;
            gridPropertyDetails.growthDays = growthDays;

            GridPropertiesManager.Instance.SetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y, gridPropertyDetails);
        }
    }
}
