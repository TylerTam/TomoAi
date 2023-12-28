using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    private Dictionary<string, GameObject> loadedCharacters = new Dictionary<string, GameObject>();
    public GameObject BaseCharacterPrefab;

    public GameObject LoadNewCharacter(CharacterData charData)
    {
        GameObject newChar = ObjectPooler.NewObject(BaseCharacterPrefab, Vector2.zero, Quaternion.identity);
        newChar.GetComponent<TomoCharPerson>().ConstructTomoChar(charData);
        return newChar;
    }

    public void ClearLoadedChars()
    {
        Queue<GameObject> loadedChars = new Queue<GameObject>();
        foreach(KeyValuePair<string, GameObject> key in loadedCharacters)
        {
            loadedChars.Enqueue(key.Value);
        }
        loadedCharacters.Clear();

        while(loadedChars.Count > 0)
        {
            ObjectPooler.ReturnToPool(loadedChars.Dequeue());
        }
    }

}
