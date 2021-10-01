using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof (GridObjectMono))]
public class GridObjectMonoEditor : Editor
{
    //GridObjectMono obj;

    private void OnEnable ()
    {
        //obj = target as GridObjectMono;
    }

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI ();

        GridObjectMono obj = target as GridObjectMono;

        if (obj.GridObject == null || obj.GridObject.Type != obj.Type)
        {
            obj.SetGridObject ();
            obj.SnapToGrid (false);
        }

        //obj.GridObject.DrawAndSetParams ();

        if (obj.transform.hasChanged)
        {
            obj.SnapToGrid (false);
            obj.transform.hasChanged = false;
        }
    }
}