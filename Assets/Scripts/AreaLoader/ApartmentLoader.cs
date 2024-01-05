using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentLoader : MonoBehaviour
{
    private bool isLoading = false;
    private CharacterData loadedCharacterApartment;
    [SerializeField] private ApartmentUiManager apartmentUiManager;

    [Header("Random Spawn Loc")]
    [SerializeField] private GameObject tomoCharPrefab;
    [SerializeField] private Transform tomoCharParent;
    private List<TomoCharPerson> allSpawnedTomoChars = new List<TomoCharPerson>();


    [SerializeField] private Vector3 spawnBounds;
    [SerializeField] private Vector3 spawnBoundOffset;



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
    private void Awake()
    {
        floorPropBlock = new MaterialPropertyBlock();
        floorRenderer.GetPropertyBlock(floorPropBlock);
    }


    public void LoadApartment(CharacterData charData)
    {
        if (isLoading) return;
        if (loadedCharacterApartment != null && loadedCharacterApartment.LocalId == charData.LocalId) return;
        StartCoroutine(Load(charData));

    }

    private IEnumerator Load(CharacterData charData)
    {
        isLoading = true;
        loadedCharacterApartment = charData;
        floorPropBlock.SetColor("_Color", charData.FavouriteColor);
        floorRenderer.SetPropertyBlock(floorPropBlock);

        for (int i = 0; i < allSpawnedTomoChars.Count; i++)
        {
            ObjectPooler.ReturnToPool(allSpawnedTomoChars[i].gameObject);
        }
        allSpawnedTomoChars.Clear();

        apartmentResident = charData;

        NpcAreaData npcArea = GameTick.Instance.NpcPlacements.GetNpcArea(charData.LocalId);
        if (npcArea.area == NpcPlacements.AreaType.Apartments)
        {
            
            if (npcArea.ApartmentOwnerId == charData.LocalId)
            {
                isAiHome = true;
                TomoCharPerson ownerTomoChar = ObjectPooler.NewObject(tomoCharPrefab, GetRandomSpawnPos(), Quaternion.Euler(0, Random.Range(0, 360), 0)).GetComponent<TomoCharPerson>();
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
        TomoCharPerson ownerTomoChar = ObjectPooler.NewObject(tomoCharPrefab, GetRandomSpawnPos(), Quaternion.Euler(0, Random.Range(0, 360), 0)).GetComponent<TomoCharPerson>();
        ownerTomoChar.ConstructTomoChar(apartmentResident);
        ownerTomoChar.transform.parent = tomoCharParent;
        allSpawnedTomoChars.Add(ownerTomoChar);
        yield return new WaitForSeconds(1);
        enterApartment.Invoke();
    }
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
