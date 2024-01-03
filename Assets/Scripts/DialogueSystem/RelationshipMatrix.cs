using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

[CreateAssetMenu(fileName = "RelationshipPromptMatrix", menuName = "Data/Dialogue/RelationMatrix", order = 0)]
public class RelationshipMatrix : ScriptableObject
{
    public enum RelationShipType
    {
        Themself=0,
        Spouse=1,
        Sibling=2,
        ParentOf=3,
        ChildOf=4,
        Family=5,
        Divorced=6,
        Ex=7,
        SignificantOther=8,
        CloseFriend=9,
        Friend=10,
        Acquaintance=11,
        
        
        
    }
    public enum RelationshipStatus
    {
        Amazing,
        Great,
        Okay,
        NotGood,
        Terrible,

    }

    [SerializeField] private string char1Tag = "<Char1>";
    [SerializeField] private string char2Tag = "<Char2>";
    [SerializeField] private string NullRelationPrompt;
    [SerializeField] private SerializableDictionaryBase<RelationShipType, RelationShipTypeContainer> AllRelationshipPrompts;
    [SerializeField] private SerializableDictionaryBase<RelationshipStatus, Color> relationshipColor;
    [SerializeField] private SerializableDictionaryBase<RelationShipType, RelationShipTypeContainer> RelationBarText;
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

    public Color GetColor(RelationshipStatus stat)
    {
        return relationshipColor[stat];
    }
    public string GetBarText(RelationShipType relationType, RelationshipStatus stat)
    {
        string barText = RelationBarText[relationType].promptText[stat];
        if (string.IsNullOrWhiteSpace(barText))
        {
            return stat.ToString();
        }
        return barText;
    }
}
