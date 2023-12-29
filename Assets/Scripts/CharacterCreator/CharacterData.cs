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

    public CharacterData (string charName, string creator, float creatorId)
    {
        Name = charName;
        Creator = creator;
        Creator_Id = creatorId;
        Examples_Of_Dialogue = new string[0];
        CharacterAppearence = new CharacterAppearance();
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

    public override string ToString()
    {
        return CreateCharacterPrompt();
    }
    public string CreateCharacterPrompt()
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
        return sb.ToString();

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





