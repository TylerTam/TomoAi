using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueSystem_Main : MonoBehaviour
{
    public static DialogueSystem_Main Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DialogueSystem_Main>();
            }
            return _instance;
        }
    }
    private static DialogueSystem_Main _instance;


    [SerializeField] private DialogueUiSystem dialogueUi;
    [SerializeField] private ConversationData currentConversation;

    public System.Action StartedDataFetch;
    public System.Action<CharacterData, string> DataFetchRes;
    public ScenarioPromptManager scenarioPromptManager;

    [SerializeField] private AllPromptActions allPromptActions;

#if UNITY_EDITOR
    [Header("Test Dialogue")]
    public bool UseTestDialogue;
    public TestDialogue TestDialogue;
#endif

    const string TEST_STRING = "hello there, I am the monitor of installation 04. I am called penetant Tangent.A reclaimer? Here? Brilliant! There is much to do, and no time to waste.We must start immediately, if we want to control this outbreak";
    private void Awake()
    {
        dialogueUi.CloseMenu(true);
    }


    public string GetPromptAction(AllPromptActions.ActionType promptAction, string charName)
    {
        return allPromptActions.GetPrompt(promptAction, charName);
    }
    public void StartConversationWithPlayer(CharacterData targetData, TomoCharPerson targetGameObject, bool aiSpeaksFirst = false, string startingActionPrompt = "", string scenarioPrompt = "")
    {
        currentConversation = new ConversationData();
        currentConversation.RelevantCharacters.Add(GameManager.Instance.PlayerCharacterData.Name, GameManager.Instance.PlayerCharacterData);
        currentConversation.RelevantCharacters.Add(targetData.Name, targetData);
        currentConversation.currentConversationGenSettings = ServerLink.Instance.GetGenSettType(ServerLink.GenerationType.Default).generationSettings;
        currentConversation.RelevantCharsController.Add(targetData.Name, targetGameObject);

        currentConversation.scenarioPrompt = scenarioPrompt;
        if (string.IsNullOrWhiteSpace(scenarioPrompt))
        {
            List<string> names = new List<string>();
            names.Add(GameManager.Instance.PlayerCharacterData.Name);
            names.Add(targetData.Name);
            currentConversation.scenarioPrompt = scenarioPromptManager.GetPrompt(names);
        }


        currentConversation.startingActionPrompt = startingActionPrompt;
        dialogueUi.OpenMenu(/*CurrentConversation*/);
        //SetupPersonaPrompt
        if (aiSpeaksFirst)
        {
            SendToGenerator(targetData.Name);
        }
    }

    public void AddPlayerDialogue(string playerText)
    {
        Debug.Log("Call the API here to send: " + playerText);
        dialogueUi.DisplayNewDialogue(GameManager.Instance.PlayerCharacterData, playerText, true);
        AddDialogue(GameManager.Instance.PlayerCharacterData, true, playerText);
        SendToGenerator(currentConversation.GetNextSpeaker(playerText));
    }
    public void AddDialogue(CharacterData speakingChar, bool isPlayer, string spokenText)
    {
        if (currentConversation == null)
        {
            currentConversation = new ConversationData();
        }
        currentConversation.AddDialogue(speakingChar, isPlayer, spokenText);
    }

    public void SendToGenerator(string speakingChar)
    {
        if (currentConversation != null)
        {
            currentConversation.RelevantCharsController[speakingChar].ShowThinkingBubble();

            string dialogueToSend = currentConversation.BuildCurrentPrompt() + " \n" + speakingChar + ":";

            
            //Debug.Log(dialogueToSend);

            ServerLink.Instance.StartGenerator(dialogueToSend, speakingChar, GeneratorResponse);
        }
        else
        {
            Debug.LogError("There is no conversation data to send to the generator");
        }
    }

    /// <summary>
    /// The callback from the generator. This adds the dialogue to the conversation history, and displays it on the spoken npc
    /// </summary>
    /// <param name="generated"></param>
    /// <param name="speakingChar"></param>
    /// <param name="generatedText"></param>
    private void GeneratorResponse(bool generated, string speakingChar, string generatedText)
    {


        currentConversation.AddDialogue(speakingChar, generatedText);
        //Seach the game's data for a character that matches the name
        CharacterData charData = null;
        if (currentConversation.RelevantCharacters.ContainsKey(speakingChar))
        {
            charData = currentConversation.RelevantCharacters[speakingChar];
        }
        else
        {
            Debug.Log("Error!!!!!!!!");
            Debug.Log("Implement solution to find the character's data in the game data");
            Debug.Break();

        }
        DataFetchRes?.Invoke(charData, generatedText);
        if (currentConversation.RelevantCharsController.ContainsKey(speakingChar))
        {
            currentConversation.RelevantCharsController[speakingChar].WorldDialoguePopUp(charData, generatedText);
        }

    }



    public void EndConversation()
    {
        if (currentConversation == null) return;
        dialogueUi.CloseMenu(true);
        foreach (KeyValuePair<string, TomoCharPerson> per in currentConversation.RelevantCharsController)
        {
            per.Value.DialogueEnded();
        }
        currentConversation = null;
    }

}

