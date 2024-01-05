using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSelectManager : MonoBehaviour
{

    public static AreaSelectManager Instance;
    private SceneLoader.SceneNames selectedArea;

    [SerializeField] private TMPro.TextMeshProUGUI buttonAreaName;
    [Header("Animation")]
    [SerializeField] private RectTransform buttonRect;
    [SerializeField] private float hiddenYPos;
    [SerializeField] private float animTime;
    [SerializeField] private AnimationCurve animCurve;
    private void Awake()
    {
        Instance = this;
    }
    public void AreaPressed(SceneLoader.SceneNames area)
    {

        selectedArea = area;
        buttonAreaName.text = area.ToString();
        StartCoroutine(ToggleButton(true));
    }
    public void CancelSelect()
    {
        selectedArea = SceneLoader.SceneNames.None;
        StartCoroutine(ToggleButton(false));
        PlayerController.Instance.ToggleInteractionInput(true);
        CameraManager.Instance.SwitchCam(CameraManager.CamType.FarCam,false);

    }
    public void EnterArea()
    {
        FindObjectOfType<SceneLoader>().LoadScene(selectedArea);
    }

    private IEnumerator ToggleButton(bool enable)
    {
        buttonRect.gameObject.SetActive(true);
        float timer = 0;
        while (timer < animTime)
        {
            timer += Time.deltaTime;
            buttonRect.anchoredPosition = new Vector2(buttonRect.anchoredPosition.x, Mathf.LerpUnclamped(hiddenYPos, 0, animCurve.Evaluate(enable ? (timer / animTime) : 1 - (timer / animTime))));
            yield return null;
        }

        buttonRect.anchoredPosition = new Vector2(buttonRect.anchoredPosition.x, enable ? 0 : hiddenYPos);
        buttonRect.gameObject.SetActive(enable);
    }
}
