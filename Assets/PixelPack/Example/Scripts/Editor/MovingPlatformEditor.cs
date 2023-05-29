/**
 *  Tool for editing MovingPlatform waypoints.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(MovingPlatform))]

public class MovingPlatformEditor : Editor
{
    private float handleSize = .1f;
    private Vector3 snap = Vector3.one * 0.1f;
    private GUIStyle labelStyle = new GUIStyle();
    private MovingPlatform targetPlatform;

    private Vector3 objectWorldPos = Vector3.zero;

    private List<Vector3> targetPositions = new List<Vector3>();
    public virtual void OnEnable()
    {
        Debug.Log("Init MovingPlatformEditor");
        labelStyle.fontSize = 20;
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.padding = new RectOffset(0, 0, 0, 0);
        labelStyle.contentOffset = Vector2.zero;

        labelStyle.normal.textColor = Color.white;

        targetPlatform = (MovingPlatform)target;
        targetPositions = targetPlatform.MovePoints;

        targetPositions.ForEach(pos => pos = targetPlatform.transform.position + pos);

        objectWorldPos = targetPlatform.transform.localPosition;

        
    }
    protected virtual void OnSceneGUI()
    {
        //--------------- SET HANDLE COLOR AND (OBJECT'S) LOCAL MATRIX ------------

        Handles.color = Handles.yAxisColor;
        Handles.matrix = targetPlatform.transform.localToWorldMatrix;

        //--------------- DRAW HANDLES AND LABELS ------------

        //Begin check, draw handles and labels
        EditorGUI.BeginChangeCheck();
        for (int i = 0; i < targetPlatform.MovePoints.Count; i++)
        {
            targetPositions[i] = Handles.FreeMoveHandle(targetPlatform.MovePoints[i], Quaternion.identity, handleSize, snap, Handles.SphereHandleCap);
            Handles.Label(targetPositions[i] + new Vector3(0f, -0.1f, 0f), (i + 1).ToString(), labelStyle); 
        }

        //End check, update MovingPlatform MovePoints list
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(targetPlatform, "Change Move Point Handles"); //Record undo
            targetPlatform.MovePoints = targetPositions;            
        }
    }
}
