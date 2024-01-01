using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestingCharData", menuName = "Data/CharData", order=0)]
public class TestingCharacterData : ScriptableObject
{
    public CharacterData characterData;
    public string Name => characterData.Name;
    public string BuildPersona()
    {
        return characterData.BuildPersona();
    }
}



