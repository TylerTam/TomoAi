using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Profiling.Memory.Experimental;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class ContentSizeFitterEdit : ContentSizeFitter
{
    public float MinLength;
    public float MaxLength;
    public float MinHeight;
    public float MaxHeight;
    public float LengthAdditive;
    public float HeightAdditive;

    public override void SetLayoutHorizontal()
    {
        base.SetLayoutHorizontal();
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(Mathf.Clamp(rect.sizeDelta.x, MinLength, MaxLength) + LengthAdditive, Mathf.Clamp(rect.sizeDelta.y, MinHeight, MaxHeight) + HeightAdditive);
    }
    public override void SetLayoutVertical()
    {
        base.SetLayoutVertical();
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(Mathf.Clamp(rect.sizeDelta.x, MinLength, MaxLength) + LengthAdditive, Mathf.Clamp(rect.sizeDelta.y, MinHeight, MaxHeight) + HeightAdditive);
    }
    public void OnEnable()
    {
        SetLayoutHorizontal();
        SetLayoutVertical();
        Canvas.ForceUpdateCanvases();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ContentSizeFitterEdit))]
public class ContentFitterEditInspector : Editor
{

}
#endif
