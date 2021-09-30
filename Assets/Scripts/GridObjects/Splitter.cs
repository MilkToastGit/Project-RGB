using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Splitter : GridObject
{
    public override GridObjectType Type => GridObjectType.Splitter;

    public override void ReceiveBeam (int direction, LightBeam beam)
    {
        LightBeam[] components = beam.GetComponents ();
        Debug.Log (components.Length);
        if (components.Length == 1)
            components = new LightBeam[] { components[0], components[0] };

        if (components.Length == 2)
        {
            IsoGrid.Instance.CastBeam (GridPosition.x, GridPosition.y, direction - 1, components[0]);
            IsoGrid.Instance.CastBeam (GridPosition.x, GridPosition.y, direction + 1, components[1]);
        }
        else if (components.Length == 3)
        {
            IsoGrid.Instance.CastBeam (GridPosition.x, GridPosition.y, direction - 1, components[0]);
            IsoGrid.Instance.CastBeam (GridPosition.x, GridPosition.y, direction, components[1]);
            IsoGrid.Instance.CastBeam (GridPosition.x, GridPosition.y, direction + 1, components[2]);
        }
    }
}
