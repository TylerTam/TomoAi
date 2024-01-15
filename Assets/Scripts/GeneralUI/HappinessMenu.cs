using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappinessMenu : ToggleableInGameUI
{
    [SerializeField] private UnityEngine.UI.Image emotionImage;
    [SerializeField] private RotaryHeart.Lib.SerializableDictionary.SerializableDictionaryBase<EmotionAnalysis.Emotion, EmotionUi> emotionUi;
    private TomoCharEmotions targetTomoChar;

    [System.Serializable]
    private struct EmotionUi
    {
        public Sprite EmotionSprite;
        public Color EmotionColor;
    }


    [Header("Animation")]
    [SerializeField] private RectTransform menuRectTrans;
    [SerializeField] private float hiddenPos;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float animTime;


    public override bool ToggleMenu()
    {
        return base.ToggleMenu();
    }
    public override bool ToggleMenu(TomoCharPerson tomoChar)
    {
        if (!base.ToggleMenu(tomoChar)) return false;


        if (isOpen)
        {
            targetTomoChar = tomoChar.GetComponent<TomoCharEmotions>();
            targetTomoChar.MainEmotionUpdated += UpdateEmotion;
            UpdateEmotion();
            OpenMenu();
        }
        else
        {
            if(targetTomoChar != null)
            {
                targetTomoChar.MainEmotionUpdated -= UpdateEmotion;
            }
            StartCoroutine(ShowHideAnim(false));
        }


        return true;
    }

    private void UpdateEmotion()
    {
        EmotionUi emotion = emotionUi[targetTomoChar.MainEmotion];
        emotionImage.sprite = emotion.EmotionSprite;
        emotionImage.color = emotion.EmotionColor;
    }
    public override bool CloseMenu()
    {
        if (!base.CloseMenu()) return false;
        StartCoroutine(ShowHideAnim(false));
        return true;
    }
    public override void ForceClose()
    {
        base.ForceClose();
        StopAllCoroutines();
        menuRectTrans.anchoredPosition = new Vector2(hiddenPos, menuRectTrans.anchoredPosition.y);
        gameObject.SetActive(false);
    }

    private void OpenMenu()
    {
        
        StartCoroutine(ShowHideAnim(true));
    }
    private IEnumerator ShowHideAnim(bool enable)
    {
        float timer = 0;
        while (timer < animTime)
        {
            timer += Time.deltaTime;

            menuRectTrans.anchoredPosition = new Vector2(Mathf.LerpUnclamped(hiddenPos, 0, animCurve.Evaluate(enable ? (timer / animTime) : 1 - (timer / animTime))), menuRectTrans.anchoredPosition.y);

            yield return null;
        }
        menuRectTrans.anchoredPosition = new Vector2(enable ? 0 : hiddenPos, menuRectTrans.anchoredPosition.y);
        if (!enable) gameObject.SetActive(false);
    }


    //Called from button
    public void ExitMenu()
    {
        InGameUIManager.Instance.CloseMenu(InGameUIManager.InGameUIType.Relationship);
    }
}
