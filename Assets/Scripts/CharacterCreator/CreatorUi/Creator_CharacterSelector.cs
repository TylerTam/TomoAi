using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creator_CharacterSelector : MonoBehaviour
{
    [SerializeField] private CreatorManager creatorManager;
    [SerializeField] private GameObject selectCharacterButtonPrefab;
    [SerializeField] private Transform selectorButtonParent;

    
    [SerializeField] private Creator_SelectedCharacterAttribs SelectedCharacterAttribs;

    
    public TomoCharPerson TomoCharPreview;
    [SerializeField] private TMPro.TextMeshProUGUI characterNameText;
    [SerializeField] private TMPro.TextMeshProUGUI creatorText;


    public void PopulateCharacters(int forceIndex = 0, bool selectFinalIndex = false)
    {
        PopulateSelectorUI();
        if (!selectFinalIndex)
        {
            SelectCharacter(forceIndex);
        }
        else
        {
            SelectCharacter(selectorButtonParent.childCount - 1);
        }
    }
    private void ClearExistingSelectButtons()
    {
        int childCount = selectorButtonParent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            ObjectPooler.ReturnToPool(selectorButtonParent.GetChild(0).gameObject);
        }
    }
    private void PopulateSelectorUI(int forceLoadIndex = 0)
    {
        ClearExistingSelectButtons();
        List<CharacterData> allSavedCharacters = GameManager.Instance.SaveLoader.LoadedData.SavedCharacters;
        for (int i = 0; i < allSavedCharacters.Count; i++)
        {
            Creator_SelectorButton btn = ObjectPooler.NewObject(selectCharacterButtonPrefab, Vector3.zero, Quaternion.identity).GetComponent<Creator_SelectorButton>();
            btn.transform.SetParent(selectorButtonParent);
            btn.transform.localPosition = Vector3.zero;
            btn.transform.localScale = Vector3.one;
            btn.transform.localRotation = Quaternion.identity;
            btn.InitButton(allSavedCharacters[i], this, i);
        }

    }
    public void SelectCharacter(int selectCharIndex)
    {
        if(selectorButtonParent.childCount > selectCharIndex)
        {
            selectorButtonParent.GetChild(selectCharIndex).GetComponent<Creator_SelectorButton>().SelectCharacter();
        }
    }
    public void SelectCharacter(CharacterData selectChar, int selectedCharIndex)
    {

        creatorManager.SetSelectedChar( selectChar, selectedCharIndex);
        characterNameText.text = selectChar.Name;
        creatorText.text = "Creator: " + selectChar.Creator;
    }
}
