using UnityEngine;
using UnityEditor;

[RequireComponent (typeof(Interactable))]
public class GridObjectMono : MonoBehaviour
{
    [SerializeField]
    private GridObject gridObject;

    public GridObject GridObject => gridObject;
    public GridObjectType Type;

    [SerializeField]
    private bool movable = false;

    private Interactable interactable;
    private bool held = false;

    private void Awake ()
    {
        interactable = GetComponent<Interactable> ();
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

    private void Update ()
    {
        if (held)
        {
            transform.position = InputManager.Instance.WorldTouchPosition;
            SnapToGrid ();
        }
    }

    public void SnapToGrid (bool callEvent = true)
    {
        Vector2Int newPos = IsoGrid.Instance.WorldToGrid (transform.position, false);
        if (gridObject.GridPosition != newPos)
        {
            GridObject.SetPosition (IsoGrid.Instance.WorldToGrid (transform.position, false));
            if (callEvent)
                EventManager.Instance.GridChanged ();
        }

        transform.position = IsoGrid.Instance.GridToWorld (GridObject.GridPosition);
    }

    public void SetGridObject ()
    {
        switch (Type)
        {
            case GridObjectType.Emitter:
                gridObject = new Emitter ();
                break;
            case GridObjectType.Splitter:
                gridObject = new Splitter ();
                break;
        }
    }

    private void OnEnable ()
    {
        gridObject.OnEnable ();

        if (movable)
            interactable.OnInteract += OnInteract;
    }

    private void OnDisable ()
    {
        gridObject.OnDisable ();

        if (movable)
            interactable.OnInteract -= OnInteract;
        if (held) InputManager.Instance.OnEndTouch -= OnInteractEnd;
    }
}