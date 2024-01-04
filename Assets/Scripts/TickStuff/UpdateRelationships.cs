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
    }
}
