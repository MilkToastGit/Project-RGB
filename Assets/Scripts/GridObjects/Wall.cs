using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : GridObject
{
    public override GridObjectType Type => GridObjectType.Wall;

    private Wall prevWall, nextWall;

    public void SetConnected (Wall previous, Wall next)
    {
        prevWall = previous;
        nextWall = next;
    }

    //public bool IsConnected (Wall wall) => prevWall == wall || nextWall == wall;
    public bool IsConnected (Wall wall)
    {
        print ($"prevWall match {prevWall == wall}, nextWall match {nextWall == wall}");
        return prevWall == wall || nextWall == wall;
    }

    public override void ReceiveBeam (LightBeam beam) { }
}
