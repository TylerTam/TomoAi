using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public enum SceneNames
    {
        None=0,
        SceneSelect=1,
        Apartments=2,
        Restaurant=3,
        CityHall=4,
    }

    [SerializeField] private RotaryHeart.Lib.SerializableDictionary.SerializableDictionaryBase<SceneNames, string> allScenes = new RotaryHeart.Lib.SerializableDictionary.SerializableDictionaryBase<SceneNames, string>();
    [SerializeField] private SceneNames currentLoadedScene;

    [Header("Scene Load Animation")]

    [SerializeField] private GameObject sceneLoadCanvas;
    [SerializeField] private RectTransform sceneLoadContainer;
    [SerializeField] private float hiddenYPosition;
    [SerializeField] private float animTime;
    [SerializeField] private AnimationCurve animCurve;
    private bool isLoadingScene = false;
    private void Awake()
    {
        StartCoroutine(HideLoaderScreen());
    }
    public void LoadScene(SceneNames newScene)
    {
        if (isLoadingScene) return;
        isLoadingScene = true;
        StartCoroutine(LoadScene(allScenes[newScene]));
    }

    private IEnumerator HideLoaderScreen()
    {
        sceneLoadCanvas.SetActive(true);
        float timer = 0;
        while (timer < animTime)
        {
            timer += Time.deltaTime;
            sceneLoadContainer.anchoredPosition = new Vector2(sceneLoadContainer.anchoredPosition.x, Mathf.LerpUnclamped(0, hiddenYPosition, animCurve.Evaluate(timer / animTime)));
            yield return null;
        }
        sceneLoadContainer.anchoredPosition = new Vector2(sceneLoadContainer.anchoredPosition.x, hiddenYPosition);
        sceneLoadCanvas.SetActive(false);
    }
    private IEnumerator LoadScene(string sceneName)
    {
        GameTick.Instance.ToggleTickSave(false);
        sceneLoadCanvas.SetActive(true);
        float timer = 0;
        while(timer < animTime)
        {
            timer += Time.deltaTime;
            sceneLoadContainer.anchoredPosition = new Vector2(sceneLoadContainer.anchoredPosition.x, Mathf.LerpUnclamped(hiddenYPosition, 0, animCurve.Evaluate(timer / animTime)));
            yield return null;
        }
        sceneLoadContainer.anchoredPosition = new Vector2(sceneLoadContainer.anchoredPosition.x,  0);

        yield return SceneManager.LoadSceneAsync(sceneName);

        AreaLoader area = FindObjectOfType<AreaLoader>();
        if (area)
        {
            area.LoadArea();
        }

        yield return new WaitForSeconds(0.2f);

        timer = 0;
        while (timer < animTime)
        {
            timer += Time.deltaTime;
            sceneLoadContainer.anchoredPosition = new Vector2(sceneLoadContainer.anchoredPosition.x, Mathf.LerpUnclamped(0, hiddenYPosition, animCurve.Evaluate(timer / animTime)));
            yield return null;
        }
        sceneLoadContainer.anchoredPosition = new Vector2(sceneLoadContainer.anchoredPosition.x, hiddenYPosition);
        sceneLoadCanvas.SetActive(false);
        isLoadingScene = false;
        GameTick.Instance.ToggleTickSave(true);
    }
}
