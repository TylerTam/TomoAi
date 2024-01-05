using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;



[System.Serializable]
public class CharacterData: IComparable
{
    [SerializeField] public string Name;
    [SerializeField][TextArea] public string Description; //Personality and other characteristics
    [SerializeField][TextArea] public string Personality_Summary;
    [SerializeField][TextArea] public string AppearanceDescription;
    [SerializeField][TextArea] public string ElevatorDescription;//Used for when they are brought up in other character dialogues
    [SerializeField] public Color FavouriteColor;
    [SerializeField] public double Intensity = 0.7f;
    [SerializeField][TextArea] public string[] Examples_Of_Dialogue;
    [SerializeField] public CharacterAppearance CharacterAppearence;
    [SerializeField] public string Creator;
    [SerializeField] public float Creator_Id;
    [SerializeField] public List<CharacterRelationShip> serializedRelationships;
    [SerializeField] public int LocalId;
    private Dictionary<int, CharacterRelationShip> searchableRelationships;

    public Mood currentMood = Mood.Neutral;
    public enum Mood
    {
        Neutral,
        Happy,
        Excited,
        Sad,
        Depressed,
        Mad,
        Furious
    }
    public double GetIntensity
    {
        get
        {
            double ret = Intensity;
            switch (currentMood)
            {
                case Mood.Happy:
                case Mood.Mad:
                    ret += 0.1;
                    break;
                case Mood.Excited:
                case Mood.Furious:
                    ret += 0.2;
                    break;
                case Mood.Sad:
                    ret -= 0.1;
                    break;
                case Mood.Depressed:
                    ret -= 0.2;
                    break;

                default:
                case Mood.Neutral:
                    break;
            }
            return ret;
        }
    }
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
            CharacterRelationShip selfRelation = new CharacterRelationShip(Name, LocalId, RelationshipMatrix.RelationShipType.Themself);
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

    public string BuildPersona(List<int> charIdRelationships = null)
    {
        return CreateCharacterPrompt(charIdRelationships);
    }
    public string CreateCharacterPrompt(List<int> charIdRelationships = null)
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


        sb.Append("<START>\n");

