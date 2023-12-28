using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Creator_SelectorButton : Button
{

    [SerializeField] private Image CharacterImage;
    [SerializeField] private TMPro.TextMeshProUGUI CharacterName;
    private CharacterData characterData;
    private Creator_CharacterSelector selector;
    public void InitButton(CharacterData charData, Creator_CharacterSelector characterSelector)
    {
        characterData = charData;
        CharacterName.text = charData.Name;
        //CharacterImage.sprite = null;
        selector = characterSelector;
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        selector.SelectCharacter(characterData);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        selector.SelectCharacter(characterData);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Creator_SelectorButton))]
public class CreatorSelectorButtonInspector : ButtonEditor
{
    private SerializedProperty characterImage;
    private SerializedProperty characterName;

    protected override void OnEnable()
    {
        base.OnEnable();
        characterImage = serializedObject.FindProperty("CharacterImage");
        characterName = serializedObject.FindProperty("CharacterName");
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(characterImage);
        EditorGUILayout.PropertyField(characterName);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}
#endif
