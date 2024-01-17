using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGrid : MonoBehaviour
{
    [SerializeField] private LayerMask wallDetectionLayer;
    [SerializeField] private Vector3Int gridSize;
    [SerializeField] private Vector3 offset;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private ApartmentLoader apartmentLoader;
#if UNITY_EDITOR
    [SerializeField] private bool debugGrid;
    [SerializeField] private Color gridColor = Color.white;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Transform testGridPoint;
#endif

    private WallTile[,] allWallTiles;
    private Vector2Int currentCellGrid;
    private Vector2Int prevCellGrid;

    private void Awake()
    {
        allWallTiles = new WallTile[gridSize.x, gridSize.z];
    }

    public void InitWallTiles(List<int> wallTileLayout)
    {
        bool load = true;
        if(wallTileLayout == null || wallTileLayout.Count == 0)
        {
            GetComponent<ApartmentLoader>().SaveApartmentConfig();
            load = false;
        }

        int wallIndex = 0;
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.z; y++)
            {
                if (allWallTiles[x,y] != null)
                {
                    ObjectPooler.ReturnToPool(allWallTiles[x, y].gameObject);
                    allWallTiles[x, y] = null;
                }
                if (load)
                {
                    if (wallTileLayout[wallIndex] == 1)
                    {
                        ToggleWallTile(new Vector2Int(x, y));
                    }
                }
                wallIndex++;
            }
        }
    }

    public void DoWallBuilding()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000, wallDetectionLayer))
        {
            currentCellGrid = GetGridIndex(hit.point);
            if (prevCellGrid != currentCellGrid)
            {
                prevCellGrid = currentCellGrid;
                ToggleWallTile(hit.point);
                apartmentLoader.SetLoaderDirty();
            }
        }
    }
    public void ToggleWallTile(Vector3 position)
    {
        ToggleWallTile(GetGridIndex(position));
    }
    private void ToggleWallTile(Vector2Int gridPos)
    {
        if (allWallTiles[gridPos.x, gridPos.y] == null)
        {
            WallTile pref = ObjectPooler.NewObject(wallPrefab).GetComponent<WallTile>();
            allWallTiles[gridPos.x, gridPos.y] = pref;
            pref.transform.position = GetCellPos(gridPos);

        }
        else
        {
            ObjectPooler.ReturnToPool(allWallTiles[gridPos.x, gridPos.y].gameObject);
            allWallTiles[gridPos.x, gridPos.y] = null;
        }
    }
    public Vector2Int GetGridIndex(Vector3 worldPosition)
    {
        worldPosition = new Vector3(Mathf.Floor(worldPosition.x), 0, Mathf.Floor(worldPosition.z));
        worldPosition = worldPosition + gridSize / 2;
        return new Vector2Int((int)worldPosition.x, (int)worldPosition.z);

    }
    public Vector3 GetCellPos(Vector2Int cellIndex)
    {
        return new Vector3(cellIndex.x - gridSize.x / 2 + offset.x, 0, cellIndex.y - gridSize.z / 2 + offset.z);
    }


    public List<int> GetWallSetup()
    {
        List<int> wallTiles = new List<int>();
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.z; y++)
            {
                if (allWallTiles[x, y] != null)
                {
                    wallTiles.Add(1);
                }
                else
                {
                    wallTiles.Add(0);
                }
            }
        }
        return wallTiles;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!debugGrid) return;
        Gizmos.color = gridColor;
        Vector2Int selectedCell = Vector2Int.zero;
        if (testGridPoint) selectedCell = GetGridIndex(testGridPoint.position);
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.z; y++)
            {

                if (x == selectedCell.x && y == selectedCell.y) Gizmos.color = selectedColor;
                else Gizmos.color = gridColor;
                Gizmos.DrawCube(new Vector3(x, 0, y) - (gridSize/2) + offset, new Vector3(.5f,.5f,.5f));
            }
        }
    }
#endif
}
