using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamRadial : IEnumerable<LightBeam>
{
    private LightBeam[] beams = new LightBeam[8];
    
    public bool isEmpty
    {
        get
        {
            foreach (LightBeam beam in beams)
                if (beam != null) return false;

            return true;
        }
    }
    public LightBeam Up => beams[0];
    public LightBeam UpRight => beams[1];
    public LightBeam Right => beams[2];
    public LightBeam DownRight => beams[3];
    public LightBeam Down => beams[4];
    public LightBeam DownLeft => beams[5];
    public LightBeam Left => beams[6];
    public LightBeam UpLeft => beams[7];

    public LightBeam this[int direction]
    {
        get { return beams[Tools.Wrap (direction, 8)]; }
        set 
        {
            direction = Tools.Wrap (direction, 8);
            beams[direction] = value.Direction == direction ? value : value.SetDirection (direction); 
        }
    }

    public LightBeam[] GetBeams ()
    {
        List<LightBeam> activeBeams = new List<LightBeam> ();
        foreach (LightBeam beam in beams)
            activeBeams.Add (beam);

        return activeBeams.ToArray ();
    }
    
    public LightBeam[] Clear ()
    {
        for (int dir = 0; dir < 8; dir++)
            beams[dir] = null;

        return beams;
    }

    public void Remove (LightBeam beam)
    {
        for (int dir = 0; dir < 8; dir++)
        {
            if (beams[dir] == beam)
                beam = null;
        }
    }

    public void Add (LightBeam beam) => this[beam.Direction] = beam;

    public void AddCombine (LightBeam beam) => this[beam.Direction] += beam;

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

    public IEnumerator<LightBeam> GetEnumerator ()
    {
        foreach (LightBeam beam in beams)
        {
            if (beam != null)
                yield return beam;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator ();
    }
}
