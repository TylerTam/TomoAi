using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelationshipBar : MonoBehaviour
{
    [SerializeField] private GameObject titleBar;
    [SerializeField] private TMPro.TextMeshProUGUI titleText;
    [SerializeField] private GameObject personBar;
    [SerializeField] private TMPro.TextMeshProUGUI personName, relationshipStatus;
    [SerializeField] private Image barImage;

    public void InitTitleBar(RelationshipMatrix.RelationShipType relationshipType)
    {
        titleBar.SetActive(true);
        personBar.SetActive(false);
        titleText.text = relationshipType.ToString();
    }

    public void InitRelationBar(CharacterRelationShip relation)
    {
        titleBar.SetActive(false);
        personBar.SetActive(true);
        personName.text = relation.characterName;
        relationshipStatus.text = GameManager.Instance.RelationshipMatrix.GetBarText(relation.relationshipType, relation.relationshipStatus);
        personName.color = relationshipStatus.color = GameManager.Instance.RelationshipMatrix.GetColor(relation.relationshipStatus);
    }
    public void SetBarColor(Color col)
    {
        barImage.color = col;
    }
}
