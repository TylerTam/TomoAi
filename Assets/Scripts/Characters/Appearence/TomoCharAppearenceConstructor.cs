using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomoCharAppearenceConstructor : MonoBehaviour
{
    [SerializeField] protected AppearenceSlot GeneralHeadSlot;
    [SerializeField] protected AppearenceSlot LeftEyeSlot;
    [SerializeField] protected AppearenceSlot RightEyeSlot;
    [SerializeField] protected AppearenceSlot MouthSlot;


    public void ConstructCharacter(CharacterAppearance appearenceData)
    {
        GeneralHeadSlot.Init(appearenceData.HeadSlot);
        LeftEyeSlot.Init(appearenceData.Eyes);
        RightEyeSlot.Init(appearenceData.Eyes);
        MouthSlot.Init(appearenceData.MouthSlot);

    }
    
}
