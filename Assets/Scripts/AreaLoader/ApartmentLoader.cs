using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentLoader : MonoBehaviour
{
    private bool isLoading = false;
    private CharacterData loadedCharacterApartment;

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

        TomoCharPerson ownerTomoChar = ObjectPooler.NewObject(tomoCharPrefab, GetRandomSpawnPos(), Quaternion.Euler(0, Random.Range(0, 360), 0)).GetComponent<TomoCharPerson>();
        ownerTomoChar.ConstructTomoChar(charData);
        ownerTomoChar.transform.parent = tomoCharParent;


        allSpawnedTomoChars.Add(ownerTomoChar);
        yield return null;
        isLoading = false;
        DialogueSystem_Main.Instance.scenarioPromptManager.LoadPromptByIndex(0, 2);
    }

    private Vector3 GetRandomSpawnPos()
    {
        return transform.position + new Vector3(Random.Range(-spawnBounds.x, spawnBounds.x), Random.Range(-spawnBounds.y, spawnBounds.y), Random.Range(-spawnBounds.z, spawnBounds.z)) + spawnBoundOffset;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (debugBounds)
        {
            Gizmos.color = debugBoundsColor;
            Gizmos.DrawCube(transform.position + spawnBoundOffset, spawnBounds*2);
        }
    }
#endif
}
