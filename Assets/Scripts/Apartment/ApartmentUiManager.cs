using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentUiManager : MonoBehaviour
{

    [SerializeField] private Transform buttonParent;
    [SerializeField] private ApartmentSelectButton apartmentSelectBtnPrefab;
    [SerializeField] private ApartmentLoader apartmentLoader;
    [SerializeField] private TMPro.TextMeshProUGUI summaryText, btnText;

    [Header("Hide UI Anim")]
    [SerializeField] private RectTransform uiContainer;
    [SerializeField] private CanvasGroup uiCg;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float animTime;

    [SerializeField] private RectTransform buttonContainer;
    [SerializeField] private AnimationCurve buttonContainerCurve;
    [SerializeField] private CanvasGroup exitAparmentCg;
    private void Start()
    {
        LoadButtons();
        SetText("", "Enter Apartment");
    }
    public void SetText(string descriptionText, string buttonText)
    {
        summaryText.text = descriptionText;
        btnText.text = buttonText;
    }
    private void LoadButtons()
    {
        List<CharacterData> chars = GameManager.Instance.SaveLoader.LoadedData.SavedCharacters;
        for (int i = 0; i < chars.Count; i++)
        {
            ApartmentSelectButton newBtn = Instantiate(apartmentSelectBtnPrefab).GetComponentInChildren<ApartmentSelectButton>();
            newBtn.transform.parent = buttonParent;
            newBtn.transform.localPosition = Vector3.zero;
            newBtn.transform.localRotation = Quaternion.identity;
            newBtn.transform.localScale = Vector3.one;
            newBtn.InitButton(chars[i], this);
        }
    }
    public void ExitToAreaSelect()
    {
        PlayerController.Instance.ToggleInteractionInput(false);
        FindObjectOfType<SceneLoader>().LoadScene(SceneLoader.SceneNames.SceneSelect);
    }
    public void LoadApartment(CharacterData charData)
    {
        apartmentLoader.LoadApartment(charData);
    }

    public void SelectApartment()
    {
        StartCoroutine(ToggleUi(false));
        apartmentLoader.CheckAiHome(delegate { CameraManager.Instance.OpenApartmentCam(); });
    }

    public void OpenSelectMenu()
    {
        InGameUIManager.Instance.OpenMenu(InGameUIManager.InGameUIType.None);
        DialogueSystem_Main.Instance.EndConversation();
        CameraManager.Instance.SwitchCam(CameraManager.CamType.FarCam);
        StartCoroutine(ToggleUi(true));
    }

    private IEnumerator ToggleUi(bool enable)
    {
        uiCg.interactable = false;
        exitAparmentCg.interactable = false;
        exitAparmentCg.gameObject.SetActive(true);
        uiCg.gameObject.SetActive(true);
        float timer = 0;
        float percent = 0;
        while (timer < animTime)
        {
            timer += Time.deltaTime;
            yield return null;
            percent = timer / animTime;
            if (enable) percent = 1 - percent;
            uiContainer.anchoredPosition = new Vector2(animCurve.Evaluate(percent) * -uiContainer.sizeDelta.x, uiContainer.anchoredPosition.y);
            buttonContainer.anchoredPosition = new Vector2(buttonContainer.anchoredPosition.x, buttonContainerCurve.Evaluate(percent) * -buttonContainer.sizeDelta.y);
        }
        percent = enable ? 0 : 1;

        uiContainer.anchoredPosition = new Vector2(animCurve.Evaluate(percent) * -uiContainer.sizeDelta.x, uiContainer.anchoredPosition.y);
        buttonContainer.anchoredPosition = new Vector2(buttonContainer.anchoredPosition.x, buttonContainerCurve.Evaluate(percent) * -buttonContainer.sizeDelta.y);
        uiCg.gameObject.SetActive(enable);
        uiCg.interactable = enable;
        exitAparmentCg.gameObject.SetActive(!enable);
        exitAparmentCg.interactable = !enable;
    }
}
