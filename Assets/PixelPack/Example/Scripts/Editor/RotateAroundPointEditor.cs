/**
 *  Tool for editing RotateAroundPoint center points.
 */

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RotateAroundPoint)),CanEditMultipleObjects]

public class RotateAroundPointEditor : Editor
{
    private float handleSize = .1f;
    private Vector3 snap = Vector3.one * 0.04f;
    private GUIStyle labelStyle = new GUIStyle();
    private RotateAroundPoint targetObject;

    private Vector3 objectWorldPos = Vector3.zero;

    private Vector3 rotationCenter = new Vector3();
    public virtual void OnEnable()
    {
        labelStyle.fontSize = 20;
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.padding = new RectOffset(0, 0, 0, 0);
        labelStyle.contentOffset = Vector2.zero;
        labelStyle.normal.textColor = Color.white;

        targetObject = (RotateAroundPoint)target;
        
        rotationCenter = targetObject.RotationCenterPoint;  
    }

    protected virtual void OnSceneGUI()
    {
        //--------------- SET HANDLE COLOR AND (OBJECT'S) LOCAL MATRIX ------------

        Handles.color = Handles.yAxisColor;

        //if (!Application.isPlaying)
        Handles.matrix = targetObject.transform.localToWorldMatrix;

        //--------------- DRAW HANDLES AND LABELS ------------

        //Begin check, draw handles and labels
        EditorGUI.BeginChangeCheck();

        //TODO: is snap broken in this Unity version?         
        rotationCenter = Handles.FreeMoveHandle(targetObject.RotationCenterPoint, Quaternion.identity, handleSize, snap, Handles.SphereHandleCap);

        Handles.Label(rotationCenter + new Vector3(0f, -0.1f, 0f), rotationCenter.ToString(), labelStyle);
        
        //End check, update RotateAroundPoint RotationCenterPoint value
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(targetObject, "Change Center Point Handle"); //Record undo            
            targetObject.RotationCenterPoint = rotationCenter;
        }
    }
}
