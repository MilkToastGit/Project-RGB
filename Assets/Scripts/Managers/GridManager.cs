using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    [SerializeField]
    private Vector2Int gridSize;
    [SerializeField]
    private Vector2 gridSpacing;
    private bool[,] grid;

    private float gridWidth;
    private float gridHeight;

    private void Start ()
    {
        GenerateGrid ();
    }

    [ContextMenu ("GenerateGrid")]
    private void GenerateGrid ()
    {
        GenerateGrid (gridSize);
    }

    private void GenerateGrid (Vector2Int size)
    {
        grid = new bool[size.x, size.y];
        gridWidth = (size.x - 1) * gridSpacing.x;
        gridHeight = (size.y - 1) * gridSpacing.y;
        print (gridHeight);

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Debug.DrawRay (GetGridPointPosition (x, y), Vector2.up * 0.1f, Color.white, 20);
            }
        }
    }

    public Vector2 GetGridPointPosition (int x, int y)
    {
        float posX = x * gridSpacing.x + y % 2 * gridSpacing.x / 2 - (gridWidth + gridSpacing.x / 2) / 2;
        float posY = y * gridSpacing.y - gridHeight / 2;

        return new Vector2 (posX, posY);
    }

    public Vector2 SnapToGrid (Vector2 position)
    {
        print (gridHeight);

        int y = Mathf.RoundToInt ((position.y + gridHeight / 2) / gridSpacing.y);
        int x = Mathf.RoundToInt ((position.x - y % 2 * gridSpacing.x / 2 + (gridWidth + gridSpacing.x / 2) / 2) / gridSpacing.x);
        print ($"(({position.y} + {gridHeight / 2}) / {gridSpacing.y} = {y}");

        return GetGridPointPosition (Mathf.Clamp (x, 0, gridSize.x), Mathf.Clamp (y, 0, gridSize.y));
    }

    public bool GridCast (int x, int y, int direction)
    {
        direction %= 8;

        Vector2Int point = new Vector2Int (x, y);
        for (int i = 0; i < Mathf.Max (gridSize.x, gridSize.y); i++)
        {
            point = GetPointInDirection (point, direction);
            if (!point.x.isBetween (0, gridSize.x) || !point.y.isBetween (0, gridSize.y))
                return false;
            if (grid[point.x, point.y])
                return true;
        }

        return false;
    }

    private Vector2Int GetPointInDirection (Vector2Int origin, int direction) { return GetPointInDirection (origin.x, origin.y, direction); }
    private Vector2Int GetPointInDirection (int xOrigin, int yOrigin, int direction)
    {
        int x = xOrigin;
        int y = yOrigin;

        direction %= 8;

        y += 2 * Mathf.Abs (((direction / 2 - 2) % 4) - 2) - 2;

        switch (direction)
        {
            case 1:
            case 3:
                x += y % 2;
                break;
            case 2:
            case 6:
                x += 1;
                break;
            case 5:
            case 7:
                x -= 1 - y % 2;
                break;
        }

        return new Vector2Int (x, y);

        //switch (direction)
        //{
        //    case 0:
        //        y -= 2;
        //        break;
        //    case 1:
        //        x += y % 2;
        //        y -= 1;
        //        break;
        //    case 2:
        //        x += 1;
        //        break;
        //    case 3:
        //        x += y % 2;
        //        y += 1;
        //        break;
        //    case 4:
        //        y += 2;
        //        break;
        //    case 5:
        //        x -= 1 - y % 2;
        //        y += 1;
        //        break;
        //    case 6:
        //        x -= 1;
        //        break;
        //    case 7:
        //        x -= y % 2;
        //        y -= 1;
        //        break;
        //}
    }
}
