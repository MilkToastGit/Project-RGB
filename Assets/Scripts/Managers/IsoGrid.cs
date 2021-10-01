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
    private GridObject[] gridObjects;
    [SerializeField]
    private int maxPropagationSteps;

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

    private void Awake ()
    {
        GenerateGrid ();
        if (!InstantiatePersistentScene.loaded)
            InstantiatePersistentScene.OnManagersLoaded += SubscribeToEvents;
        else
            SubscribeToEvents ();
    }

    private void SubscribeToEvents ()
    {
        InstantiatePersistentScene.OnManagersLoaded -= SubscribeToEvents;
    }

    private void GenerateGrid (Vector2Int size)
    {
        grid = new GridObject[size.x, size.y];
        gridWidth = (size.x - 1) * gridSpacing.x;
        gridHeight = (size.y - 1) * gridSpacing.y;

        foreach (GridObject obj in gridObjects)
        {
            if (obj != null)
                grid[obj.GridPosition.x, obj.GridPosition.y] = obj;
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

    public void Reallocate (GridObject obj, Vector2Int destination)
    {
        grid[obj.GridPosition.x, obj.GridPosition.y] = null;
        grid[destination.x, destination.y] = obj;
        obj.SetPosition (destination);

        PropagateBeams ();
    }

    // I don't really know what propagate means but it sounds cool here
    private void PropagateBeams ()
    {
        print ("Beginning Propagation");
        print ("Step One");

        // Step 1
        EventManager.Instance.GridObjectMoved ();
        
        print ("Step Two");
        // Step 2
        for (int i = 0; i < maxPropagationSteps && EventManager.Instance.AllBeamsTerminatedHasListeners; i++)
        {
            print ($"Propagating step {i}, remaining listeners? {EventManager.Instance.AllBeamsTerminatedHasListeners}");
            EventManager.Instance.AllBeamsTerminated ();
        }

        print ("Rendering");
        BeamRenderer.Instance.Render ();
        EventManager.Instance.AllBeamsRendered ();

        print ("Propagation Complete");
    }

    public bool GridCast (Vector2Int origin, int direction, out Vector2Int hitPoint)
    {
        Vector2Int lastPoint = origin;
        for (int i = 0; i < Mathf.Max (gridSize.x, gridSize.y); i++)
        {
            Vector2Int point = TranslatePoint (lastPoint, direction);
            if (!point.x.isBetween (0, gridSize.x - 1) || !point.y.isBetween (0, gridSize.y - 1))
            {
                print ($"point ({point.x}, {point.y}) is out of bounds");
                hitPoint = lastPoint;
                return false;
            }
            if (GetAt (point) != null)
            {
                print ($"point ({point.x}, {point.y}) contains object ({GetAt (point)})");
                hitPoint = point;
                return true;
            }

            lastPoint = point;
        }

        hitPoint = lastPoint;
        return false;
    }

    public Vector2Int CastBeam (LightBeam beam)
    {
        Debug.Log ("Start " + beam.Color);
        if (beam.Origin == null) throw new System.Exception ("Cannot cast beam with no origin.");

        if (GridCast (beam.Origin, beam.Direction, out Vector2Int hitPoint))
            GetAt (hitPoint)?.ReceiveBeam (beam);

        //Debug.DrawLine (GridToWorld (beam.Origin.x, beam.Origin.y), GridToWorld (hitPoint), beam.Color, 0.5f);
        Debug.Log ("End " + beam.Color);
        return hitPoint;
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

    public Vector2 SnapToGrid (Vector2 position, bool restrainToWorkingArea = true) 
    {
        Vector2Int nearestToPosition = WorldToGrid (position);

        //Get nearest avaialable if 
        if (GetAt (nearestToPosition) != null)
        {
            for (int y = 0; y < nearestToPosition.y + 1; y++)
            {
                float closestDistance = -1;
                Vector2Int closestPoint;
                for (int x = 0; x < nearestToPosition.x; x++)
                {
                    if (x == nearestToPosition.x || y == nearestToPosition.y) continue;

                    Vector2 displacement = position - GridToWorld (x, y);
                    float sqrDistance = Mathf.Pow (displacement.x, 2) + Mathf.Pow (displacement.y, 2);
                    if (closestDistance > 0 && sqrDistance < closestDistance)
                    {
                        closestDistance = sqrDistance;
                        closestPoint = new Vector2Int (x, y);
                    }
                }
            }

            //nearestToPosition
        }
        return GridToWorld (WorldToGrid (position, restrainToWorkingArea)); 
    }

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

    private GridObject GetAt (Vector2Int position) => grid[position.x, position.y];
    private GridObject SetAt (Vector2Int position, GridObject obj) => grid[position.x, position.y] = obj;
}
