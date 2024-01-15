using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class DialogueUiSystem : ToggleableInGameUI
{

    [Header("Conversation History")]
    public GameObject DialogueDisplayPrefab;
    public GameObject PlayerDialogueDisplayPrefab;
    public Transform DialogueParent;
    [Header("Player Dialogue Input")]
    public GameObject PlayerInputParent;
    public InputFieldTMP_Enter PlayerInput;
    public Button SubmitDialogueButton;
    public Button SpeakInputButton;

    public DialogueBubble_Typing TypingBubble;
    private List<DialogueBubble> bubbles = new List<DialogueBubble>();


    #region Chat History ui
    [Header("Chat History UI")]
    public float LerpHistoryTime;
    public AnimationCurve LerpHistoryCurve;
    public RectTransform MenuParent;
    private bool chatHistoryOpen;
    private Coroutine lerpHistoryCor;
    #endregion

    #region OpenAnim
    [SerializeField] private RectTransform typingBoxRect;
    [SerializeField] private AnimationCurve typingBoxAnimCurve;
    [SerializeField] private float typingBoxAnimTime;
    [SerializeField] private float typingBoxHiddenPos;
    #endregion


    private void Awake()
    {

        DialogueSystem_Main.Instance.StartedDataFetch += TextGenerationStarted;
        DialogueSystem_Main.Instance.DataFetchRes += RecievedGeneratedDialogue;
        TypingBubble.gameObject.SetActive(false);
        ToggleChatHistory(false,true);
        PlayerInput.InputSubmit += PlayerSubmitDialogue;
    }

    public void PlayerSubmitDialogue()
    {
        string playerText = PlayerInput.text;
        DialogueSystem_Main.Instance.AddPlayerDialogue(playerText);
        
        DisplayNewDialogue(GameManager.Instance.PlayerCharacterData, playerText, true);
        PlayerInput.text = "";
    }

    public void DisplayNewDialogue(CharacterData speakingCharData, string spokenDialogue, bool isPlayerSpeaking = false)
    {
        GameObject newBubble = ObjectPooler.NewObject((isPlayerSpeaking ? PlayerDialogueDisplayPrefab : DialogueDisplayPrefab), Vector3.zero, Quaternion.identity);
        newBubble.transform.parent = DialogueParent;
        newBubble.transform.localPosition = Vector3.zero;
        DialogueBubble bub = newBubble.GetComponent<DialogueBubble>();
        bub.SetBubbleColor(speakingCharData.FavouriteColor);
        bub.DialogueInit(speakingCharData, spokenDialogue);
        bubbles.Add(bub);
        TypingBubble.transform.SetAsLastSibling();
        Canvas.ForceUpdateCanvases();
        newBubble.SetActive(false);
        newBubble.SetActive(true);
        newBubble.SetActive(false);
        newBubble.SetActive(true);

    }

    private void TextGenerationStarted()
    {
        StartCoroutine(DelayTypingBubble());
    }

    private IEnumerator DelayTypingBubble()
    {
        yield return new WaitForSeconds(Random.Range(1f, 1.6f));
        TypingBubble.gameObject.SetActive(true);
    }
    private void RecievedGeneratedDialogue(CharacterData charData, string addedText)
    {
        TypingBubble.gameObject.SetActive(false);
        DisplayNewDialogue(charData, addedText);
    }

    public void ClearConversation()
    {
        for (int i = bubbles.Count - 1; i >= 0; i--)
        {
            ObjectPooler.ReturnToPool(bubbles[i].gameObject);
        }
        bubbles.Clear();
    }


    public override bool ToggleMenu()
    {
        if (!base.ToggleMenu()) return false;

            StartCoroutine(ToggleTypingBox(isOpen));
            ToggleChatHistory(isOpen, false);

        return true;
    }

    public override bool CloseMenu()
    {
        if (!base.CloseMenu()) return false;
        StartCoroutine(ToggleTypingBox(isOpen));
        ToggleChatHistory(isOpen, false);
        return true;
    }

    public void ResetConversation()
    {
        ClearConversation();
        DialogueSystem_Main.Instance.EndConversation();
    }

    public override void ForceClose()
    {
        typingBoxRect.anchoredPosition = new Vector2(typingBoxRect.anchoredPosition.x, typingBoxHiddenPos);
        ToggleChatHistory(false, true);
        base.ForceClose();
    }

    private IEnumerator ToggleTypingBox(bool enable)
    {
        float timer = 0;
        while (timer < typingBoxAnimTime)
        {
            timer += Time.deltaTime;
            typingBoxRect.anchoredPosition = new Vector2(typingBoxRect.anchoredPosition.x,
                Mathf.LerpUnclamped(typingBoxHiddenPos, 0, typingBoxAnimCurve.Evaluate(enable ? (timer / typingBoxAnimTime) : 1 - (timer / typingBoxAnimTime))));
            yield return null;
        }
        typingBoxRect.anchoredPosition = new Vector2(typingBoxRect.anchoredPosition.x, enable ? 0 : typingBoxHiddenPos);
        yield return null;
        if(!enable) ForceClose();
    }
    /// <summary>
    /// Called from the unity button event;
    /// </summary>
    public void ToggleChatHistory(bool enable, bool forceClosed)
    {
        chatHistoryOpen = enable;
        if (forceClosed)
        {
            chatHistoryOpen = false;
            LerpHistoryMenu(0);
            return;
        }
        if (lerpHistoryCor == null)
        {
            lerpHistoryCor = StartCoroutine(LerpHistoryAnim(chatHistoryOpen ? 0 : LerpHistoryTime));
        }

    }

    private void LerpHistoryMenu(float openPercent)
    {
        float newX = Mathf.Lerp(MenuParent.sizeDelta.x, 0, LerpHistoryCurve.Evaluate(openPercent));
        MenuParent.anchoredPosition = new Vector2(newX, MenuParent.anchoredPosition.y);
    }

    private IEnumerator LerpHistoryAnim(float startTime)
    {
        float timer = startTime;
        bool performAnim = true;
        while (performAnim)
        {
            if (chatHistoryOpen)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer -= Time.deltaTime;
            }
            timer = Mathf.Clamp(timer, 0, LerpHistoryTime);
            LerpHistoryMenu(timer / LerpHistoryTime);
            yield return null;
            if (timer <= 0 || timer >= LerpHistoryTime) performAnim = false;

        }
        lerpHistoryCor = null;
    }

    public void BTNCloseMenu()
    {
        InGameUIManager.Instance.CloseMenu(InGameUIManager.InGameUIType.Dialogue);
    }
}


