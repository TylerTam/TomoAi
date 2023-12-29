using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.ComponentModel;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class Creator_SelectedCharacterAttribs : MonoBehaviour
{
    [SerializeField] private EditorMenu editorMenu;
    [SerializeField] private TomoCharPerson tomoChar;
    [SerializeField] private Creator_InputSection charName, charDescription, charPersonality, charAppearenceDescription;
    [SerializeField] private Creator_InputSection creatorInput;

    [Header("Dialogue Examples")]
    [SerializeField] private GameObject dialogueExPrefab;
    [SerializeField] private Transform dialogueExRoot;
    [SerializeField] private Transform addDialogueButton;

    [SerializeField] private List<ContentSizeFitterEdit> sizeFitters;
    private List<Creator_DialogueExampleInput> addedDialogues = new List<Creator_DialogueExampleInput>();
    [SerializeField] private ContentSizeFitterEdit dialogueEditor;
    [SerializeField] private VerticalLayoutGroup dialogueLayout;
    [SerializeField] private ScrollRect scrollRect;
    private CharacterData loadedCharacter;
    [SerializeField] private int maxDialogueExamples = 11;

    private bool markedDirty;
    public void MarkDirty()
    {
        markedDirty = true;
    }
    public void LoadTomoChar(CharacterData charData)
    {

        ClearAttribs();

        charName.Init(charData.Name,this);
        charDescription.Init(charData.Description,this);
        charPersonality.Init(charData.Personality_Summary,this);
        charAppearenceDescription.Init(charData.AppearanceDescription,this);

        addedDialogues.Clear();
        for (int i = 0; i < charData.Examples_Of_Dialogue.Length; i++)
        {
            Creator_DialogueExampleInput dialEx = ObjectPooler.NewObject(dialogueExPrefab, Vector3.zero, Quaternion.identity).GetComponent<Creator_DialogueExampleInput>();
            dialEx.transform.parent = dialogueExRoot;
            dialEx.transform.localPosition = Vector3.zero;
            dialEx.transform.localRotation = Quaternion.identity;
            dialEx.transform.localScale = Vector3.one;
            dialEx.InitDialogueExample(charData.Examples_Of_Dialogue[i], this);
            dialEx.gameObject.SetActive(false);
            dialEx.gameObject.SetActive(true);
            addedDialogues.Add(dialEx);
        }

        
        addDialogueButton.transform.SetAsLastSibling();

        addDialogueButton.gameObject.SetActive(dialogueExRoot.childCount-1< maxDialogueExamples);
        
        tomoChar.ConstructTomoChar(charData);
        markedDirty = false;
        StartCoroutine(DelayForceUpdateFitters());
    }


    public void ExitAttribs()
    {
        if (markedDirty)
        {
            PromptWindow.Instance.YesPressed.AddListener(delegate {SaveCharData(); Debug.Log("Exit Herer"); });
            //PromptWindow.Instance.NoPressed.AddListener();
            PromptWindow.Instance.ShowPromptWindow("Are you sure you want to leave? Any unsaved changes will not save", true);
        }

    }
    public void SaveCharData()
    {
        editorMenu.SaveCharacterData(loadedCharacter);
        
        markedDirty = false;
    }

    public void AddMoreDialogue()
    {
        Creator_DialogueExampleInput newDialogue = ObjectPooler.NewObject(dialogueExPrefab, Vector3.zero, Quaternion.identity).GetComponent<Creator_DialogueExampleInput>();
        newDialogue.transform.parent = dialogueExRoot;
        newDialogue.transform.localPosition = Vector3.zero;
        newDialogue.transform.localRotation = Quaternion.identity;
        newDialogue.transform.localScale = Vector3.one;
        newDialogue.InitDialogueExample("", this);
        addDialogueButton.transform.SetAsLastSibling();
        addDialogueButton.gameObject.SetActive(dialogueExRoot.childCount - 1 < maxDialogueExamples);
        markedDirty = true;

        addedDialogues.Add(newDialogue);
        StartCoroutine(DelayForceUpdateFitters());

    }
    public void RemoveDialogue(int index)
    {
        
        addedDialogues.RemoveAt(index);
        ObjectPooler.ReturnToPool(dialogueExRoot.GetChild(index).gameObject);
        markedDirty = true;
        addDialogueButton.gameObject.SetActive(dialogueExRoot.childCount < maxDialogueExamples);
        StartCoroutine(DelayForceUpdateFitters());
    }

    private void ClearAttribs()
    {
        markedDirty = false;
        addDialogueButton.transform.parent = transform;
        int childCount = dialogueExRoot.childCount;
        for (int i = 0; i < childCount; i++)
        {
            ObjectPooler.ReturnToPool(dialogueExRoot.GetChild(0).gameObject);
        }
        addDialogueButton.transform.parent = dialogueExRoot;

        charName.Text = charDescription.Text = charPersonality.Text = charAppearenceDescription.Text = creatorInput.Text = "";
        StartCoroutine(DelayForceUpdateFitters());
    }
    private IEnumerator DelayForceUpdateFitters()
    {

        Canvas.ForceUpdateCanvases();
        
        scrollRect.enabled = false;
        dialogueLayout.enabled = false;
        yield return null;
        foreach (ContentSizeFitterEdit fitter in sizeFitters)
        {
            fitter.enabled = false;
        }
        foreach (Creator_DialogueExampleInput dial in addedDialogues)
        {
            dial.ToggleFitters(false);
        }
        dialogueEditor.enabled = false;

        Canvas.ForceUpdateCanvases();
        yield return null;
        foreach (ContentSizeFitterEdit fitter in sizeFitters)
        {
            fitter.enabled = true;
        }
        foreach (Creator_DialogueExampleInput dial in addedDialogues)
        {
            dial.ToggleFitters(true);
        }

        Canvas.ForceUpdateCanvases();
        yield return null;
        dialogueLayout.enabled = true;
        dialogueEditor.enabled = true;
        scrollRect.enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    public void UpdateFitters()
    {
        StartCoroutine(DelayForceUpdateFitters());
    }
        
}

#if UNITY_EDITOR
[CustomEditor(typeof(Creator_SelectedCharacterAttribs))]
public class CreatorAtribInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("UpdateFitters"))
        {
            (target as Creator_SelectedCharacterAttribs).UpdateFitters();
        }
    }
}
#endif