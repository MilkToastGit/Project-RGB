using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof (GridObjectMono))]
public class GridObjectMonoEditor : Editor
{
    GridObjectMono obj;

    private void OnEnable ()
    {
        obj = target as GridObjectMono;
    }

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI ();

        if (obj.GridObject == null || obj.GridObject.Type != obj.Type)
        {
            switch (obj.Type)
            {
                case GridObjectType.Emitter:
                    obj.GridObject = new Emitter ();
                    break;
                case GridObjectType.Splitter:
                    obj.GridObject = new Splitter ();
                    break;
            }
            obj.SnapToGrid ();
        }

        obj.GridObject.DrawAndSetParams ();

        if (obj.transform.hasChanged)
            obj.SnapToGrid ();
    }

    private void SetGridObjectPosition ()
    {
        if (obj.GridObject == null) return;

        obj.GridObject.SetPosition (IsoGrid.Instance.WorldToGrid (obj.transform.position, false));
        obj.gameObject.transform.position = IsoGrid.Instance.GridToWorld (obj.GridObject.GridPosition);
        obj.transform.hasChanged = false;
    }
}