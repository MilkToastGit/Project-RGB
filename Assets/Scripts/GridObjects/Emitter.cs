using UnityEditor;

[System.Serializable]
public class Emitter : GridObject
{
    public override GridObjectType Type => GridObjectType.Emitter;

    [UnityEngine.Range (0, 7)]
    public int direction;
    public LightBeam beam;

    public override void ReceiveBeam (LightBeam beam) { }

    public void EmitBeam ()
    {
        //UnityEngine.Debug.Log("EMITTING");
        beam.Cast (GridPosition, direction);
    }

    protected override void OnManagersLoaded ()
    {
        base.OnManagersLoaded ();
        EventManager.Instance.OnGridObjectMoved += EmitBeam;
    }

    protected override void OnDisable ()
    {
        base.OnDisable ();
        EventManager.Instance.OnGridObjectMoved -= EmitBeam;
    }
}
