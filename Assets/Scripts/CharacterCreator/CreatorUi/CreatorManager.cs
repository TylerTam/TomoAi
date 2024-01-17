using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CreatorManager : AreaLoader
{
    public enum CreatorMenu
    {
        Selector,
        Editor
    }

    [SerializeField] private GameObject createNewCharMenu;
    [SerializeField] private Creator_CharacterSelector SelectorRoot;
    [SerializeField] private EditorMenu EditorRoot;
    [SerializeField]private CharacterData SelectedCharacter;
    [SerializeField]private int selectedCharIndex;
    [SerializeField] private TomoCharPerson tomoChar;
    [SerializeField] private TMPro.TextMeshProUGUI deleteCharNameText;

    [Header("Cameras")]
    [SerializeField] private GameObject selectorCamera;
    [SerializeField] private GameObject attributeCamera;
    [SerializeField] private TMPro.TMP_InputField newCharName;

    


    public void ReturnToAreaSelect()
    {
        GameTick.Instance.ToggleTickSave(true);
        FindObjectOfType<SceneLoader>().LoadScene(SceneLoader.SceneNames.SceneSelect);
    }
    public void SetSelectedChar(CharacterData charData, int charIndex)
    {
        SelectedCharacter = charData;
        selectedCharIndex = charIndex;
        deleteCharNameText.text = charData.Name;
        tomoChar.ConstructTomoChar(charData);
    }
    private void Start()
    {
        ChangeMenu(0);
        SelectorRoot.PopulateCharacters(0);
    }
    public void ChangeMenu(int menuType)
    {
        createNewCharMenu.SetActive(false);
        if (menuType == 0)
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
                SelectorRoot.PopulateCharacters(selectedCharIndex);
                EditorRoot.gameObject.SetActive(false);
                SelectorRoot.gameObject.SetActive(true);
                selectorCamera.SetActive(true);
                attributeCamera.SetActive(false);
                break;
            case CreatorMenu.Editor:
                EditorRoot.gameObject.SetActive(true);
                EditorRoot.OpenEditor(SelectedCharacter, selectedCharIndex);
                SelectorRoot.gameObject.SetActive(false);
                attributeCamera.SetActive(true);
                selectorCamera.SetActive(false);
                break;
        }
    }

    public void PreviewText()
    {

    }

    public void CreateNewCharacter()
    {
        CharacterData newChar = new CharacterData(newCharName.text, GameManager.Instance.SaveLoader.LoadedData.Player.PlayerName, GameManager.Instance.SaveLoader.GenerateCharacterId());
        newChar.CharGender = CharacterData.Gender.Male;
        newChar.RelationshipPreference = CharacterData.Gender.None;
        newChar.FavouriteColor = Color.white;
        newChar.UpdateForNewVersion();
        GameManager.Instance.SaveLoader.UpdateCharacter(newChar, true, -1);
        SelectorRoot.PopulateCharacters(0, true);
        newCharName.text = "";
        ChangeMenu(1);
        GameTick.Instance.ForceTick();
        GameTick.Instance.NpcPlacements.ForceNpcIntoArea(newChar.LocalId, NpcPlacements.AreaType.Apartments, newChar.LocalId);
    }

    public void ConfirmDelete()
    {
        GameManager.Instance.SaveLoader.DeleteCharacter(SelectedCharacter.LocalId);
        ChangeMenu(0);
    }
    public override void LoadArea()
    {
        StartCoroutine(DelayTickDisable());
    }

    private IEnumerator DelayTickDisable()
    {
        yield return new WaitForSeconds(.3f);
        GameTick.Instance.ToggleTickSave(false);
    }
}
