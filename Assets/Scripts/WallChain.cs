using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallChain : MonoBehaviour
{
    private LineRenderer _line;
    private LineRenderer line { get { if (_line == null) _line = GetComponent<LineRenderer> (); return _line; } }

    private Wall[] walls;

    private void OnDrawGizmos ()
    {
        InitialiseChain ();
        SetLinePositions ();
    }

    private void Awake ()
    {
        InitialiseChain ();
        SetLinePositions ();
    }

    private void InitialiseChain ()
    {
        walls = GetComponentsInChildren<Wall> ();

        for (int i = 0; i < walls.Length; i++)
        {
            Wall prev = i > 0 ? walls[i - 1] : null;
            Wall next = i < walls.Length - 1 ? walls[i + 1] : null;
            walls[i].SetConnected (prev, next);
        }
    }

    private void SetLinePositions ()
    {
        List<Vector3> wallPoints = new List<Vector3> ();

        foreach (Wall wall in walls)
            wallPoints.Add (wall.transform.position);

        line.positionCount = wallPoints.Count;
        line.SetPositions (wallPoints.ToArray ());
    }
}
