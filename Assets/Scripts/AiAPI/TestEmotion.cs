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
        if (Input.GetKeyDown(KeyCode.L))
        {
            TestSerialize();
        }
    }

    private void TestSerialize()
    {
        Dictionary<string, float> dic = new Dictionary<string, float>();
        dic.Add("Happy", 0.3f);
        dic.Add("Sad", 0.3f);
        dic.Add("Angry", 0.3f);
        dic.Add("Scared", 0.3f);
        Debug.Log(JsonConvert.SerializeObject(dic));

        
    }
}
