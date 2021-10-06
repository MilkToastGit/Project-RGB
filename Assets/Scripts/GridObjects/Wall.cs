using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : GridObject
{
    public override GridObjectType Type => GridObjectType.Wall;

    public override void ReceiveBeam (LightBeam beam) { }
}
