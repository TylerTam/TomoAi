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


    public DialogueUiSystem DialogueUi;
    public ConversationData CurrentConversation;
    public CharacterData PlayerData;

    public System.Action StartedDataFetch;
    public System.Action<CharacterData, string> DataFetchRes;

#if UNITY_EDITOR
    [Header("Test Dialogue")]
    public bool UseTestDialogue;
    public TestDialogue TestDialogue;
#endif

    const string TEST_STRING = "hello there, I am the monitor of installation 04. I am called penetant Tangent.A reclaimer? Here? Brilliant! There is much to do, and no time to waste.We must start immediately, if we want to control this outbreak";
    private void Awake()
    {
        DialogueUi.CloseMenu(true);
    }
    public void StartConversation(CharacterData targetData, TomoCharPerson targetGameObject, bool aiSpeaksFirst = false, string scenarioPrompt = "")
    {
        CurrentConversation = new ConversationData();
        CurrentConversation.RelevantCharacters.Add(PlayerData.Name, PlayerData);
        CurrentConversation.RelevantCharacters.Add(targetData.Name, targetData);
        CurrentConversation.currentConversationGenSettings = ServerLink.Instance.GetGenSettType(ServerLink.GenerationType.Default).generationSettings;
        CurrentConversation.RelevantCharsController.Add(targetData.Name, targetGameObject);

        DialogueUi.OpenMenu(/*CurrentConversation*/);
        //SetupPersonaPrompt
        if (aiSpeaksFirst)
        {
            SendToGenerator(targetData.Name);
        }
    }

    public void AddPlayerDialogue(string playerText)
    {
        Debug.Log("Call the API here to send: " + playerText);
        DialogueUi.DisplayNewDialogue(PlayerData, playerText, true);
        AddDialogue(PlayerData, true, playerText);
        SendToGenerator(CurrentConversation.GetNextSpeaker(playerText));
    }
    public void AddDialogue(CharacterData speakingChar, bool isPlayer, string spokenText)
    {
        if(CurrentConversation == null)
        {
            CurrentConversation = new ConversationData();
        }
        CurrentConversation.AddDialogue(speakingChar, isPlayer, spokenText);
    }

    public void SendToGenerator(string speakingChar)
    {
        if (CurrentConversation != null)
        {
            CurrentConversation.RelevantCharsController[speakingChar].ShowThinkingBubble();

            string dialogueToSend = CurrentConversation.BuildCurrentPrompt() + " \n" + speakingChar + ":";
            
            Debug.Log(dialogueToSend);

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
    private void GeneratorResponse(bool generated,string speakingChar, string generatedText)
    {


        CurrentConversation.AddDialogue(speakingChar, generatedText);
        //Seach the game's data for a character that matches the name
        CharacterData charData = null;
        if (CurrentConversation.RelevantCharacters.ContainsKey(speakingChar))
        {
            charData = CurrentConversation.RelevantCharacters[speakingChar];
        }
        else
        {
            charData = new CharacterData();
            charData.Name = speakingChar;
            CurrentConversation.RelevantCharacters.Add(speakingChar, charData);
            Debug.Log("Implement solution to find the character's data in the game data");
        }
        DataFetchRes?.Invoke(charData, generatedText);
        if (CurrentConversation.RelevantCharsController.ContainsKey(speakingChar))
        {
            CurrentConversation.RelevantCharsController[speakingChar].WorldDialoguePopUp(charData, generatedText);
        }

    }



    public void EndConversation()
    {
        DialogueUi.CloseMenu(true);
        foreach(KeyValuePair<string, TomoCharPerson> per in CurrentConversation.RelevantCharsController)
        {
            per.Value.DialogueEnded();
        }
        CurrentConversation = null;
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
    public GenerationSettings currentConversationGenSettings;
    public string BuildCurrentPrompt()
    {
        StringBuilder sb = new StringBuilder();
        foreach(KeyValuePair<string, CharacterData> keys in RelevantCharacters)
        {
            if(keys.Value != DialogueSystem_Main.Instance.PlayerData)
            {
                sb.Append(keys.Value.ToString());
            }
        }
        if(scenarioPrompt != "" && string.IsNullOrWhiteSpace(scenarioPrompt))
        {
            sb.AppendLine("Scenario:" + scenarioPrompt);
        }
        sb.AppendLine("<START>");
        foreach (SpokenDialogue sd in SpokenDialogues)
        {
            sb.AppendLine(sd.ToString());
        }

        
        return sb.Replace("\r", "").ToString();
    }
    public void AddDialogue(CharacterData speakingChar, bool isPlayer, string spokenText)
    {
        SpokenDialogue spokenDialogue = new SpokenDialogue(isPlayer? "You" : speakingChar.Name, spokenText);
        SpokenDialogues.Add(spokenDialogue);
    }
    public void AddDialogue(string speakingChar, string spokenText)
    {
        SpokenDialogue spokenDialogue = new SpokenDialogue(speakingChar, spokenText);
        SpokenDialogues.Add(spokenDialogue);
        
        if(SpokenDialogues.Count >= currentConversationGenSettings.max_history_count)
        {
            SpokenDialogues.RemoveAt(0);
        }
    }

    public string GetNextSpeaker(string currentScentence)
    {
        List<string> currentChars = new List<string>();
        foreach(KeyValuePair <string, CharacterData> val in RelevantCharacters)
        {
            if(val.Value != DialogueSystem_Main.Instance.PlayerData)
            {
                currentChars.Add(val.Value.Name);
            }
        }
        return currentChars[Random.Range(0, currentChars.Count)];
    }


}