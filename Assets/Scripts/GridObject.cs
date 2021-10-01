using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GridObject : MonoBehaviour
{
    public virtual GridObjectType Type { get; }
    protected virtual bool isMultiInput => false;

    [SerializeField]
    [HideInInspector]
    private Vector2Int gridPos;
    [SerializeField]
    private bool movable = false;

    public Vector2Int GridPosition => gridPos;

    protected List<LightBeam> inputBuffer;
    private Interactable interactable;
    private bool held = false;

    public virtual void ReceiveBeam (LightBeam beam) { if (isMultiInput) inputBuffer.Add (beam); }

    public void SetPosition (int x, int y) => gridPos = new Vector2Int (x, y);

    public void SetPosition (Vector2Int position) => gridPos = position;


    private void Awake ()
    {
        if (movable)
            interactable = GetComponent<Interactable> ();
        if (isMultiInput)
            inputBuffer = new List<LightBeam> ();
    }

    private void OnInteract (Vector2 touchPos)
    {
        held = true;
        InputManager.Instance.OnEndTouch += OnInteractEnd;
    }

    private void OnInteractEnd (Vector2 touchPos)
    {
        held = false;
        InputManager.Instance.OnEndTouch -= OnInteractEnd;
    }

    [ExecuteInEditMode]
    private void Update ()
    {
        if (held)
        {
            transform.position = InputManager.Instance.WorldTouchPosition;
        }
        SnapToGrid ();
    }

    public void SnapToGrid ()
    {
        Vector2Int newPos = IsoGrid.Instance.WorldToGrid (transform.position, false);
        if (gridPos != newPos)
            IsoGrid.Instance.Reallocate (this, newPos);

        transform.position = IsoGrid.Instance.GridToWorld (GridPosition);
    }

    public virtual void OnEnable ()
    {
        InstantiatePersistentScene.OnManagersLoaded += OnManagersLoaded;

        if (movable)
            interactable.OnInteract += OnInteract;
    }

    public virtual void OnManagersLoaded () { }

    public virtual void OnDisable ()
    {
        if (movable)
            interactable.OnInteract -= OnInteract;
        if (held) InputManager.Instance.OnEndTouch -= OnInteractEnd;
    }
}

public enum GridObjectType
{
    Emitter,
    Splitter
}
