using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PromptActionMain", menuName = "Data/Dialogue/PromptActionMain", order = 0)]
public class AllPromptActions : ScriptableObject
{
    public enum ActionType
    {
        None
        ,TapOnNPC
    }
    [SerializeField] private RotaryHeart.Lib.SerializableDictionary.SerializableDictionaryBase<ActionType, PromptAction> allPromptActions;

    public string GetPrompt(ActionType actType, string charName)
    {
        
        return allPromptActions[actType].GetRandomPrompt(charName);
    }
}
