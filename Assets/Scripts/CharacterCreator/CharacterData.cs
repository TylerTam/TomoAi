using System.Collections.Generic;
using System.Text;
using UnityEngine;



[System.Serializable]
public class CharacterData
{
    [SerializeField] public string Name;
    [SerializeField][TextArea] public string Description; //Personality and other characteristics
    [SerializeField][TextArea] public string Personality_Summary;
    [SerializeField][TextArea] public string AppearanceDescription;
    [SerializeField][TextArea] public string[] Examples_Of_Dialogue;
    [SerializeField] public CharacterAppearance CharacterAppearence;
    [SerializeField] public string Creator;
    [SerializeField] public float Creator_Id;
    [SerializeField] public List<CharacterRelationShip> serializedRelationships;
    [SerializeField] public int LocalId;
    private Dictionary<int, CharacterRelationShip> searchableRelationships;
    public CharacterData (string charName, string creator, float creatorId)
    {
        Name = charName;
        Creator = creator;
        Creator_Id = creatorId;
        Examples_Of_Dialogue = new string[0];
        CharacterAppearence = new CharacterAppearance();
        serializedRelationships = new List<CharacterRelationShip>();
    }

    public void UpdateForNewVersion()
    {
        if (serializedRelationships == null || serializedRelationships.Count == 0)
        {
            serializedRelationships = new List<CharacterRelationShip>();
            CharacterRelationShip selfRelation = new CharacterRelationShip();
            selfRelation.characterId = LocalId;
            selfRelation.relationshipType = RelationshipMatrix.RelationShipType.Themself;
            selfRelation.relationshipStatus = RelationshipMatrix.RelationshipStatus.Okay;
            serializedRelationships.Add(selfRelation);
        }
    }

    public void InitRuntimeData()
    {
        searchableRelationships = new Dictionary<int, CharacterRelationShip>();
        foreach(CharacterRelationShip relation in serializedRelationships)
        {
            searchableRelationships.Add(relation.characterId, relation);
        }
    }
    public CharacterData Clone()
    {
        CharacterData cloneChar = new CharacterData(Name, Creator, Creator_Id);

        cloneChar.Description = Description;
        cloneChar.Personality_Summary = Personality_Summary;
        cloneChar.AppearanceDescription = AppearanceDescription;
        cloneChar.Examples_Of_Dialogue = Examples_Of_Dialogue;
        cloneChar.CharacterAppearence = CharacterAppearence;

        return cloneChar;
    }

    public bool IsEqual(CharacterData compareData)
    {
        if (compareData.Name != Name) return false;
        if (compareData.Creator != Creator) return false;
        return compareData.Creator_Id == Creator_Id;
    }

    public string BuildPersona(List<string> relevantCharacterRelationships = null)
    {
        return CreateCharacterPrompt(relevantCharacterRelationships);
    }
    public string CreateCharacterPrompt(List<string> relevantCharacterRelationships = null)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(Name);
        sb.Append("'s Persona: ");
        sb.AppendLine(Description);
        sb.Append(Name);
        sb.Append("'s Appearence: ");
        sb.AppendLine(AppearanceDescription);
        sb.Append("Personality: ");
        sb.AppendLine(Personality_Summary);

        if (relevantCharacterRelationships != null && relevantCharacterRelationships.Count > 0)
        {
            sb.Append(Name + "'s Relationships: ");
            foreach (string relName in relevantCharacterRelationships)
            {
                sb.Append(BuildRelationshipPrompt(relName) + " ");
            }
            sb.Append("\n");
        }
        sb.Append("<START>\n");

        foreach (string str in Examples_Of_Dialogue)
        {
            sb.Append(Name + ": ");
            sb.Append(str + "\n");
            //sb.AppendLine();
        }
        return sb.ToString();

    }

    public string BuildRelationshipPrompt(string characterName)
    {
        List<int> ids = GameManager.Instance.SaveLoader.GetCharacterIdsByName(characterName);
        if(ids == null || ids.Count == 0)
        {
            return "";
        }
        List<CharacterRelationShip> relationshipIndexes = new List<CharacterRelationShip>();
        foreach(int id in ids)
        {
            if (searchableRelationships.ContainsKey(id))
            {
                relationshipIndexes.Add(searchableRelationships[id]);
            }
        }
        if(relationshipIndexes.Count == 0)
        {
            return CharacterRelationShip.GetNoRelationshipPrompt(Name, characterName);
        }
        else
        {
            return relationshipIndexes[Random.Range(0, relationshipIndexes.Count)].GetRelationShipPrompt(Name);
        }
    }
}


[System.Serializable]
public class CharacterAppearance
{
    [SerializeField] public SlotWithPieces HeadSlot;
    [SerializeField] public SlotWithPieces MouthSlot;
    [SerializeField] public EyesSlot Eyes;

    public CharacterAppearance()
    {
        HeadSlot = new SlotWithPieces();
        MouthSlot = new SlotWithPieces();
        Eyes = new EyesSlot();
    }


    [System.Serializable]
    public class AppearenceSlot
    {
        [SerializeField] public Vector2 slotPosition;
        [SerializeField] public Vector2 slotScale;
        [SerializeField] public float BaseEyeZRot;
    }

    [System.Serializable]
    public class SlotWithPieces : AppearenceSlot
    {
        public SlotWithPieces()
        {
            AppearancePieces = new AppearancePieces[0];
        }
        [SerializeField] public AppearancePieces[] AppearancePieces;
    }

    [System.Serializable]
    public class EyesSlot : AppearenceSlot
    {
        public EyesSlot()
        {
            LeftEye = new EyePieces();
            RightEye = new EyePieces();
        }
        [SerializeField] public EyePieces LeftEye;
        [SerializeField] public EyePieces RightEye;
    }

    [System.Serializable]
    public class EyePieces
    {
        public EyePieces()
        {
            AttachedEyePieces = new AppearancePieces[0];
        }
        [SerializeField] public string BaseEyeId;
        [SerializeField] public AppearancePieces[] AttachedEyePieces;
    }
    [System.Serializable]
    public class AppearancePieces
    {
        [SerializeField] public string ItemId;
        [SerializeField] public Vector3 PiecePos;
        [SerializeField] public float PieceZRot;
        [SerializeField] public Vector2 PieceScale;

    }
}

[System.Serializable]
public class CharacterRelationShip
{
    [SerializeField] public string characterName;
    [SerializeField] public int characterId;
    [SerializeField] public RelationshipMatrix.RelationShipType relationshipType;
    [SerializeField] public RelationshipMatrix.RelationshipStatus relationshipStatus;

    /// <summary>
    /// Used to create the relationship prompt for the character <br/>
    /// The character who you are asking is the parameter. <br/>
    /// IE. If you asked "Hey, Bob, what is your relation with Ted?", Bob would be the parameter here
    /// </summary>
    /// <returns></returns>
    public string GetRelationShipPrompt(string char1Name)
    {
        CharacterData otherPerson = GameManager.Instance.SaveLoader.GetCharacterByID(characterId);
        string prompt = GameManager.Instance.RelationshipMatrix.GetRelation(relationshipType, relationshipStatus, char1Name, otherPerson.Name);

        return prompt;
        
    }

    public static string GetNoRelationshipPrompt(string char1Name, string char2Name)
    {
        string prompt = GameManager.Instance.RelationshipMatrix.GetNullRelation(char1Name, char2Name);
        return prompt;
    }
}




