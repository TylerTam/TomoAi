using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestDialogue_", menuName = "Data/TestDialogue", order = 0)]
public class TestDialogue:ScriptableObject
{
    public string[] testDialogue;
    public CharacterData TestCharacterData;
    public string GetRandomDialogue()
    {
        return testDialogue[Random.Range(0, testDialogue.Length)];
    }
}