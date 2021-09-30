using System;
using UnityEngine;

public class EventManager : SingletonPersistent<EventManager>
{
    public event Action<Vector2Int, Vector2Int> OnGridObjectMoved;
    public void GridObjectMoved (Vector2Int from, Vector2Int to) => OnGridObjectMoved?.Invoke (from, to);
}
