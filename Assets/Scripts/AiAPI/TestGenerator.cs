using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TestGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    public string prompt = "oi";
    public string prompt2 = "oi";
    public string prompt3 = "oi";
    public TestingCharacterData testingChar;
    public TestingCharacterData testChar2;
    private CharacterData PlayerData;
    

    private void Start()
    {
        PlayerData = GameManager.Instance.SaveLoader.LoadedData.Player.ToCharData();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {

            
            ServerLink.Instance.StartGenerator(BuildTestConvo(), testingChar.characterData.Name, testingChar.characterData.GetIntensity, Response);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(ServerLink.Instance.CleanString(prompt3));
        }
        if (Input.GetKeyDown(KeyCode.I))
        {

            Debug.Log(BuildTestConvo());
            //CharacterData chardata = GameManager.Instance.SaveLoader.GetCharacterByID(testingChar.characterData.LocalId);
            //Debug.Log(chardata.BuildRelationshipPrompt(testChar2.Name));
        }
    }


    private string BuildPrompt()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(testingChar.ToString());
        sb.AppendLine(testingChar.characterData.Name + prompt);
        sb.AppendLine("<START>");
        sb.AppendLine("You: " + prompt2);
        sb.AppendLine(testingChar.characterData.Name + ": ");
        return sb.ToString();
    }
    private void Response(bool success, string speakingChar, string response)
    {
        Debug.Log("Success: " + success + " | Char: " + speakingChar + " | Res: " + response);
    }

    private string BuildTestConvo()
    {
        ConversationData currentConversation = new ConversationData();
        currentConversation.RelevantCharacters.Add(PlayerData.Name, PlayerData);
        currentConversation.RelevantCharacters.Add(testingChar.characterData.Name, GameManager.Instance.SaveLoader.GetCharacterByID( testingChar.characterData.LocalId));
        currentConversation.currentConversationGenSettings = ServerLink.Instance.GetGenSettType(ServerLink.GenerationType.Default).generationSettings;
        //currentConversation.RelevantCharsController.Add(targetData.Name, targetGameObject);
        currentConversation.scenarioPrompt = "";
        currentConversation.startingActionPrompt = "";

        currentConversation.AddDialogue(PlayerData.Name, prompt);

        return currentConversation.BuildCurrentPrompt() + " \n" + testingChar.characterData.Name + ":";
    }
#endif
}
