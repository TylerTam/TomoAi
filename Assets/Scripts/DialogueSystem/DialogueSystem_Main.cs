using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
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


    [SerializeField] private ConversationData currentConversation;

    public System.Action StartedDataFetch;
    public System.Action<CharacterData, string> DataFetchRes;
    public ScenarioPromptManager scenarioPromptManager;

    [SerializeField] private AllPromptActions allPromptActions;
    public ServerLink.GenerationType currentGenerationType = ServerLink.GenerationType.Default;

#if UNITY_EDITOR
    [Header("Test Dialogue")]
    public bool UseTestDialogue;
    public TestDialogue TestDialogue;
#endif

    const string TEST_STRING = "hello there, I am the monitor of installation 04. I am called penetant Tangent.A reclaimer? Here? At last! There is much to do, and no time to waste.We must act quickly, if we are to control this outbreak";


    public string GetPromptAction(AllPromptActions.ActionType promptAction, string charName)
    {
        return allPromptActions.GetPrompt(promptAction, charName);
    }
    public void StartConversationWithPlayer(CharacterData targetData, TomoCharPerson targetTomoChar, bool aiSpeaksFirst = false, string startingActionPrompt = "", string scenarioPrompt = "")
    {
        currentConversation = new ConversationData();
        currentConversation.RelevantCharacters.Add(GameManager.Instance.PlayerCharacterData.LocalId, GameManager.Instance.PlayerCharacterData);
        currentConversation.currentConversationGenSettings = ServerLink.Instance.GetGenSettType(currentGenerationType).generationSettings;
        
        currentConversation.AddNpc(targetTomoChar);


        currentConversation.scenarioPrompt = scenarioPrompt;
        if (string.IsNullOrWhiteSpace(scenarioPrompt))
        {
            List<string> names = new List<string>();
            names.Add(GameManager.Instance.PlayerCharacterData.Name);
            names.Add(targetData.Name);
            currentConversation.scenarioPrompt = scenarioPromptManager.GetPrompt(names);
        }


        currentConversation.startingActionPrompt = startingActionPrompt;
        //SetupPersonaPrompt
        if (aiSpeaksFirst)
        {
            SendToGenerator(targetData.LocalId,true, GeneratorResponse);
        }
    }
    public void StartConversationWithNpcsOnly(List<TomoCharPerson> relevantCharacters, System.Action<bool, int, string> dialogueRecieved, string startingActionPrompt = "", string scenarioPrompt = "")
    {
        currentConversation = new ConversationData();
        currentConversation.currentConversationGenSettings = ServerLink.Instance.GetGenSettType(currentGenerationType).generationSettings;

        List<string> names = new List<string>();
        foreach (TomoCharPerson charPerson in relevantCharacters)
        {
            currentConversation.AddNpc(charPerson);
            names.Add(charPerson.CharData.Name);
        }

        currentConversation.scenarioPrompt = scenarioPrompt;
        if (string.IsNullOrWhiteSpace(scenarioPrompt))
        {
            currentConversation.scenarioPrompt = scenarioPromptManager.GetPrompt(names);
        }


        currentConversation.startingActionPrompt = startingActionPrompt;


        SendToGenerator(relevantCharacters[Random.Range(0, relevantCharacters.Count)].CharData.LocalId, true, dialogueRecieved) ;

    }
    
    /// <summary>
    /// If the user submits a blank sentence, and also used for multiple npc dialogues
    /// </summary>
    public void ContinueWithAiGenerate(System.Action<bool, int, string> dialogueRecieved)
    {

        SendToGenerator(currentConversation.RelevantCharacters[currentConversation.GetRandomCharIndex()].LocalId,false, dialogueRecieved);
    }
    public void AddPlayerDialogue(string playerText)
    {

        AddDialogue(GameManager.Instance.PlayerCharacterData.LocalId, true, playerText);
        SendToGenerator(currentConversation.GetNextSpeaker(playerText),true, GeneratorResponse);
    }
    public void AddDialogue(int speakingCharId, bool isPlayer, string spokenText)
    {
        if (currentConversation == null)
        {
            currentConversation = new ConversationData();
        }
        currentConversation.AddDialogue(speakingCharId, isPlayer, spokenText);
    }

    public void SendToGenerator(int speakingCharId, bool showThinkingBubble, System.Action<bool, int, string> response)
    {
        if (currentConversation != null)
        {
            currentConversation.currentSpeakerId = speakingCharId;
            if (showThinkingBubble)
            {
                currentConversation.RelevantCharsController[speakingCharId].ShowThinkingBubble();
            }

            string dialogueToSend = currentConversation.BuildCurrentPrompt() + " \n" + currentConversation.RelevantCharacters[speakingCharId].Name + ":";
            //Debug.Log(dialogueToSend);
            ServerLink.Instance.StartGenerator(dialogueToSend, speakingCharId, currentConversation.GetGeneratorTemperature(speakingCharId), response);
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
    /// <param name="speakingCharId"></param>
    /// <param name="generatedText"></param>
    private void GeneratorResponse(bool generated, int speakingCharId, string generatedText)
    {


        currentConversation.AddDialogue(speakingCharId, generatedText);
        //Seach the game's data for a character that matches the name
        CharacterData charData = null;
        if (currentConversation.RelevantCharacters.ContainsKey(speakingCharId))
        {
            charData = currentConversation.RelevantCharacters[speakingCharId];
        }
        else
        {
            Debug.Log("Error!!!!!!!!");
            Debug.Log("Implement solution to find the character's data in the game data");
            Debug.Break();

        }
        DataFetchRes?.Invoke(charData, generatedText);
        if (currentConversation.RelevantCharsController.ContainsKey(speakingCharId))
        {
            currentConversation.RelevantCharsController[speakingCharId].WorldDialoguePopUp(generatedText);
        }

    }

    


    public void EndConversation()
    {
        if (currentConversation == null) return;
        currentConversation = null;
    }

    public int GetCurrentThinkingCharacterId()
    {
        return currentConversation.currentSpeakerId;
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
    public Dictionary<int, CharacterData> RelevantCharacters = new Dictionary<int, CharacterData>();
    public Dictionary<int, TomoCharPerson> RelevantCharsController = new Dictionary<int, TomoCharPerson>();

    public string scenarioPrompt;
    public string startingActionPrompt;
    public double tempAddition = 0.0f;
    public GenerationSettings currentConversationGenSettings;
    public Dictionary<int, int> appendedRelationshipPrompts = new Dictionary<int, int>();

    private const int keepRelationshipPrompt = 10;

    public int currentSpeakerId;
    private List<int> relevantCharIds = new List<int>();
    public int GetRandomCharIndex()
    {
        return relevantCharIds[Random.Range(0, relevantCharIds.Count)];
    }
    public void AddNpc(TomoCharPerson tomochar)
    {
        RelevantCharacters.Add(tomochar.CharData.LocalId, tomochar.CharData);
        RelevantCharsController.Add(tomochar.CharData.LocalId, tomochar);
        relevantCharIds.Add(tomochar.CharData.LocalId);
    }
    public double GetGeneratorTemperature(int speakingCharId)
    {
        if (!RelevantCharacters.ContainsKey(speakingCharId)) return 0.8f + tempAddition;
        return RelevantCharacters[speakingCharId].GetIntensity + tempAddition;
    }
    public List<string> GetRelevantCharactersInText()
    {

        if (SpokenDialogues == null || SpokenDialogues.Count <= 0) return new List<string>();

        List<string> result = new List<string>();
        foreach (KeyValuePair<int, CharacterData> keys in RelevantCharacters)
        {
            if (keys.Value.LocalId != GameManager.Instance.PlayerCharacterData.LocalId)
            {
                foreach (CharacterRelationShip characterRelationShip in keys.Value.serializedRelationships)
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
                if (!SpokenDialogues[SpokenDialogues.Count - 1].SpokenText.ToLower().Contains(result[i].ToLower()))
                {
                    result.RemoveAt(i);
                }
            }
        }

        return result;
    }
    public string BuildCurrentPrompt()
    {
        #region Relevant Characters
        List<string> relationshipNames = GetRelevantCharactersInText();
        foreach (string nam in relationshipNames)
        {
            List<int> sharedCharacterIndexes = new List<int>();
            sharedCharacterIndexes = GameManager.Instance.SaveLoader.GetCharacterIdsByName(nam);
            bool hasIndex = false;
            for (int i = 0; i < sharedCharacterIndexes.Count; i++)
            {
                if (appendedRelationshipPrompts.ContainsKey(sharedCharacterIndexes[i]))
                {
                    appendedRelationshipPrompts[sharedCharacterIndexes[i]] = keepRelationshipPrompt;
                    hasIndex = true;
                    break;
                }
            }
            if (!hasIndex)
            {
                appendedRelationshipPrompts.Add(sharedCharacterIndexes[Random.Range(0, sharedCharacterIndexes.Count)], keepRelationshipPrompt);
            }


        }

        List<int> characterIds = new List<int>();
        foreach (KeyValuePair<int, int> nam in appendedRelationshipPrompts)
        {
            characterIds.Add(nam.Key);
        }
        #endregion

        #region Persona / Scenario
        StringBuilder sb = new StringBuilder();

        sb.Append(RelevantCharacters[currentSpeakerId].BuildPersona(characterIds));

        if (scenarioPrompt != "" && !string.IsNullOrWhiteSpace(scenarioPrompt))
        {
            sb.AppendLine("Scenario:" + scenarioPrompt);
            if (RelevantCharacters[currentSpeakerId].currentMood != CharacterData.Mood.Neutral)
            {
                sb.AppendLine(" " + RelevantCharacters[currentSpeakerId].Name + " is " + RelevantCharacters[currentSpeakerId].currentMood + " at something. ");
            }
        }
        else
        {

            sb.Append("Scenario:");
            if (RelevantCharacters[currentSpeakerId].currentMood != CharacterData.Mood.Neutral)
            {
                sb.AppendLine(" " + RelevantCharacters[currentSpeakerId].Name + " is " + RelevantCharacters[currentSpeakerId].currentMood + " at something. ");
            }
        }
        #endregion


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

    #region original prompt builder (commented out)
    //Original Prompt Builder
    /*public string BuildCurrentPrompt()
    {
        #region Relevant Characters
        List<string> relationshipNames = GetRelevantCharactersInText();
        foreach (string nam in relationshipNames)
        {
            List<int> sharedCharacterIndexes = new List<int>();
            sharedCharacterIndexes = GameManager.Instance.SaveLoader.GetCharacterIdsByName(nam);
            bool hasIndex = false;
            for (int i = 0; i < sharedCharacterIndexes.Count; i++)
            {
                if (appendedRelationshipPrompts.ContainsKey(sharedCharacterIndexes[i]))
                {
                    appendedRelationshipPrompts[sharedCharacterIndexes[i]] = keepRelationshipPrompt;
                    hasIndex = true;
                    break;
                }
            }
            if (!hasIndex)
            {
                appendedRelationshipPrompts.Add(sharedCharacterIndexes[Random.Range(0, sharedCharacterIndexes.Count)], keepRelationshipPrompt);
            }


        }

        List<int> characterIds = new List<int>();
        foreach (KeyValuePair<int, int> nam in appendedRelationshipPrompts)
        {
            characterIds.Add(nam.Key);
        }
        #endregion

        #region Persona / Scenario
        StringBuilder sb = new StringBuilder();

        foreach (KeyValuePair<int, CharacterData> keys in RelevantCharacters)
        {
            if (keys.Value.LocalId != GameManager.Instance.PlayerCharacterData.LocalId)
            {
                sb.Append(keys.Value.BuildPersona(characterIds));
            }
        }

        if (scenarioPrompt != "" && !string.IsNullOrWhiteSpace(scenarioPrompt))
        {
            sb.AppendLine("Scenario:" + scenarioPrompt);
            foreach (KeyValuePair<int, CharacterData> key in RelevantCharacters)
            {
                if (key.Value.LocalId == GameManager.Instance.PlayerCharacterData.LocalId) continue;
                if (key.Value.currentMood != CharacterData.Mood.Neutral)
                {
                    sb.Append(" " + key.Value.Name + " is " + key.Value.currentMood + " at something. ");
                }
            }
            sb.Append("\n");
        }
        else
        {
            StringBuilder moodSb = new StringBuilder();
            bool hasMood = false;
            moodSb.Append("Scenario:");
            foreach (KeyValuePair<int, CharacterData> key in RelevantCharacters)
            {
                if (key.Value.LocalId == GameManager.Instance.PlayerCharacterData.LocalId) continue;
                if (key.Value.currentMood != CharacterData.Mood.Neutral)
                {
                    hasMood = true;
                    moodSb.Append(" " + key.Value.Name + " is " + key.Value.currentMood + " at something. ");
                }
            }
            if (hasMood)
            {
                sb.AppendLine(moodSb.ToString());
            }
        }
        #endregion


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
    }*/
    
    #endregion

    /// <summary>
    /// Typically Player Dialogue
    /// </summary>
    public void AddDialogue(int speakingCharId, bool isPlayer, string spokenText)
    {

        SpokenDialogue spokenDialogue = new SpokenDialogue(isPlayer ? "You" : RelevantCharacters[speakingCharId].Name, spokenText);
        SpokenDialogues.Add(spokenDialogue);
        ReduceReleventCharacterCounters();
    }

    /// <summary>
    /// NPC Dialogue
    /// </summary>

    public void AddDialogue(int speakingCharId, string spokenText)
    {
        SpokenDialogue spokenDialogue = new SpokenDialogue(RelevantCharacters[speakingCharId].Name, spokenText);
        SpokenDialogues.Add(spokenDialogue);

        if (SpokenDialogues.Count >= currentConversationGenSettings.max_history_count)
        {
            SpokenDialogues.RemoveAt(0);
        }
        ReduceReleventCharacterCounters();
    }

    public void ReduceReleventCharacterCounters()
    {
        List<int> relationShipKeys = new List<int>(appendedRelationshipPrompts.Keys);
        for (int i = 0; i < relationShipKeys.Count; i++)
        {
            appendedRelationshipPrompts[relationShipKeys[i]] -= 1;
            if (appendedRelationshipPrompts[relationShipKeys[i]] <= 0)
            {
                appendedRelationshipPrompts.Remove(relationShipKeys[i]);
            }
        }
    }
    public int GetNextSpeaker(string currentScentence)
    {
        //Maybe search for a character to speak next, if their name is mentioned in the current scentence
        List<int> currentChars = new List<int>();
        foreach (KeyValuePair<int, CharacterData> val in RelevantCharacters)
        {
            if (val.Value != GameManager.Instance.PlayerCharacterData)
            {
                currentChars.Add(val.Value.LocalId);
            }
        }
        return currentChars[Random.Range(0, currentChars.Count)];
    }


}