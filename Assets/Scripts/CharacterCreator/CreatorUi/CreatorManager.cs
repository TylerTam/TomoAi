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
    [SerializeField] private GameObject SelectorPreviewRoot;
    [SerializeField] private GameObject EditorRoot;
    public CharacterData SelectedCharacter;

    private void Start()
    {
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
                EditorRoot.SetActive(false);
                SelectorRoot.gameObject.SetActive(true);
                SelectorPreviewRoot.SetActive(true);
                break;
            case CreatorMenu.Editor:
                EditorRoot.SetActive(true);
                SelectorRoot.gameObject.SetActive(false);
                SelectorPreviewRoot.SetActive(false);
                break;

        }
    }
}
