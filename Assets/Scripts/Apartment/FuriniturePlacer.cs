using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuriniturePlacer : MonoBehaviour
{
    [SerializeField] private LayerMask placeableLayer;
    [SerializeField] private FurnitureData furnitureToPlace;
    [SerializeField] private ApartmentLoader apartmentLoader;
    [SerializeField] private BuildingMode buildingMode;
    [SerializeField] private WallGrid wallGrid;

    private enum BuildingMode
    {
        None,
        Object,
        Wall
    }
    private void Update()
    {
        switch (buildingMode)
        {
            case BuildingMode.Object:
                if (Input.GetMouseButtonDown(0))
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 10000, placeableLayer))
                    {
                        apartmentLoader.AddFurniture(furnitureToPlace, hit.point, Random.Range(0, 360));
                        apartmentLoader.SetLoaderDirty();
                        buildingMode = BuildingMode.None;
                    }
                }
                break;
            case BuildingMode.Wall:
                if (Input.GetMouseButton(0))
                {
                    wallGrid.DoWallBuilding();
                }
                break;

        }
    }
}
