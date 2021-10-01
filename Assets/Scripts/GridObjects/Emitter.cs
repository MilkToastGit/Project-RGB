using UnityEditor;

[System.Serializable]
public class Emitter : GridObject
{
    public override GridObjectType Type => GridObjectType.Emitter;

    public int direction;
    public LightBeam beam;

    private void EmitBeam ()
    {
        beam.Cast (GridPosition, direction);
    }

    public override void OnManagersLoaded ()
    {
        base.OnManagersLoaded ();
        EventManager.Instance.OnGridObjectMoved += EmitBeam;
    }

    public override void OnDisable ()
    {
        base.OnDisable ();
        EventManager.Instance.OnGridObjectMoved -= EmitBeam;
    }
}