[System.Serializable]
public struct SpokenDialogue
{
    [SerializeField] public string SpeakingCharacter, SpokenText;
    public SpokenDialogue(string speakingCharName, string spokenTxt)
    {
        SpeakingCharacter = speakingCharName;
        SpokenText = spokenTxt;
    }
    public override string ToString()
    {
        return SpeakingCharacter + ": " + SpokenText;
    }
}

public class ConversationData
{
    public List<SpokenDialogue> SpokenDialogues = new List<SpokenDialogue>();
    public Dictionary<string, CharacterData> RelevantCharacters = new Dictionary<string, CharacterData>();
    public Dictionary<string, TomoCharPerson> RelevantCharsController = new Dictionary<string, TomoCharPerson>();

    public string scenarioPrompt;
    public string startingActionPrompt;
    public GenerationSettings currentConversationGenSettings;
    public Dictionary<string, int> appendedRelationshipPrompts = new Dictionary<string, int>();

    private const int keepRelationshipPrompt = 10;
    public List<string> GetRelevantCharactersInText()
    {

        //if (SpokenDialogues == null || SpokenDialogues.Count <= 0) return new List<string>();
        
            List<string> result = new List<string>();
        foreach(KeyValuePair<string, CharacterData> keys in RelevantCharacters)
        {
            if(keys.Value.LocalId != GameManager.Instance.PlayerCharacterData.LocalId)
            {
                foreach(CharacterRelationShip characterRelationShip in keys.Value.serializedRelationships)
                {
                    if (characterRelationShip.characterName == keys.Value.Name ||
                        string.IsNullOrWhiteSpace(characterRelationShip.characterName)) continue;
                    if (!result.Contains(characterRelationShip.characterName))
                    {
                        result.Add(characterRelationShip.characterName);
                    }
                }
            }
        }


        if (SpokenDialogues != null && SpokenDialogues.Count > 0)
        {

            for (int i = result.Count - 1; i >= 0; i--)
            {
                if (!SpokenDialogues[SpokenDialogues.Count - 1].SpokenText.Contains(result[i]))
                {
                    result.RemoveAt(i);
                }
            }
        }

        return result;
    }
    public string BuildCurrentPrompt()
    {

        List<string> relationshipNames = GetRelevantCharactersInText();
        foreach(string nam in relationshipNames)
        {
            if (appendedRelationshipPrompts.ContainsKey(nam))
            {
                appendedRelationshipPrompts[nam] = keepRelationshipPrompt;
            }
            else
            {
                appendedRelationshipPrompts.Add(nam, keepRelationshipPrompt);
            }
        }

        relationshipNames.Clear();
        foreach(KeyValuePair<string, int> nam in appendedRelationshipPrompts)
        {
            relationshipNames.Add(nam.Key);
        }
        

        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<string, CharacterData> keys in RelevantCharacters)
        {
            if (keys.Value.LocalId != GameManager.Instance.PlayerCharacterData.LocalId)
            {
                sb.Append(keys.Value.BuildPersona(relationshipNames));
            }
        }
        if (scenarioPrompt != "" && !string.IsNullOrWhiteSpace(scenarioPrompt))
        {
            sb.AppendLine("Scenario:" + scenarioPrompt);
        }
        sb.AppendLine("<START>");
        foreach (SpokenDialogue sd in SpokenDialogues)
        {
            sb.AppendLine(sd.ToString());
        }
        if (startingActionPrompt != "" && !string.IsNullOrWhiteSpace(startingActionPrompt))
        {
            sb.AppendLine(startingActionPrompt);
            startingActionPrompt = "";
        }
        return sb.Replace("\r", "").ToString();
    }

    /// <summary>
    /// Typically Player Dialogue
    /// </summary>
    public void AddDialogue(CharacterData speakingChar, bool isPlayer, string spokenText)
    {
        SpokenDialogue spokenDialogue = new SpokenDialogue(isPlayer ? "You" : speakingChar.Name, spokenText);
        SpokenDialogues.Add(spokenDialogue);
        ReduceReleventCharacterCounters();
    }

    /// <summary>
    /// NPC Dialogue
    /// </summary>

    public void AddDialogue(string speakingChar, string spokenText)
    {
        SpokenDialogue spokenDialogue = new SpokenDialogue(speakingChar, spokenText);
        SpokenDialogues.Add(spokenDialogue);

        if (SpokenDialogues.Count >= currentConversationGenSettings.max_history_count)
        {
            SpokenDialogues.RemoveAt(0);
        }
        ReduceReleventCharacterCounters();
    }

    public void ReduceReleventCharacterCounters()
    {
        List<string> relationShipKeys = new List<string>(appendedRelationshipPrompts.Keys);
        for (int i = 0; i < relationShipKeys.Count; i++)
        {
            appendedRelationshipPrompts[relationShipKeys[i]] -= 1;
            if (appendedRelationshipPrompts[relationShipKeys[i]]<=0)
            {
                appendedRelationshipPrompts.Remove(relationShipKeys[i]);
            }
        }
    }
    public string GetNextSpeaker(string currentScentence)
    {
        List<string> currentChars = new List<string>();
        foreach (KeyValuePair<string, CharacterData> val in RelevantCharacters)
        {
            if (val.Value != GameManager.Instance.PlayerCharacterData)
            {
                currentChars.Add(val.Value.Name);
            }
        }
        return currentChars[Random.Range(0, currentChars.Count)];
    }


}