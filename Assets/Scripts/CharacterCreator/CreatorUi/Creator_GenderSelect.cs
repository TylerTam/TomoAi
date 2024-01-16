using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creator_GenderSelect : MonoBehaviour
{
    [SerializeField] private bool singleGender = false;
    [SerializeField] private RotaryHeart.Lib.SerializableDictionary.SerializableDictionaryBase<CharacterData.Gender, GenderButton> genderButtons;
    [System.Serializable]
    private class GenderButton
    {
        public GameObject checkMark;
        public bool isSelected;

        public void ToggleButton(bool enable)
        {
            isSelected = enable;
            checkMark.SetActive(enable);
        }
    }


    public void InitGender(CharacterData.Gender selectedGenders)
    {
        foreach (KeyValuePair<CharacterData.Gender, GenderButton> keyValuePair in genderButtons)
        {

            keyValuePair.Value.ToggleButton(selectedGenders.HasFlag(keyValuePair.Key));
        }
    }

    public void ToggleButton(int charButton)
    {
        CharacterData.Gender gender;
        switch (charButton)
        {
            case 0: gender = CharacterData.Gender.Male; break;
            case 1: gender = CharacterData.Gender.Female; break;
            case 2: gender = CharacterData.Gender.Other; break;
            default: gender = CharacterData.Gender.None; break;
        }
        if (singleGender)
        {
            foreach (KeyValuePair<CharacterData.Gender, GenderButton> keyValuePair in genderButtons)
            {
                keyValuePair.Value.ToggleButton(false);
            }
        }
        genderButtons[gender].ToggleButton(!genderButtons[gender].isSelected);
    }

    public CharacterData.Gender GetGender()
    {
        CharacterData.Gender retGender = CharacterData.Gender.None;
        foreach (KeyValuePair<CharacterData.Gender, GenderButton> keyValuePair in genderButtons)
        {
            if (keyValuePair.Value.isSelected)
            {
                if(retGender == CharacterData.Gender.None)
                {
                    retGender = keyValuePair.Key;
                }
                else
                {
                    retGender = retGender | keyValuePair.Key;
                }
            }
        }

        return retGender;
    }
}
