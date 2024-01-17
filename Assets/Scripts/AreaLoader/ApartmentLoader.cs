using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentLoader : MonoBehaviour
{
    private bool isLoading = false;
    private CharacterData loadedCharacterApartment;
    [SerializeField] private ApartmentUiManager apartmentUiManager;
    [SerializeField] private WallGrid wallSetup;

    [Header("Random Spawn Loc")]
    [SerializeField] private GameObject tomoCharPrefab;
    [SerializeField] private Transform tomoCharParent;
    private List<TomoCharPerson> allSpawnedTomoChars = new List<TomoCharPerson>();


    [SerializeField] private Vector3 spawnBounds;
    [SerializeField] private Vector3 spawnBoundOffset;
    [SerializeField] private GameObject poofGameObject;
    [SerializeField] private float poofSpawnY;
    [SerializeField] private bool loadFirstCharacter;
    



#if UNITY_EDITOR
    [SerializeField] private bool debugBounds;
    [SerializeField] private Color debugBoundsColor;
    
#endif
    [Header("ApartmentConfig")]
    [SerializeField] private Renderer floorRenderer;
    [SerializeField] private Renderer wallRenderer;

    private MaterialPropertyBlock floorPropBlock;
    private MaterialPropertyBlock wallPropBlock;

    private bool isAiHome = true;
    private CharacterData apartmentResident;

    private bool loaderIsDirty = false;
    private void Awake()
    {
        floorPropBlock = new MaterialPropertyBlock();
        floorRenderer.GetPropertyBlock(floorPropBlock);
    }
    private void Start()
    {
        if (loadFirstCharacter)
        {
            if (GameManager.Instance.SaveLoader.LoadedData.SavedCharacters.Count > 0)
            {
                LoadApartment(GameManager.Instance.SaveLoader.LoadedData.SavedCharacters[0]);
            }
        }
    }

    public void LoadApartment(CharacterData charData)
    {
        if (loaderIsDirty)
        {
            SaveApartmentConfig();
            loaderIsDirty = false;
        }
        if (isLoading) return;
        if (loadedCharacterApartment != null && loadedCharacterApartment.LocalId == charData.LocalId) return;
        StartCoroutine(Load(charData));

    }
    public void SetLoaderDirty()
    {
        loaderIsDirty = true;
    }

    private IEnumerator Load(CharacterData charData)
    {
        isLoading = true;
        loadedCharacterApartment = charData;

        #region Unload existing apartment
        for (int i = 0; i < allSpawnedTomoChars.Count; i++)
        {
            ObjectPooler.ReturnToPool(allSpawnedTomoChars[i].gameObject);
        }
        allSpawnedTomoChars.Clear();

        #endregion

        #region Load selected Apartment

        apartmentResident = charData;

        InitConfig(GameManager.Instance.SaveLoader.GetApartmentConfig(charData.LocalId));
        floorPropBlock.SetColor("_Color", charData.FavouriteColor);
        floorRenderer.SetPropertyBlock(floorPropBlock);

        NpcAreaData npcArea = GameTick.Instance.NpcPlacements.GetNpcArea(charData.LocalId);
        if (npcArea.area == NpcPlacements.AreaType.Apartments)
        {
            
            if (npcArea.ApartmentOwnerId == charData.LocalId)
            {
                isAiHome = true;
                Vector3 pos = GetRandomSpawnPos();
                pos.y = 0;
                TomoCharPerson ownerTomoChar = ObjectPooler.NewObject(tomoCharPrefab, pos, Quaternion.Euler(0, Random.Range(0, 360), 0)).GetComponent<TomoCharPerson>();
                ownerTomoChar.ConstructTomoChar(charData);
                ownerTomoChar.transform.parent = tomoCharParent;
                allSpawnedTomoChars.Add(ownerTomoChar);
                apartmentUiManager.SetText(charData.Name + " is at their apartment", "Enter Apartment");
            }
            else
            {
                apartmentUiManager.SetText(charData.Name + " is at their <Char2>'s apartment", "Summon");
                isAiHome = false;
            }
        }
        else
        {
            apartmentUiManager.SetText(charData.Name + GetDescriptionString(npcArea.area), "Summon");
            isAiHome = false;
        }
        #endregion

        yield return null;
        isLoading = false;
        DialogueSystem_Main.Instance.scenarioPromptManager.LoadPromptByIndex(0, 2);
    }

    private string GetDescriptionString(NpcPlacements.AreaType area)
    {
        switch (area)
        {
            case NpcPlacements.AreaType.Apartments:
                return " is in the apartments";

            case NpcPlacements.AreaType.Restaurant:
                return " is at the restaurant";
            default:
                return " cannot be found...";
        }
    }
    private Vector3 GetRandomSpawnPos()
    {
        return transform.position + new Vector3(Random.Range(-spawnBounds.x, spawnBounds.x), Random.Range(-spawnBounds.y, spawnBounds.y), Random.Range(-spawnBounds.z, spawnBounds.z)) + spawnBoundOffset;
    }

    public void CheckAiHome(System.Action enterApartment)
    {
        if (isAiHome)
        {
            enterApartment.Invoke();
            return;
        }
        else
        {
            StartCoroutine(SummonNpc(enterApartment));
        }
    }

    private IEnumerator SummonNpc(System.Action enterApartment)
    {
        Vector3 randomSpawn = GetRandomSpawnPos();
        randomSpawn.y = poofSpawnY;
        poofGameObject.SetActive(false);
        poofGameObject.transform.position = randomSpawn;
        poofGameObject.SetActive(true);
        TomoCharPerson ownerTomoChar = ObjectPooler.NewObject(tomoCharPrefab, randomSpawn, Quaternion.Euler(0, Random.Range(0, 360), 0)).GetComponent<TomoCharPerson>();
        ownerTomoChar.ConstructTomoChar(apartmentResident);
        ownerTomoChar.transform.parent = tomoCharParent;
        allSpawnedTomoChars.Add(ownerTomoChar);
        yield return new WaitForSeconds(1);
        enterApartment.Invoke();
    }


    #region Apartment configuration
    private List<Furniture> placedFurniture = new List<Furniture>();
    private int currentLoadedId = 0;

    public void InitConfig(ApartmentConfig config)
    {
        currentLoadedId = config.apartmentOwnerLocalId;
        for (int i = placedFurniture.Count - 1; i >= 0; i--)
        {
            ObjectPooler.ReturnToPool(placedFurniture[i].gameObject);
        }
        placedFurniture.Clear();

        foreach (FurnitureConfig furn in config.furnitureConfigs)
        {
            AddFurniture((GameManager.Instance.AllItems.GetItem(furn.FurnitureName, ItemData.ItemType.Furniture) as FurnitureData), furn.FurniturePos, furn.FurinitureYRot);
        }
        wallSetup.InitWallTiles(config.wallSetup);
    }

    public void AddFurniture(FurnitureData furnitureData, Vector3 pos, float yRot = 0)
    {
        Furniture newFurniture = ObjectPooler.NewObject(furnitureData.furniturePrefab).GetComponent<Furniture>();
        newFurniture.InitFuriniture(pos, yRot, furnitureData);
        placedFurniture.Add(newFurniture);
    }

    public void SaveApartmentConfig()
    {
        ApartmentConfig newConfig = new ApartmentConfig();
        newConfig.apartmentOwnerLocalId = currentLoadedId;
        foreach (Furniture furn in placedFurniture)
        {
            newConfig.furnitureConfigs.Add(furn.GetConfig());
        }
        newConfig.wallSetup = wallSetup.GetWallSetup();
        GameManager.Instance.SaveLoader.UpdateApartmentConfig(currentLoadedId, newConfig);
    }
    #endregion
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (debugBounds)
        {
            Gizmos.color = debugBoundsColor;
            Gizmos.DrawCube(transform.position + spawnBoundOffset, spawnBounds * 2);
        }
    }

#endif
}

[System.Serializable]
public class ApartmentConfig
{
    [SerializeField] public int apartmentOwnerLocalId;
    [SerializeField] public List<FurnitureConfig> furnitureConfigs = new List<FurnitureConfig>();
    [SerializeField] public List<int> wallSetup = new List<int>();
}
[System.Serializable]
public struct FurnitureConfig
{
    [SerializeField] public string FurnitureName;
    [SerializeField] public Vector3 FurniturePos;
    [SerializeField] public float FurinitureYRot;
}
