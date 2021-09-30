using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IsoGrid : Singleton<IsoGrid>
{
    [SerializeField]
    private Vector2Int gridSize;
    [SerializeField]
    private Vector2Int workingArea;
    [SerializeField]
    private Vector2 gridSpacing;
    [SerializeField]
    private GridObjectMono[] gridObjects;

    [SerializeField]
    private GridObject[,] grid;
    [SerializeField]
    [HideInInspector]
    private float gridWidth, gridHeight;

    [ContextMenu ("GenerateGrid")]
    private void GenerateGrid ()
    {
        GenerateGrid (gridSize);
    }

    [ContextMenu ("CastBeamTest")]
    public void CastBeamTest () { GenerateGrid ();  CastBeam (0, 3, 2, new LightBeam (1, 1, 1)); }

    private void GenerateGrid (Vector2Int size)
    {
        grid = new GridObject[size.x, size.y];
        gridWidth = (size.x - 1) * gridSpacing.x;
        gridHeight = (size.y - 1) * gridSpacing.y;

        foreach (GridObjectMono obj in gridObjects)
        {
            if (obj.GridObject != null)
                grid[obj.GridObject.GridPosition.x, obj.GridObject.GridPosition.y] = obj.GridObject;
        }

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Debug.DrawRay (GridToWorld (x, y), Vector2.up * 0.1f, Color.blue, 60);
                //Debug.DrawLine (GetGridPointPosition (x, y), GetGridPointPosition (GetPointInDirection (x, y, 1)), Color.grey, 60);
                //Debug.DrawLine (GetGridPointPosition (x, y), GetGridPointPosition (GetPointInDirection (x, y, 3)), Color.grey, 60);
                //Debug.DrawLine (GetGridPointPosition (x, y), GetGridPointPosition (GetPointInDirection (x, y, 5)), Color.grey, 60);
                //Debug.DrawLine (GetGridPointPosition (x, y), GetGridPointPosition (GetPointInDirection (x, y, 7)), Color.grey, 60);
            }
        }
    }

    public bool GridCast (int x, int y, int direction, out Vector2Int hitPoint)
    {
        Vector2Int lastPoint = new Vector2Int (x, y);
        for (int i = 0; i < Mathf.Max (gridSize.x, gridSize.y); i++)
        {
            Vector2Int point = TranslatePoint (lastPoint, direction);
            if (!point.x.isBetween (0, gridSize.x - 1) || !point.y.isBetween (0, gridSize.y - 1))
            {
                hitPoint = new Vector2Int (lastPoint.x, lastPoint.y);
                return false;
            }
            if (grid[point.x, point.y] != null)
            {
                hitPoint = new Vector2Int (point.x, point.y);
                return true;
            }

            lastPoint = point;
        }

        hitPoint = lastPoint;
        return false;
    }

    public void CastBeam (int x, int y, int direction, LightBeam beam)
    {
        if (GridCast (x, y, direction, out Vector2Int hitPoint))
            grid[hitPoint.x, hitPoint.y].ReceiveBeam (direction, beam);

        Debug.DrawLine (GridToWorld (x, y), GridToWorld (hitPoint), beam.Color, 20);
    }

    public Vector2 GridToWorld (Vector2Int point) { return GridToWorld (point.x, point.y); }
    public Vector2 GridToWorld (int x, int y)
    {
        float posX = x * gridSpacing.x + y % 2 * gridSpacing.x / 2 - (gridWidth + gridSpacing.x / 2) / 2;
        float posY = gridHeight / 2 - y * gridSpacing.y;

        return new Vector2 (posX, posY);
    }

    public Vector2Int WorldToGrid (Vector2 position, bool restrainToWorkingArea = true)
    {
        int y = Mathf.RoundToInt ((gridHeight / 2 - position.y) / gridSpacing.y);
        int x = Mathf.RoundToInt ((position.x - y % 2 * gridSpacing.x / 2 + (gridWidth + gridSpacing.x / 2) / 2) / gridSpacing.x);

        if (restrainToWorkingArea)
        {
            Vector2Int margin = new Vector2Int ((gridSize.x - workingArea.x) / 2, (gridSize.y - workingArea.y) / 2);
            x = Mathf.Clamp (x, margin.x, gridSize.x - margin.x);
            y = Mathf.Clamp (y, margin.y, gridSize.y - margin.y);
        }
        else
        {
            x = Mathf.Clamp (x, 0, gridSize.x - 1);
            y = Mathf.Clamp (y, 0, gridSize.y - 1);
        }

        return new Vector2Int (x, y);
    }

    public Vector2 SnapToGrid (Vector2 position, bool restrainToWorkingArea = true) { return GridToWorld (WorldToGrid (position, restrainToWorkingArea)); }

    private Vector2Int TranslatePoint (Vector2Int origin, int direction) { return TranslatePoint (origin.x, origin.y, direction); }
    private Vector2Int TranslatePoint (int xOrigin, int yOrigin, int direction)
    {
        int x = xOrigin;
        int y = yOrigin;

        direction = Tools.Wrap (direction, 8);

        switch (direction)
        {
            case 0:
                y -= 2;
                break;
            case 1:
                x += y % 2;
                y -= 1;
                break;
            case 2:
                x += 1;
                break;
            case 3:
                x += y % 2;
                y += 1;
                break;
            case 4:
                y += 2;
                break;
            case 5:
                x -= 1 - y % 2;
                y += 1;
                break;
            case 6:
                x -= 1;
                break;
            case 7:
                x -= 1 - y % 2;
                y -= 1;
                break;
        }
        Debug.Log ($"{x}, {y}");

        return new Vector2Int (x, y);

        //y += 2 * Mathf.Abs (((direction / 2 - 2) % 4) - 2) - 2;

        //switch (direction)
        //{
        //    case 1:
        //    case 3:
        //        x += y % 2;
        //        break;
        //    case 2:
        //    case 6:
        //        x += 1;
        //        break;
        //    case 5:
        //    case 7:
        //        x -= 1 - y % 2;
        //        break;
        //}
    }
}
