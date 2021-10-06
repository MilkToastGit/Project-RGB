using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (GridManager))]
public class GridManagerEditor : Editor
{
    GridManager obj;

    private void OnEnable ()
    {
        obj = target as GridManager;
    }

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI ();

        if (GUILayout.Button ("GENERATE"))
        {
            obj.GenerateGrid ();
        }
    }
}
