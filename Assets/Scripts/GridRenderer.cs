using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    [SerializeField] private GameObject rendererPrefab;
    public void RenderGrid (IsoGrid grid)
    {
        float lineLength = grid.WorldSize.magnitude;
        for (int y = 0; y < grid.Height - 1; y++)
        {
            DrawLineInDirection (0, y, 3, grid);
            DrawLineInDirection (grid.Width - 1, y, 5, grid);
        }

        for (int x = 1; x < grid.Width - 1; x++)
        {
            DrawLineInDirection (x, 0, 3, grid);
            DrawLineInDirection (x, 0, 5, grid);
        }
    }

    private void DrawLineInDirection (int x, int y, int direction, IsoGrid grid)
    {
        LineRenderer lr = Instantiate (rendererPrefab, transform).GetComponent<LineRenderer> ();
        Vector2Int origin = new Vector2Int (x, y);
        Vector3[] positions = new Vector3[] {
                grid.GridToWorld (origin),
                grid.GridToWorld (grid.GetBoundPointInDirection (origin, direction)) };
        lr.SetPositions (positions);
    }
}
