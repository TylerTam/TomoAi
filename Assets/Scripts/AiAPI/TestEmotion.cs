using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class TestEmotion : MonoBehaviour
{

    public string text;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine( ServerLink.Instance.TestGetEmotion(text));
        }
    }
}
