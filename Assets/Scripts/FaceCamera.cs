using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class FaceCamera : MonoBehaviour
{
    [SerializeField] private bool sameRotAsTarget;
#if UNITY_EDITOR
    [SerializeField] private Transform debugTransform;

    public void FaceTransform()
    {
        if (sameRotAsTarget)
        {
            transform.rotation = debugTransform.rotation;
        }
        else
        {
            transform.LookAt(debugTransform.position);
        }
    }
#endif

    private void LateUpdate()
    {
        if (sameRotAsTarget)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
        else
        {

            transform.LookAt(Camera.main.transform.position);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FaceCamera))]
public class FaceCamInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if(!Application.isPlaying)
        {
            if(GUILayout.Button("Face Transform"))
            {
                (target as FaceCamera).FaceTransform();
            }
        }
        base.OnInspectorGUI();
    }
}
#endif
