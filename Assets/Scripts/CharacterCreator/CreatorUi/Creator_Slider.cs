using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creator_Slider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMPro.TMP_InputField inputField;


    public double GetIntensity => (slider.value/100);

    public void Init(double val)
    {
        float floatVal = Mathf.Clamp((float)val*100, slider.minValue, slider.maxValue);
        slider.value = floatVal;
        inputField.text = (floatVal / 100).ToString();
    }
    public void TextUpdate(string str)
    {
        float val = 0.97f;
        if(float.TryParse(str, out val))
        {
            val = Mathf.Clamp(val*100, slider.minValue, slider.maxValue);
            slider.value = val;
            inputField.text = (val/100).ToString();
        }
    }

    public void SliderUpdate(float val)
    {
        inputField.text = (val/100).ToString();
    }
}
