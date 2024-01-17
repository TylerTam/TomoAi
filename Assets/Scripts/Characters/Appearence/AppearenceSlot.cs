using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearenceSlot : MonoBehaviour
{
    protected enum AppearenceSlotType
    {
        General,
        Mouth,
        LeftEye,
        RightEye
    }
    [SerializeField] protected AppearenceSlotType SlotType;

    private List<GameObject> childObjects = new List<GameObject>();
    
    public virtual void Init(CharacterAppearance.AppearenceSlot slotData)
    {
        
        switch (SlotType)
        {
            case AppearenceSlotType.General:
            case AppearenceSlotType.Mouth:
                transform.localPosition = new Vector3(slotData.slotPosition.x, slotData.slotPosition.y, 0);
                transform.localScale = new Vector3(slotData.slotScale.x, slotData.slotScale.y, 1);
                CharacterAppearance.SlotWithPieces pieces = (CharacterAppearance.SlotWithPieces)(slotData);
                foreach (CharacterAppearance.AppearancePieces piece in pieces.AppearancePieces)
                {
                    AddPiece(piece);
                }
                break;

            case AppearenceSlotType.LeftEye:
                CharacterAppearance.EyesSlot lEyeSlot = (CharacterAppearance.EyesSlot)slotData;
                transform.localPosition = new Vector3(-lEyeSlot.slotPosition.x, lEyeSlot.slotPosition.y, 0);
                transform.localScale = new Vector3(-lEyeSlot.slotScale.x, lEyeSlot.slotScale.y, 1);
                foreach (CharacterAppearance.AppearancePieces piece in lEyeSlot.LeftEye.AttachedEyePieces)
                {
                    AddPiece(piece);
                }
                break;
            case AppearenceSlotType.RightEye:
                CharacterAppearance.EyesSlot rEyeSlot = (CharacterAppearance.EyesSlot)slotData;
                transform.localPosition = new Vector3(rEyeSlot.slotPosition.x, rEyeSlot.slotPosition.y, 0);
                transform.localScale = new Vector3(rEyeSlot.slotScale.x, rEyeSlot.slotScale.y, 1);
                foreach (CharacterAppearance.AppearancePieces piece in rEyeSlot.RightEye.AttachedEyePieces)
                {
                    AddPiece(piece);
                }
                break;
        }
    }


    private void AddPiece(CharacterAppearance.AppearancePieces pieceData)
    {
        GameObject newPiece = GameManager.Instance.AllAppearenceItems.GetAppearenceItem(pieceData.ItemId); 
        
        newPiece.transform.parent = transform;
        newPiece.transform.localPosition = pieceData.PiecePos;
        newPiece.transform.localEulerAngles = new Vector3(0, 0, pieceData.PieceZRot);
        newPiece.transform.localScale = new Vector3(pieceData.PieceScale.x, pieceData.PieceScale.y, 1);
        PieceAdded(newPiece);
    }
    protected virtual void PieceAdded(GameObject piece)
    {
        childObjects.Add(piece);
    }
    protected virtual void ClearAppearence()
    {
        for (int i = childObjects.Count-1; i >=0; i--)
        {
            ObjectPooler.ReturnToPool(childObjects[i]);
        }
    }
}
