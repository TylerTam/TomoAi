using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Creator_SelectedCharacterAttribs : MonoBehaviour
{
    [SerializeField] private TomoCharPerson tomoChar;
    [SerializeField] private TMPro.TMP_InputField charName, charDescription, charPersonality, charAppearenceDescription;
    [SerializeField] private TMPro.TMP_InputField creatorInput;

    [Header("Dialogue Examples")]
    [SerializeField] private GameObject dialogueExPrefab;
    [SerializeField] private Transform dialogueExRoot;


    private CharacterData loadedCharacter;
    private bool markedDirty;
    private CharacterData tempLoadedData;

    public void LoadTomoChar(CharacterData charData)
    {
        tempLoadedData = charData;
        if (markedDirty)
        {
            PromptWindow.Instance.YesPressed.AddListener( delegate { SaveCharData(); LoadTomoChar(tempLoadedData); });
            PromptWindow.Instance.NoPressed.AddListener( delegate { markedDirty = false; LoadTomoChar(charData); });
            PromptWindow.Instance.ShowPromptWindow("Save changes to current character?" ,true);
            return;
        }
        markedDirty = false;
        int dialogueExChilds = dialogueExRoot.childCount;
        for (int i = 0; i < dialogueExChilds; i++)
        {
            ObjectPooler.ReturnToPool( dialogueExRoot.GetChild(0).gameObject);
        }

        charName.text = charData.Name;
        charDescription.text = charData.Description;
        charPersonality.text = charData.Personality_Summary;
        charAppearenceDescription.text = charData.AppearanceDescription;
        for (int i = 0; i < charData.Examples_Of_Dialogue.Length; i++)
        {
            Creator_DialogueExampleInput dialEx = ObjectPooler.NewObject(dialogueExPrefab, Vector3.zero, Quaternion.identity).GetComponent<Creator_DialogueExampleInput>();
            dialEx.transform.parent = dialogueExRoot;
            dialEx.InitDialogueExample(charData.Examples_Of_Dialogue[i]);
        }
    }

    public void CancelEdit()
    {
        markedDirty = false;
        LoadTomoChar(tempLoadedData);
    }
    public void MarkCharacterDirty()
    {
        markedDirty = true;
    }

    public void SaveCharData()
    {
        GameManager.Instance.SaveLoader.UpdateCharacter(loadedCharacter, false);
        markedDirty = false;
    }

}
