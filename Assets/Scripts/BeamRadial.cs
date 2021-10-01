using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamRadial
{
    private LightBeam[] beams = new LightBeam[8];

    public LightBeam this[int direction]
    {
        get { return beams[Tools.Wrap (direction, 8)]; }
        set { beams[Tools.Wrap (direction, 8)] = value; }
    }

    public LightBeam Up => beams[0];
    public LightBeam UpRight => beams[1];
    public LightBeam Right => beams[2];
    public LightBeam DownRight => beams[3];
    public LightBeam Down => beams[4];
    public LightBeam DownLeft => beams[5];
    public LightBeam Left => beams[6];
    public LightBeam UpLeft => beams[7];

    public int Count
    {
        get
        {
            int count = 0;
            foreach (LightBeam beam in beams)
                if (beam != null) count++;

            return count;
        }
    }
}