        foreach (string str in Examples_Of_Dialogue)
        {
            sb.Append(Name + ": ");
            sb.Append(str + "\n");
            //sb.AppendLine();
        }
        sb.AppendLine("<START>");
        if (charIdRelationships != null && charIdRelationships.Count > 0)
        {
            sb.Append(Name + "'s Relationships: ");
            foreach (int relId in charIdRelationships)
            {
                sb.Append(BuildRelationshipPrompt(relId) + " " /*+ CharacterData.GetQuickCharDescription(relId) + " "*/);
            }
            
            sb.AppendLine("<START>");
        }
        return sb.ToString();

    }

    public string BuildRelationshipPrompt(int charId)
    {

        CharacterData charData = GameManager.Instance.SaveLoader.GetCharacterByID(charId);
        if (charData == null) return "";
        if (searchableRelationships.ContainsKey(charId))
        {
            return searchableRelationships[charId].GetRelationShipPrompt(Name);
        }
        return CharacterRelationShip.GetNoRelationshipPrompt(Name, charData.Name);
    }
    public void DeleteRelationship(int charId)
    {
        if (searchableRelationships.ContainsKey(charId))
        {
            serializedRelationships.Remove(searchableRelationships[charId]);
            searchableRelationships.Remove(charId);
        }
    }

    public bool Knows(int charId)
    {
        return searchableRelationships.ContainsKey(charId);
    }
    public List<CharacterRelationShip> SortRelationships()
    {
        List<CharacterRelationShip> sortedList = new List<CharacterRelationShip>(serializedRelationships);
        sortedList.Sort();
        return sortedList;
    }

    public static string GetQuickCharDescription(int charId)
    {
        return GameManager.Instance.SaveLoader.GetCharacterByID(charId).ElevatorDescription;
    }

    public void UpdateRelationship(CharacterData charData, RelationshipMatrix.RelationShipType relType, RelationshipMatrix.RelationshipStatus relStatus)
    {
        CharacterRelationShip relationshipShip = null;
        try
        {
            relationshipShip = serializedRelationships.Find(x => x.characterId == charData.LocalId);
            relationshipShip.UpdateRelatioship(relType, relStatus);
        }
        catch
        {
            relationshipShip = new CharacterRelationShip(charData.Name, charData.LocalId, relType, relStatus);
            serializedRelationships.Add(relationshipShip);
            searchableRelationships.Add(charData.LocalId, relationshipShip);
        }
        
    }

    public void AdjustRelationshipLevel(CharacterData charData, int incrementAmount)
    {
        CharacterRelationShip relationshipShip = null;
        try
        {
            relationshipShip = serializedRelationships.Find(x => x.characterId == charData.LocalId);
        }
        catch
        {
            relationshipShip = new CharacterRelationShip(charData.Name, charData.LocalId, RelationshipMatrix.RelationShipType.Acquaintance, RelationshipMatrix.RelationshipStatus.Okay);
        }

        relationshipShip.AdjustRelationshipLevel(incrementAmount);
    }

    public int CompareTo(object obj)
    {
        return LocalId.CompareTo(((CharacterData)obj).LocalId);
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
public class CharacterRelationShip: System.IComparable
{
    [SerializeField] public string characterName;
    [SerializeField] public int characterId;
    [SerializeField] public RelationshipMatrix.RelationShipType relationshipType;
    [SerializeField] public RelationshipMatrix.RelationshipStatus relationshipStatus;

    [SerializeField] private float relationShipLevel;

    public CharacterRelationShip (string charName, int charId, RelationshipMatrix.RelationShipType relType, RelationshipMatrix.RelationshipStatus relStat = RelationshipMatrix.RelationshipStatus.Okay)
    {
        characterName = charName;
        characterId = charId;
        relationshipType = relType;
        relationshipStatus = relStat;
        relationShipLevel = 100 - ((float)((int)relStat) * 20) - 1;

    }

    public void UpdateRelatioship(RelationshipMatrix.RelationShipType relType, RelationshipMatrix.RelationshipStatus relStatus)
    {
        relationshipType = relType;
        relationshipStatus = relStatus;
    }

    public void AdjustRelationshipLevel(int adjustmentLevel)
    {
        relationShipLevel = Mathf.Clamp(relationShipLevel + adjustmentLevel, 0, 100);
        relationShipLevel = Mathf.Round(relationShipLevel * 1000) / 1000;
        int relationshipLevels = System.Enum.GetNames(typeof(RelationshipMatrix.RelationshipStatus)).Length;
        int increment = 100 / relationshipLevels;
        int curLevel = 100 - increment;
        for (int i = 0; i < relationshipLevels; i++)
        {
            if (relationShipLevel > curLevel)
            {
                relationshipStatus = (RelationshipMatrix.RelationshipStatus)i;
                break;
            }
            curLevel -= increment;
        }

        bool canChangeRelationshipType = false;
        switch (relationshipType)
        {
            case RelationshipMatrix.RelationShipType.Themself:
            case RelationshipMatrix.RelationShipType.Spouse:
            case RelationshipMatrix.RelationShipType.Sibling:
            case RelationshipMatrix.RelationShipType.ParentOf:
            case RelationshipMatrix.RelationShipType.ChildOf:
            case RelationshipMatrix.RelationShipType.Family:
            case RelationshipMatrix.RelationShipType.Divorced:
            case RelationshipMatrix.RelationShipType.Ex:
            case RelationshipMatrix.RelationShipType.SignificantOther:
                canChangeRelationshipType = false;
                break;


            case RelationshipMatrix.RelationShipType.CloseFriend:
            case RelationshipMatrix.RelationShipType.Friend:
            case RelationshipMatrix.RelationShipType.Acquaintance:
                canChangeRelationshipType = true;
                break;
        }
        if (canChangeRelationshipType)
        {
            float changeRate = UnityEngine.Random.Range(0f, 1f);
            switch (relationshipStatus)
            {
                case RelationshipMatrix.RelationshipStatus.Amazing:
                    if (changeRate < 0.4)
                    {
                        switch (relationshipType)
                        {
                            case RelationshipMatrix.RelationShipType.CloseFriend:
                                break;
                            case RelationshipMatrix.RelationShipType.Friend:
                                relationshipType = RelationshipMatrix.RelationShipType.CloseFriend;
                                relationShipLevel = 75;
                                break;
                            case RelationshipMatrix.RelationShipType.Acquaintance:
                                relationshipType = RelationshipMatrix.RelationShipType.Friend;
                                relationShipLevel = 75;
                                break;
                        }
                    }
                    break;
                case RelationshipMatrix.RelationshipStatus.Great:
                    if (changeRate < 0.1)
                    {
                        switch (relationshipType)
                        {
                            case RelationshipMatrix.RelationShipType.CloseFriend:
                                break;
                            case RelationshipMatrix.RelationShipType.Friend:
                                relationshipType = RelationshipMatrix.RelationShipType.CloseFriend;
                                relationShipLevel = 50;
                                break;
                            case RelationshipMatrix.RelationShipType.Acquaintance:
                                relationshipType = RelationshipMatrix.RelationShipType.Friend;
                                relationShipLevel = 50;
                                break;
                        }
                    }
                    break;
                case RelationshipMatrix.RelationshipStatus.Okay:
                    break;
                case RelationshipMatrix.RelationshipStatus.NotGood:
                    if (changeRate < 0.1)
                    {
                        switch (relationshipType)
                        {
                            case RelationshipMatrix.RelationShipType.CloseFriend:
                                relationshipType = RelationshipMatrix.RelationShipType.Friend;
                                relationShipLevel = 50;
                                break;
                            case RelationshipMatrix.RelationShipType.Friend:
                                relationshipType = RelationshipMatrix.RelationShipType.Acquaintance;
                                relationShipLevel = 50;
                                break;
                            case RelationshipMatrix.RelationShipType.Acquaintance:
                                break;
                        }
                    }
                    break;
                case RelationshipMatrix.RelationshipStatus.Terrible:
                    if (changeRate < 0.4)
                    {
                        switch (relationshipType)
                        {
                            case RelationshipMatrix.RelationShipType.CloseFriend:
                                relationshipType = RelationshipMatrix.RelationShipType.Friend;
                                relationShipLevel = 25;
                                break;
                            case RelationshipMatrix.RelationShipType.Friend:
                                relationshipType = RelationshipMatrix.RelationShipType.Acquaintance;
                                relationShipLevel = 25;
                                break;
                            case RelationshipMatrix.RelationShipType.Acquaintance:
                                break;
                        }
                    }
                    break;
            }
        }

    }

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

    public int CompareTo(object obj)
    {
        CharacterRelationShip relation = (CharacterRelationShip)obj;
        if (relation.relationshipType != relationshipType)
        {
            return relation.relationshipType > relationshipType ? -1 : 1;
        }
        else
        {
            if (relation.relationshipStatus != relationshipStatus)
            {
                return relation.relationshipStatus > relationshipStatus ? -1 : 1;
            }
            else
            {
                return relation.characterName.CompareTo(characterName);
            }
        }
    }
}




