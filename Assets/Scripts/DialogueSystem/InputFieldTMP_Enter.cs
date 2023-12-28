using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFieldTMP_Enter : TMPro.TMP_InputField
{
    public System.Action InputSubmit;
    protected override void Awake()
    {
        base.Awake();
        onEndEdit.AddListener(CheckForEnter);
    }

    private void CheckForEnter(string endEditstring)
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            InputSubmit?.Invoke();
        }
    }
}
