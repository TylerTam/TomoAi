using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CreatorManager : MonoBehaviour
{
    public enum CreatorMenu
    {
        Selector,
        Editor
    }

    [SerializeField] private Creator_CharacterSelector SelectorRoot;
    [SerializeField] private EditorMenu EditorRoot;
    [SerializeField]private CharacterData SelectedCharacter;
    [SerializeField]private int selectedCharIndex;


    public void SetSelectedChar(CharacterData charData, int charIndex)
    {
        SelectedCharacter = charData;
        selectedCharIndex = charIndex;
    }
    private void Start()
    {
        ChangeMenu(0);
        SelectorRoot.PopulateCharacters(0);
    }
    public void ChangeMenu(int menuType)
    {
        if(menuType == 0)
        {
            ChangeMenu(CreatorMenu.Selector);
        }else if (menuType == 1)
        {
            ChangeMenu(CreatorMenu.Editor);
        }
    }
    public void ChangeMenu(CreatorMenu newMenu)
    {
        switch (newMenu)
        {
            case CreatorMenu.Selector:
                EditorRoot.gameObject.SetActive(false);
                SelectorRoot.gameObject.SetActive(true);
                break;
            case CreatorMenu.Editor:
                EditorRoot.gameObject.SetActive(true);
                EditorRoot.OpenEditor(SelectedCharacter, selectedCharIndex);
                SelectorRoot.gameObject.SetActive(false);
                break;
        }
    }
}
