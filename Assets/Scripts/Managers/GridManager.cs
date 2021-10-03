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
    private void GenerateGrid ()
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
    }

    private void Start ()
    {
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

    public Vector2Int CastBeam (LightBeam beam)
    {
        Debug.Log ("Start " + beam.Colour);
        if (beam.Origin == null) throw new System.Exception ("Cannot cast beam with no origin.");

        if (grid.GridCast (beam.Origin, beam.Direction, out Vector2Int hitPoint))
            grid[hitPoint]?.ReceiveBeam (beam);

        Debug.Log ("End " + beam.Colour);
        return hitPoint;
    }

    public Vector2 GridToWorld (Vector2Int point) => grid.GridToWorld (point);
    public Vector2 GridToWorld (int x, int y) => grid.GridToWorld (x, y);

    public Vector2Int WorldToGrid (Vector2 position, bool restrainToWorkingArea = true) => grid.WorldToGrid (position, restrainToWorkingArea);

    public void SnapToGrid (GridObject obj, Vector2 position, bool restrainToWorkingArea = true)
    {
        grid.SnapToGrid (obj, position, restrainToWorkingArea);
    }

    public Vector2 SnapToGrid (Vector2 position, bool restrainToWorkingArea = true)
    {
        return grid.GridToWorld (grid.WorldToGrid (position, restrainToWorkingArea));
    }

    public void MoveObject (GridObject obj, Vector2Int destination) => grid.MoveObject (obj, destination);
}

[System.Serializable]
public class IsoGrid
{
    [SerializeField] private GridObject[,] grid;
    [SerializeField] private Vector2Int gridSize, workingArea;
    [SerializeField] private Vector2 worldSize, spacing;

    public GridObject this[Vector2Int i]
    {
        get { return grid[i.x, i.y]; }
        set { grid[i.x, i.y] = value; }
    }

    public IsoGrid (Vector2Int size, Vector2 spacing, Vector2Int workingArea, GridObject[] gridObjects = null)
    {
        grid = new GridObject[size.x, size.y];
        gridSize = size;
        this.spacing = spacing;
        this.workingArea = workingArea;
        worldSize.x = (size.x - 1) * this.spacing.x;
        worldSize.y = (size.y - 1) * this.spacing.y;

        if (gridObjects != null)
        {
            foreach (GridObject obj in gridObjects)
            {
                if (obj != null)
                    grid[obj.GridPosition.x, obj.GridPosition.y] = obj;
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

    public bool GridCast (Vector2Int origin, int direction, out Vector2Int hitPoint)
    {
        Vector2Int lastPoint = origin;
        for (int i = 0; i < Mathf.Max (gridSize.x, gridSize.y); i++)
        {
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

        grid[obj.GridPosition.x, obj.GridPosition.y] = null;
        grid[destination.x, destination.y] = obj;
        obj.SetPosition (destination);
    }

    public bool SnapToGrid (GridObject obj, Vector2 position, bool restrainToWorkingArea = true)
    {
        Vector2Int targetPoint = WorldToGrid (position, restrainToWorkingArea);
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
}