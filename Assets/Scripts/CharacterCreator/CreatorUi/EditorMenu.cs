using UnityEngine;

public class EditorMenu : MonoBehaviour
{
    [SerializeField] private Creator_SelectedCharacterAttribs characterDataEditor;

    private CharacterData charData;
    int savedCharacterIndex;

    public void OpenEditor(CharacterData loadCharData, int selectedCharacterIndex)
    {
        charData = loadCharData;
        characterDataEditor.LoadTomoChar(charData);
        savedCharacterIndex = selectedCharacterIndex;
    }

    public void SaveCharacterData(CharacterData saveCharData)
    {
        charData = saveCharData;
        //Debug.Log(savedCharacterIndex);
        GameManager.Instance.SaveLoader.UpdateCharacter(charData, false, savedCharacterIndex);
    }
}
