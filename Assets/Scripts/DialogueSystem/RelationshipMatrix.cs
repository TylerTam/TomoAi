using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

[CreateAssetMenu(fileName = "RelationshipPromptMatrix", menuName = "Data/Dialogue/RelationMatrix", order = 0)]
public class RelationshipMatrix : ScriptableObject
{
    public enum RelationShipType
    {
        Acquaintance,
        Friend,
        CloseFriend,
        SignificantOther,
        Spouse,
        Ex,
        Divorced,
        ChildOf,
        ParentOf,
        Sibling,
        Family,
        Themself
    }
    public enum RelationshipStatus
    {
        Terrible,
        NotGood,
        Okay,
        Great,
        Amazing

    }

    [SerializeField] private string char1Tag = "<Char1>";
    [SerializeField] private string char2Tag = "<Char2>";
    [SerializeField] private string NullRelationPrompt;
    [SerializeField] private SerializableDictionaryBase<RelationShipType, RelationShipTypeContainer> AllRelationshipPrompts;
    [System.Serializable]
    public class RelationShipTypeContainer
    {
        [SerializeField] public SerializableDictionaryBase<RelationshipStatus, string> promptText;
    }
    public string GetRelation(RelationShipType relationType, RelationshipStatus relationStatus, string char1Name, string char2Name)
    {
        string prompt = "";
        try
        {
            prompt = AllRelationshipPrompts[relationType].promptText[relationStatus];
        }
        catch
        {
            Debug.LogError("Did not find prompt type for relation: " + relationType + " | " + relationStatus);
            return "Oh Boy...";
        }


        prompt = prompt.Replace(char1Tag, char1Name);
        prompt = prompt.Replace(char2Tag, char2Name);
        return prompt;
    }

    public string GetNullRelation(string char1Name, string char2Name)
    {
        string prompt = NullRelationPrompt;
        prompt = prompt.Replace(char1Tag, char1Name);
        prompt = prompt.Replace(char2Tag, char2Name);
        return prompt;
    }
}
