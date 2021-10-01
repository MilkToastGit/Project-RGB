using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Splitter : GridObject
{
    public override GridObjectType Type => GridObjectType.Splitter;

    public override void ReceiveBeam (LightBeam beam)
    {
        LightBeam[] components = beam.GetComponents ();
        if (components.Length == 1)
            components = new LightBeam[] { components[0], components[0] };

        if (components.Length == 2)
        {
            components[0].Cast (GridPosition, beam.Direction - 1);
            components[1].Cast (GridPosition, beam.Direction + 1);
        }
        else if (components.Length == 3)
        {
            for (int i = 0; i < 3; i++)
                components[i].Cast (GridPosition, beam.Direction + i - 1);
        }
    }
}
