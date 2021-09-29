using UnityEngine;

[System.Serializable]
public abstract class GridObject
{
    public abstract GridObjectType Type { get; }

    [SerializeField]
    private Vector2Int gridPos;

    public Vector2Int GridPosition => gridPos;

    public virtual void ReceiveBeam (int direction, LightBeam beam) { }
    public virtual void DrawAndSetParams () { }

    public void SetPosition (int x, int y) => gridPos = new Vector2Int (x, y);

    public void SetPosition (Vector2Int position) => gridPos = position;
}

public enum GridObjectType
{
    Emitter,
    Splitter
}
