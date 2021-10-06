using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridManager : Singleton<GridManager>
{
    [SerializeField]
    private Vector2Int gridSize;
    [SerializeField]
    private Vector2 gridSpacing;
    [SerializeField]
    private Vector2Int workingArea;
    [SerializeField]
    private int maxPropagationSteps;

    [SerializeField]
    [HideInInspector]
    private IsoGrid grid;

    [ContextMenu ("GenerateGrid")]
    [InitializeOnLoadMethod]
    public void GenerateGrid ()
    {
        grid = new IsoGrid (gridSize, gridSpacing, workingArea, GetComponentsInChildren<GridObject> ());
    }

    private void Awake ()
    {
        GenerateGrid ();
        if (ManagerLoader.loaded)
            OnManagersLoaded ();
        else
            ManagerLoader.OnManagersLoaded += OnManagersLoaded;
    }

    private void OnManagersLoaded ()
    {
        ManagerLoader.OnManagersLoaded -= OnManagersLoaded;
        EventManager.Instance.OnGridObjectMoved += PropagateBeams;
        EventManager.Instance.GridObjectMoved ();
    }

    private void PropagateBeams ()
    {
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

    public Vector2Int CastBeam (LightBeam beam, out bool hitWall, out Vector2 wallHitPoint)
    {
        Debug.Log ("Start " + beam.Colour);
        if (beam.Origin == null) throw new System.Exception ("Cannot cast beam with no origin.");

        if (grid.GridCast (beam.Origin, beam.Direction, out Vector2Int hitPoint, out hitWall, out wallHitPoint))
        {
            if (!hitWall)
                grid[hitPoint]?.ReceiveBeam (beam);
        }

        Debug.Log ("End " + beam.Colour);
        return hitPoint;
    }

    public Vector2 GridToWorld (Vector2Int point) => grid.GridToWorld (point);
    public Vector2 GridToWorld (int x, int y) => grid.GridToWorld (x, y);

    public Vector2Int WorldToGrid (Vector2 position, bool restrainToWorking = true) => grid.WorldToGrid (position, restrainToWorking);

    public void SnapToGrid (GridObject obj, Vector2 position, bool restrainToWorking = true)
    {
        grid.SnapToGrid (obj, position, restrainToWorking);
    }

    public Vector2 SnapToGrid (Vector2 position, Vector2Int originalPosition, bool restrainToWorking = true)
    {
        return grid.GridToWorld (grid.NearestAvailablePoint (position, originalPosition, restrainToWorking));
    }

    public void MoveObject (GridObject obj, Vector2Int destination) => grid.MoveObject (obj, destination);
}

[System.Serializable]
public class IsoGrid
{
    [SerializeField] private GridObject[,] grid;
    [SerializeField] private Vector2Int gridSize, workingMin, workingMax;
    [SerializeField] private Vector2 worldSize, spacing;

    public GridObject this[Vector2Int i]
    {
        get { return grid[i.x, i.y]; }
        set { grid[i.x, i.y] = value; }
    }

    public GridObject this[int x, int y]
    {
        get { return grid[x, y]; }
        set { grid[x, y] = value; }
    }

    public IsoGrid (Vector2Int size, Vector2 spacing, Vector2Int workingArea, GridObject[] gridObjects = null)
    {
        grid = new GridObject[size.x, size.y];
        gridSize = size;
        this.spacing = spacing;
        worldSize.x = (size.x - 1) * this.spacing.x;
        worldSize.y = (size.y - 1) * this.spacing.y;

        Vector2Int halfGridSize = new Vector2Int (Mathf.FloorToInt (size.x / 2), Mathf.FloorToInt (size.y / 2)); // 3, 2
        Vector2Int halfWorkSize = new Vector2Int (Mathf.FloorToInt (workingArea.x / 2), Mathf.FloorToInt (workingArea.y / 2)); // 1, 1
        workingMin = halfGridSize - halfWorkSize;
        workingMax = halfGridSize + halfWorkSize;
        Debug.Log (workingMax);

        if (gridObjects != null)
        {
            foreach (GridObject obj in gridObjects)
            {
                Debug.Log ($"({obj.GridPosition}) {obj.name}");
                if (obj != null)
                {
                    obj.SetPosition (WorldToGrid (obj.transform.position, false));
                    grid[obj.GridPosition.x, obj.GridPosition.y] = obj;
                }
            }
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

    public bool GridCast (Vector2Int origin, int direction, out Vector2Int hitPoint, out bool hitWall, out Vector2 wallHitPoint)
    {
        Vector2Int lastPoint = origin;
        hitWall = false;
        wallHitPoint = Vector2.zero;
        for (int i = 0; i < Mathf.Max (gridSize.x, gridSize.y); i++)
        {
            // If cardinal
            if (direction % 2 == 0)
            {
                Debug.Log ("Line is Cardinal.");
                // Check above and below line for wall
                Vector2Int abovePoint = TranslatePoint (lastPoint, direction - 1);
                Vector2Int belowPoint = TranslatePoint (lastPoint, direction + 1);
                if (abovePoint.isBetween (Vector2Int.zero, gridSize - Vector2Int.one) &&
                    belowPoint.isBetween (Vector2Int.zero, gridSize - Vector2Int.one) &&
                    this[abovePoint] != null && this[abovePoint].Type == GridObjectType.Wall &&
                    this[belowPoint] != null && this[belowPoint].Type == GridObjectType.Wall)
                {
                    Debug.Log ("Line passes through walls. Checking if connected...");
                    Wall above = this[abovePoint] as Wall;
                    Wall below = this[belowPoint] as Wall;
                    if (above.IsConnected (below))
                    {
                        Debug.Log ("Walls are connected. Colliding.");
                        hitPoint = lastPoint;
                        hitWall = true;
                        wallHitPoint = (GridToWorld (abovePoint) + GridToWorld (belowPoint)) / 2;
                        return true;
                    }
                    else Debug.Log ("Walls are NOT connected. Ignoring.");
                }
            }

            Vector2Int point = TranslatePoint (lastPoint, direction);
            if (!point.x.isBetween (0, gridSize.x - 1) || !point.y.isBetween (0, gridSize.y - 1))
            {
                Debug.Log ($"point ({point.x}, {point.y}) is out of bounds");
                hitPoint = lastPoint;
                return false;
            }
            if (this[point] != null)
            {
                Debug.Log ($"point ({point.x}, {point.y}) contains object ({this[point]})");
                hitPoint = point;
                return true;
            }

            lastPoint = point;
        }

        hitPoint = lastPoint;
        return false;
    }

    public void MoveObject (GridObject obj, Vector2Int destination)
    {
        obj.transform.position = GridToWorld (destination);

        if (obj.GridPosition == destination) return;

        this[obj.GridPosition] = null;
        this[destination] = obj;
        obj.SetPosition (destination);
    }

    public bool SnapToGrid (GridObject obj, Vector2 position, bool restrainToWorking = true)
    {
        Vector2Int targetPoint = WorldToGrid (position, restrainToWorking);
        if (targetPoint == obj.GridPosition)
        {
            obj.transform.position = GridToWorld (targetPoint);
            return false;
        }
        else if (this[targetPoint] == null)
        {
            MoveObject (obj, targetPoint);
            return true;
        }

        // Find nearest unnoccupied point if targetPoint is occupied
        bool pointFound = false;
        float smallestDist = 0;
        Vector2Int foundPoint = Vector2Int.zero;
        int yMin = Mathf.Max ((targetPoint.y - 1), 0);
        for (int y = Mathf.Max ((targetPoint.y - 1), 0); y < Mathf.Min (targetPoint.y + 1, gridSize.y - 1); y++)
        {
            for (int x = Mathf.Max ((targetPoint.x - 1), 0); x < Mathf.Min (targetPoint.x + 1, gridSize.x - 1); x++)
            {
                if (x == targetPoint.x && y == targetPoint.y) continue;

                float sqrDistance = Tools.GetSqrDist (position, GridToWorld (x, y));
                // If unoccupied and closest so far
                if ((grid[x, y] == null || new Vector2Int (x, y) == obj.GridPosition) && (sqrDistance < smallestDist || !pointFound))
                {
                    pointFound = true;
                    smallestDist = sqrDistance;
                    foundPoint = new Vector2Int (x, y);
                }
            }
        }

        if (pointFound)
        {
            MoveObject (obj, foundPoint);
            return true;
        }
        else
        {
            obj.transform.position = GridToWorld (obj.GridPosition);
            return false;
        }
    }

    public Vector2Int NearestAvailablePoint (Vector2 position, Vector2Int originalPosition, bool restrainToWorking = true)
    {
        Vector2Int targetPoint = WorldToGrid (position, restrainToWorking);
        if (targetPoint == originalPosition || this[targetPoint] == null)
            return targetPoint;

        Vector2Int min = new Vector2Int (
            Mathf.Max (targetPoint.y - 1, restrainToWorking ? workingMin.y : 0),
            Mathf.Max (targetPoint.x - 1, restrainToWorking ? workingMin.x : 0));
        Vector2Int max = new Vector2Int (
            Mathf.Min (targetPoint.y + 1, restrainToWorking ? workingMax.y : gridSize.y - 1),
            Mathf.Min (targetPoint.x + 1, restrainToWorking ? workingMax.x : gridSize.x - 1));

        Vector2Int foundPoint = originalPosition;
        float smallestDist = Tools.GetSqrDist (position, GridToWorld (foundPoint));

        for (int y = min.y; y < max.y; y++)
        {
            for (int x = min.x; x < max.x; x++)
            {
                if (targetPoint.SamePoint (x, y)) continue;

                float sqrDistance = Tools.GetSqrDist (position, GridToWorld (x, y));
                // If unoccupied and closest so far
                if ((grid[x, y] == null || originalPosition.SamePoint (x, y)) && sqrDistance < smallestDist)
                {
                    smallestDist = sqrDistance;
                    foundPoint = new Vector2Int (x, y);
                }
            }
        }

        return foundPoint;
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
    }
    
    public Vector2 GridToWorld (Vector2Int point) { return GridToWorld (point.x, point.y); }
    public Vector2 GridToWorld (int x, int y)
    {
        float posX = x * spacing.x + y % 2 * spacing.x / 2 - (worldSize.x + spacing.x / 2) / 2;
        float posY = worldSize.y / 2 - y * spacing.y;

        return new Vector2 (posX, posY);
    }

    public Vector2Int WorldToGrid (Vector2 position, bool restrainToWorkingArea = true)
    {
        int y = Mathf.RoundToInt ((worldSize.y / 2 - position.y) / spacing.y);
        int x = Mathf.RoundToInt ((position.x - y % 2 * spacing.x / 2 + (worldSize.x + spacing.x / 2) / 2) / spacing.x);

        if (restrainToWorkingArea)
        {
            x = Mathf.Clamp (x, workingMin.x, workingMax.x);
            y = Mathf.Clamp (y, workingMin.y, workingMax.y);
        }
        else
        {
            x = Mathf.Clamp (x, 0, gridSize.x - 1);
            y = Mathf.Clamp (y, 0, gridSize.y - 1);
        }

        return new Vector2Int (x, y);
    }
}