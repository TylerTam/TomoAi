using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor.UI;
using UnityEditor;
#endif
public class ApartmentSelectButton : Button
{
    private ApartmentUiManager uiManager;
    private CharacterData charData;

    [SerializeField] private Image profileImage;
    [SerializeField] private TMPro.TextMeshProUGUI charNameText;

    public void InitButton(CharacterData charData, ApartmentUiManager uiManager)
    {
        this.uiManager = uiManager;
        this.charData = charData;
        charNameText.text = charData.Name;
    }
    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        SelectApartment();
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        SelectApartment();
    }

    private void SelectApartment()
    {
        uiManager.LoadApartment(charData);

    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ApartmentSelectButton))]
public class ApartmentSelectButtonInspector : ButtonEditor
{
    SerializedProperty profileImage;
    SerializedProperty charNameText;
    public override void OnInspectorGUI()
    {
        profileImage = serializedObject.FindProperty("profileImage");
        charNameText = serializedObject.FindProperty("charNameText");
        EditorGUILayout.PropertyField(profileImage);
        EditorGUILayout.PropertyField(charNameText);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}
#endif