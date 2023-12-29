using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestLoader : MonoBehaviour
{
    [SerializeField] private int numOfChars;
    [SerializeField] private GameObject tomoCharPrefab;
    [SerializeField] private MoveTargetInBounds tomoCharSpawnPos;
    private void Start()
    {
        LoadChars();
    }
    public virtual void LoadChars()
    {
        List<CharacterData> list = new List<CharacterData>(GameManager.Instance.SaveLoader.LoadedData.SavedCharacters);
        for (int i = 0; i < numOfChars; i++)
        {
            if(list.Count > 0)
            {
                int randomIndex = Random.Range(0, list.Count);
                CharacterData charData = list[randomIndex];
                TomoCharPerson tomoChar = ObjectPooler.NewObject(tomoCharPrefab, Vector3.zero, Quaternion.identity).GetComponent<TomoCharPerson>();
                tomoChar.ConstructTomoChar(charData);
                tomoChar.transform.position = tomoCharSpawnPos.GetRandomPos();
                list.RemoveAt(randomIndex);
            }
        }
    }
}
