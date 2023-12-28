using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

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
    const string serverURL = "http://192.168.50.178:8000/";

    private string csrfToken;
    private bool isCalling = false;
    private bool isConnected = false;
    [SerializeField] private bool shouldConnect = true;

    private GeneratorCleaner sentenceCleaner;

    public enum GenerationType
    {
        Default,
        SingleLine,
    }
    [SerializeField] private RotaryHeart.Lib.SerializableDictionary.SerializableDictionaryBase<GenerationType, GeneratorSettings> generationTypes;
    
    public GeneratorSettings GetGenSettType (GenerationType genType)
    {
        return generationTypes[genType];
    }
    private void Awake()
    {
        if (shouldConnect)
        {
            StartCoroutine(LoginToServer());
        }
        sentenceCleaner = new GeneratorCleaner();
    }

    public void StartGenerator(string prompt,string speakingChar, System.Action<bool, string, string> resonseAction, GenerationType genType = GenerationType.Default)
    {
        StartCoroutine(GenerateText(prompt, speakingChar, resonseAction, genType));
    }

    public IEnumerator GenerateText(string inputPrompt, string speakingChar,System.Action <bool, string, string> response, GenerationType genType = GenerationType.Default, bool debugResponse = false)
    {

        if (!isConnected)
        {
            if (!shouldConnect)
            {
                response.Invoke(false, speakingChar, "Not Connected");
                yield break;
            }
            yield return LoginToServer();
            if (!isConnected)
            {
                response.Invoke(false, speakingChar, "Not Connected");
                yield break;
            }
        }

        GenerationSettings settings = generationTypes[genType].generationSettings.Clone();
        settings.prompt = inputPrompt;
        string data = JsonUtility.ToJson(settings);
        yield return null;

        isCalling = true;


        var uwr = new UnityWebRequest(serverURL + "generate_unity/", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(data);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        yield return uwr.SendWebRequest();


        switch (uwr.result)
        {
            case UnityWebRequest.Result.Success:
                Debug.Log("Success");
                if (debugResponse)
                {
                    Debug.Log(uwr.downloadHandler.text);
                }

                ServerResponse res = JsonUtility.FromJson<ServerResponse>(uwr.downloadHandler.text);
                response?.Invoke(true, speakingChar, sentenceCleaner.CleanSentence(res.results[0].text));
                
                break;
            default:
                Debug.Log("Error: " + uwr.responseCode);
                response?.Invoke(false, speakingChar, "oops");
                break;
        }
        isCalling = false;

    }


    private IEnumerator LoginToServer()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(serverURL + "csrf-token/"))
        {
            yield return request.SendWebRequest();
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
}
