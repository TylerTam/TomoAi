using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TomoCharUi : MonoBehaviour
{
    [SerializeField] private Transform uiRoot;
    [SerializeField] private CanvasGroup uiCg;
    [SerializeField] private UiBubbles uiBubbles;
    [System.Serializable]
    private struct UiBubbles
    {
        public GameObject ThinkingBubbleRoot;
        public GameObject SpeechBubbleRoot;
        public TMPro.TextMeshProUGUI SpeechBubbleName;
        public TMPro.TextMeshProUGUI SpeechBubbleDialogue;
        
    }

    private string tempDialogueText;
    private string tempCharName;
    

    [Header("Anim Props")]
    [SerializeField] private float animLength;
    [SerializeField] private AnimationCurve xSizeCurve, ySizeCurve;
    [SerializeField] private float showBubbleDuration;
    [SerializeField] private float fadeBubbleDuration;

    [SerializeField] private List<Image> promptImages;
    [SerializeField] private float timePerLetter = 0.05f;
    public enum UiState
    {
        None,
        Thinking,
        SpeechBubble
    }
    private UiState currentUi;
    private void Awake()
    {
        SetUiScale(0);
    }

    public void SetBubbleColors(Color userColor)
    {
        foreach(Image img in promptImages)
        {
            img.color = userColor;
        }
    }
    public void SetNextSpeechText(CharacterData charData, string dialogue)
    {
        tempCharName = charData.Name;
        tempDialogueText = dialogue;
    }
    public void DisplayUi(UiState newUiState, System.Action completeAction = null, System.Action dialogueSpokenAction = null)
    {

        StopAllCoroutines();
        //UiState prevUi = currentUi;
        currentUi = newUiState;
        if(currentUi != UiState.None)
        {
            if (currentUi == UiState.Thinking)
            {
                StartCoroutine(HideCurrentUi(delegate { ShowNewBubble(); StartCoroutine(ShowBubbleAnim(delegate { completeAction?.Invoke();})); }));
            }
            else
            {
                StartCoroutine(HideCurrentUi(delegate { ShowNewBubble(); StartCoroutine(ShowBubbleAnim(delegate { completeAction?.Invoke(); StartCoroutine(HideBubbleAnim(true, dialogueSpokenAction)); })); }));
            }
        }
        else
        {
            ShowNewBubble();
            StartCoroutine(ShowBubbleAnim(delegate { completeAction?.Invoke(); StartCoroutine(HideBubbleAnim()); }));
        }
        
    }

    private void ShowNewBubble()
    {
        uiCg.alpha = 1;
        uiBubbles.ThinkingBubbleRoot.SetActive(false);
        uiBubbles.SpeechBubbleRoot.SetActive(false);
        uiRoot.localScale = new Vector3(0, 0, 1);
        switch (currentUi)
        {
            case UiState.None:
                break;
            case UiState.Thinking:
                uiBubbles.ThinkingBubbleRoot.SetActive(true);
                break;
            case UiState.SpeechBubble:
                uiBubbles.SpeechBubbleName.text = tempCharName;
                uiBubbles.SpeechBubbleDialogue.text = tempDialogueText;
                uiBubbles.SpeechBubbleRoot.SetActive(true);
                uiBubbles.SpeechBubbleRoot.SetActive(false);
                uiBubbles.SpeechBubbleRoot.SetActive(true);

                break;
        }
    }

    private IEnumerator HideCurrentUi(System.Action returnAction)
    {
        yield return StartCoroutine(HideBubbleAnim(false));
        returnAction?.Invoke();
    }

    private IEnumerator ShowBubbleAnim(System.Action completeAction)
    {
        bool playAnim = true;
        float timer = 0;
        while (playAnim)
        {
            timer += Time.deltaTime;
            timer = Mathf.Clamp(timer, 0, animLength);
            SetUiScale(timer / animLength);
            
            yield return null;
            if (timer >= animLength) playAnim = false;
        }
        completeAction?.Invoke();
    }
    private IEnumerator HideBubbleAnim(bool waitForDisplayTime = true, System.Action dialogueSaid = null)
    {
        if (waitForDisplayTime)
        {
            float showBubbleTime = showBubbleDuration;
            if (currentUi == UiState.SpeechBubble)
            {
                showBubbleTime += (uiBubbles.SpeechBubbleDialogue.text.Length * timePerLetter);
                
            }
            yield return new WaitForSeconds(showBubbleTime);
        }
        float timer = 0;
        while(timer < fadeBubbleDuration)
        {
            timer += Time.deltaTime;
            yield return null;
            uiCg.alpha = Mathf.Clamp((1 - timer / fadeBubbleDuration),0,1);
        }
        uiCg.alpha = 0;
        uiRoot.localScale = new Vector3(0, 0, 1);
        dialogueSaid?.Invoke();
    }
    private void SetUiScale(float percent)
    {
        float xScale = Mathf.LerpUnclamped(0, 1, xSizeCurve.Evaluate(percent));
        float yScale = Mathf.LerpUnclamped(0, 1, ySizeCurve.Evaluate(percent));
        uiRoot.localScale = new Vector3(xScale, yScale, 1);
    }
}
