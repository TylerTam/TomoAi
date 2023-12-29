using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PromptAction_", menuName = "Data/Dialogue/PromptAction", order = 0)]
public class PromptAction : ScriptableObject
{
    [SerializeField] private string npcTag = "<NPC>";
    [SerializeField] private string playerTag = "<USER>";
    [SerializeField] [TextArea] private List<string> promptActions;

    public string GetRandomPrompt(string charName)
    {
        string randomPrompt = promptActions[Random.Range(0, promptActions.Count)];
        randomPrompt = randomPrompt.Replace(npcTag, charName);   
        randomPrompt = randomPrompt.Replace(playerTag, GameManager.Instance.SaveLoader.LoadedData.Player.PlayerName);
        return "*" + randomPrompt + "*";
        
    }
}
