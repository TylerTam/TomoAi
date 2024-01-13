using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class ServerLink : MonoBehaviour
{
    public static ServerLink Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<ServerLink>();
                if(_instance == null)
                {
                    _instance = new GameObject("ServerLink").AddComponent<ServerLink>();
                }
            }
            return _instance;
            
        }
    }
    private static ServerLink _instance;
    private string serverURL
    {
        get { return GameManager.Instance.SaveLoader.LoadedSettings.apiUrl + "/api"; }
    } 
    private string eaUrl//emotional analysis
    {
        get { return GameManager.Instance.SaveLoader.LoadedSettings.emotionAnalysisUrl + "/getemotion/"; }
    } 

    private string csrfToken;
    private bool isCalling = false;
    private bool isConnected = false;
    [SerializeField] private bool shouldConnect = true;
    [SerializeField] private bool debugPrompt = true;

    public static GeneratorCleaner sentenceCleaner;

    public enum GenerationType
    {
        Default,
        SingleLine,
        TestFast
    }
    [SerializeField] private RotaryHeart.Lib.SerializableDictionary.SerializableDictionaryBase<GenerationType, GeneratorSettings> generationTypes;
    
    public string CleanString(string inputString)
    {
        return sentenceCleaner.CleanSentence(inputString);
    }
    public GeneratorSettings GetGenSettType (GenerationType genType)
    {
        return generationTypes[genType];
    }
    private void Awake()
    {
        //if (shouldConnect)
        //{
        //    StartCoroutine(LoginToServer());
        //}
        isConnected = true;
        sentenceCleaner = new GeneratorCleaner();
    }

    public void StartGenerator(string prompt,int speakingCharId, double aiTemperature, System.Action<bool, int, string, EmotionAnalysis> resonseAction, GenerationType genType = GenerationType.Default)
    {
        if (!isCalling)
        {

        }
        StartCoroutine(GenerateText(prompt, speakingCharId, aiTemperature, resonseAction, genType));
    }

    public IEnumerator GenerateText(string inputPrompt, int speakingCharId, double aiTemperature, System.Action<bool, int, string, EmotionAnalysis> response,  GenerationType genType = GenerationType.Default, bool debugResponse = false)
    {

        GenerationSettings settings = generationTypes[genType].generationSettings.Clone();
        settings.prompt = inputPrompt;
        settings.temperature = aiTemperature;
#if UNITY_EDITOR
        if (debugPrompt) Debug.Log(settings.prompt);
#endif
        if (!isConnected)
        {
            if (!shouldConnect)
            {
                yield return new WaitForSeconds(2f);
                response.Invoke(false, speakingCharId, "Not Connected", null);
                
                yield break;
            }
            yield return LoginToServer();
            if (!isConnected)
            {
                yield return new WaitForSeconds(2f);
                response.Invoke(false, speakingCharId, "Not Connected", null);
                yield break;
            }
        }


        string data = JsonUtility.ToJson(settings);
        yield return null;

        isCalling = true;


        using (UnityWebRequest wr = new UnityWebRequest(serverURL + "/v1/generate", "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(data);
            wr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            wr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            wr.SetRequestHeader("Content-Type", "application/json");
            yield return wr.SendWebRequest();


            switch (wr.result)
            {
                case UnityWebRequest.Result.Success:
                    Debug.Log("Success");
                    if (debugResponse)
                    {
                        Debug.Log(wr.downloadHandler.text);
                    }

                    ServerResponse res = JsonUtility.FromJson<ServerResponse>(wr.downloadHandler.text);
                    string scentence = sentenceCleaner.CleanSentence(res.results[0].text);

                    EmotionAnalysis analysis = new EmotionAnalysis();
                    using (UnityWebRequest emotionReq = UnityWebRequest.Get(eaUrl + scentence))
                    {
                        emotionReq.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                        yield return emotionReq.SendWebRequest();
                        if (emotionReq.result != UnityWebRequest.Result.Success)
                        {
                            Debug.Log("Couldn't connect to server");
                            yield break;
                        }
                        
                        switch (emotionReq.result)
                        {
                            case UnityWebRequest.Result.Success:
                                analysis = sentenceCleaner.CleanEmotion(emotionReq.downloadHandler.text);
                                break;
                            default:
                                Debug.LogWarning("Emotional Analysis api not connected");
                                break;
                        }
                       
                    }
                    response?.Invoke(true, speakingCharId, scentence, analysis);
                    

                    break;
                default:
                    Debug.Log("Error: " + wr.responseCode);
                    response?.Invoke(false, speakingCharId, "oops", null);
                    
                    break;
            }
            isCalling = false;
        }

    }


    private IEnumerator LoginToServer()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(serverURL + "csrf-token/"))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Couldn't connect to server");
                yield break;
            }
            string SetCookie = request.GetResponseHeader("set-cookie");
            Regex rxCookie = new Regex("X-CSRF-Token=(?<csrf_token>.{64});");
            MatchCollection cookieMatches = rxCookie.Matches(SetCookie);
            csrfToken = cookieMatches[0].Groups["csrf_token"].Value;

            if (request.result == UnityWebRequest.Result.Success)
            {
                isConnected = true;
                Debug.Log("<color=yellow>Logged in.</color> Token: " + csrfToken);
            }
            else
            {
                Debug.Log("Error: " + request.responseCode);
            }
        }
        yield return null;
    }

    

    [System.Serializable]
    public struct ServerResponse
    {
        [SerializeField] public TextResponse[] results;

        [System.Serializable]
        public struct TextResponse
        {
            [SerializeField] public string text;
        }
    }

    public IEnumerator TestGetEmotion(string text)
    {
        using (UnityWebRequest emotionReq = UnityWebRequest.Get(eaUrl + text))
        {
            emotionReq.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            yield return emotionReq.SendWebRequest();
            if (emotionReq.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Couldn't connect to server");
                yield break;
            }
            if (emotionReq.result == UnityWebRequest.Result.Success)
            {
                isConnected = true;
                sentenceCleaner.CleanEmotion(emotionReq.downloadHandler.text);
                
                //EmotionAnalysis newEmotions = JsonUtility.FromJson<EmotionAnalysis>(serializeScore);
                //EmotionAnalysis newEmotions = JsonConvert.DeserializeObject<EmotionAnalysis>(serializeScore);
                
            }
            else
            {
                Debug.Log("Error: " + emotionReq.responseCode);
            }
        }
    }

}
