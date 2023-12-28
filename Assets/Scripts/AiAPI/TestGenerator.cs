using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    public string prompt = "oi";
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ServerLink.Instance.StartGenerator(prompt, "Tyler", Response);
        }
    }

    private void Response(bool success, string speakingChar, string response)
    {
        Debug.Log("Success: " + success + " | Char: " + speakingChar + " | Res: " + response);
    }
#endif
}
