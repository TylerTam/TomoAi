using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creator_InputSection : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField inputField;
    private Creator_SelectedCharacterAttribs attribManager;
    public string Text
    {
        get
        {
            return inputField.text;
        }
        set
        {
            inputField.text = value;
        }
    }
    public void Init(string inputText, Creator_SelectedCharacterAttribs attrib)
    {
        inputField.text = inputText;
        attribManager = attrib;
    }

    public void TextChanged()
    {
        attribManager.MarkDirty();
    }
}
