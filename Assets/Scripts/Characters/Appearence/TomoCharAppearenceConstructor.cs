using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomoCharAppearenceConstructor : MonoBehaviour
{
    [SerializeField] protected AppearenceSlot GeneralHeadSlot;
    [SerializeField] protected AppearenceSlot LeftEyeSlot;
    [SerializeField] protected AppearenceSlot RightEyeSlot;
    [SerializeField] protected AppearenceSlot MouthSlot;

    public TomoCharOutfit currentOutfit;
    public void ConstructCharacter( CharacterData charData)
    {
        GeneralHeadSlot.Init(charData.CharacterAppearence.HeadSlot);
        LeftEyeSlot.Init(charData.CharacterAppearence.Eyes);
        RightEyeSlot.Init(charData.CharacterAppearence.Eyes);
        MouthSlot.Init(charData.CharacterAppearence.MouthSlot);

        //Do the clothes
        currentOutfit.InitOutfit(charData);
    }
    
}
