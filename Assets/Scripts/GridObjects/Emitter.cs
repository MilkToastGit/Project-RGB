using UnityEditor;

public class Emitter : GridObject
{
    public override GridObjectType Type => GridObjectType.Emitter;
    public int direction;
    public LightBeam beam;

    public override void DrawAndSetParams ()
    {
        direction = EditorGUILayout.IntField ("Direction: ", direction);
        beam = new LightBeam (
            EditorGUILayout.Toggle ("R: ", beam == null ? true : beam.R),
            EditorGUILayout.Toggle ("G: ", beam == null ? true : beam.G),
            EditorGUILayout.Toggle ("B: ", beam == null ? true : beam.B)
            );
    }

    private void EmitBeam ()
    {
        IsoGrid.Instance.CastBeam (GridPosition.x, GridPosition.y, direction, beam);
    }

    private void OnEnable ()
    {
        EventManager.Instance.OnGridChanged += EmitBeam;
    }

    private void OnDisable ()
    {
        EventManager.Instance.OnGridChanged -= EmitBeam;
    }
}
