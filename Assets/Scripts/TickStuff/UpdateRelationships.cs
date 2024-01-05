using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateRelationships : TickUpdate
{
    /// <summary>
    /// This will be used to determine what type of relationship increment the npcs should have
    /// </summary>
    [SerializeField] private NpcPlacements npcPlacements;
    public override void UpdateFromTick()
    {
        RelationshipIncrements increments = GameManager.Instance.RelationshipMatrix.increments;

        List<CharacterData> allChars = new List<CharacterData>(GameManager.Instance.SaveLoader.LoadedData.SavedCharacters);
        NpcAreaData npcAreaChar1, npcAreaChar2;

        //Update existing relationships
        for (int i = 0; i < allChars.Count; i++)
        {
            npcAreaChar1 = npcPlacements.GetNpcArea(allChars[i].LocalId);
            for (int o = 0; o < allChars[i].serializedRelationships.Count; o++)
            {


                npcAreaChar2 = npcPlacements.GetNpcArea(allChars[i].serializedRelationships[o].characterId);

                if(npcAreaChar1.area != npcAreaChar2.area)
                {
                    //If the characters arent directly interacting with each other
                    allChars[i].serializedRelationships[o].AdjustRelationshipLevel(increments.GetIncrement(RelationshipIncrements.IncrementType.Idle));
                    continue;
                }else
                {

                    if(npcAreaChar1.area == npcAreaChar2.area && npcAreaChar1.area == NpcPlacements.AreaType.Apartments)
                    {
                        if(npcAreaChar1.ApartmentOwnerId != npcAreaChar2.ApartmentOwnerId)
                        {
                            allChars[i].serializedRelationships[o].AdjustRelationshipLevel(increments.GetIncrement(RelationshipIncrements.IncrementType.Idle));
                            continue;
                        }
                        else
                        {
                            allChars[i].serializedRelationships[o].AdjustRelationshipLevel(increments.GetIncrement(RelationshipIncrements.IncrementType.HangingOut));
                            continue;
                        }
                    }

                    ///Temp if. Will replace
                    ///REplacement should be 
                    ///     -They are in the same place
                    ///         -Are they fighting?
                    if (true)
                    {
                        //If the characters arent directly interacting with each other, and just chatting
                        allChars[i].serializedRelationships[o].AdjustRelationshipLevel(increments.GetIncrement(RelationshipIncrements.IncrementType.Chatting));

                    }
                    else
                    {
                        //If the characters arent directly interacting with each other, and fighting
                        allChars[i].serializedRelationships[o].AdjustRelationshipLevel(increments.GetIncrement(RelationshipIncrements.IncrementType.Fighting));
                    }
                }
            }
        }

        //Construct new relationships
        int areaCount = System.Enum.GetNames(typeof(NpcPlacements.AreaType)).Length;
        for (int i = 0; i < areaCount; i++)
        {
            NpcPlacements.AreaType area = (NpcPlacements.AreaType)i;
            if (area == NpcPlacements.AreaType.Apartments) continue;
            List<NpcAreaData> npcsInSameArea = npcPlacements.GetAllCharactersInArea(area);
            for (int o = 0; o < npcsInSameArea.Count; o++)
            {
                for (int p = 0; p < npcsInSameArea.Count; p++)
                {
                    if (o == p) continue;
                    CharacterData char1 = GameManager.Instance.SaveLoader.GetCharacterByID(npcsInSameArea[o].npcId);
                    if (!char1.Knows(npcsInSameArea[p].npcId))
                    {
                        CharacterData char2 = GameManager.Instance.SaveLoader.GetCharacterByID(npcsInSameArea[p].npcId);
                        char1.UpdateRelationship(char2, RelationshipMatrix.RelationShipType.Acquaintance, RelationshipMatrix.RelationshipStatus.Okay);
                        char2.UpdateRelationship(char1, RelationshipMatrix.RelationShipType.Acquaintance, RelationshipMatrix.RelationshipStatus.Okay);
                    }

                }
            }
            
        }
        
    }
}
