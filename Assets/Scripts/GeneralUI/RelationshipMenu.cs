using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationshipMenu : ToggleableInGameUI
{
    [SerializeField] private GameObject barPrefab;
    [SerializeField] private Transform barParent;
    [SerializeField] private List<Color> barColors;
    private CharacterData charData;
    private CharacterData previousChar;
    private List<RelationshipBar> relationshipBars = new List<RelationshipBar>();

    [Header("Animation")]
    [SerializeField] private RectTransform menuRectTrans;
    [SerializeField] private float hiddenPos;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float animTime;
    public override bool ToggleMenu(bool enable)
    {
        return base.ToggleMenu(enable);
    }
    public override bool ToggleMenu(bool enable, TomoCharPerson tomoChar)
    {
        if (!base.ToggleMenu(enable, tomoChar)) return false;


        if (enable)
        {
            charData = tomoChar.CharData;
            OpenMenu();
        }
        else
        {
            StartCoroutine(ShowHideAnim(false));
        }


        return true;
    }
    public override void ForceClose()
    {
        base.ForceClose();
        StopAllCoroutines();
        menuRectTrans.anchoredPosition = new Vector2( hiddenPos, menuRectTrans.anchoredPosition.y);
        gameObject.SetActive(false);
    }

    private void OpenMenu()
    {
        if (previousChar == charData) return;
        previousChar = charData;
        for (int i = 0; i < relationshipBars.Count; i++)
        {
            relationshipBars[i].gameObject.SetActive(false);
        }

        List<CharacterRelationShip> relations = charData.SortRelationships();

        RelationshipMatrix.RelationShipType currentRelationshipCat = RelationshipMatrix.RelationShipType.Themself;
        int barIndex = 0;
        for (int i = 0; i < relations.Count; i++)
        {
            RelationshipBar bar = GetNewBar(barIndex);
            if (i == 0 || currentRelationshipCat != relations[i].relationshipType)
            {
                currentRelationshipCat = relations[i].relationshipType;
                bar.InitTitleBar(currentRelationshipCat);
                bar.SetBarColor(barColors[barIndex % barColors.Count]);
                barIndex++;
                bar = GetNewBar(barIndex);

            }
            bar.InitRelationBar(relations[i]);
            bar.SetBarColor(barColors[barIndex % barColors.Count]);
            barIndex++;
        }
        StartCoroutine(ShowHideAnim(true));
    }
    private RelationshipBar GetNewBar(int index)
    {
        if(index >= relationshipBars.Count)
        {
            RelationshipBar newBar = ObjectPooler.NewObject(barPrefab, Vector3.zero, Quaternion.identity).GetComponent<RelationshipBar>();
            newBar.transform.parent = barParent;
            newBar.transform.localPosition = Vector3.zero;
            newBar.transform.localScale = Vector3.one;
            newBar.transform.localRotation = Quaternion.identity;
            relationshipBars.Add(newBar);
            newBar.gameObject.SetActive(true);
            return newBar;
        }
        relationshipBars[index].gameObject.SetActive(true);
        return relationshipBars[index];
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
        menuRectTrans.anchoredPosition = new Vector2(enable ? 0 : hiddenPos , menuRectTrans.anchoredPosition.y);
        if (!enable) gameObject.SetActive(false);
    }


    //Called from button
    public void ExitMenu()
    {
        InGameUIManager.Instance.CloseMenu(InGameUIManager.InGameUIType.Relationship);
    }
}
