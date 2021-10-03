using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Redirector : GridObject.MultiInput
{
    public override GridObjectType Type => GridObjectType.Redirector;
    [SerializeField]
    private int outputDirection;

    protected override void OnEnable()
    {
        base.OnEnable();
        SetRotation (outputDirection);
    }

    protected void OnDrawGizmos ()
    {
        base.OnDrawGizmos ();
        SetRotation (outputDirection);
    }

    protected override BeamRadial GenerateOutputBeams(BeamRadial input)
    {
        BeamRadial output = new BeamRadial();

        foreach (LightBeam beam in input)
        {
            output[outputDirection] += beam;

        }


        return output;
    }
}
