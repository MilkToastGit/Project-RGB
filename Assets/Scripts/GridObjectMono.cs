using UnityEngine;

[RequireComponent (typeof(Interactable))]
public class GridObjectMono : MonoBehaviour
{
    public GridObject GridObject;
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
            SnapToGrid ();
    }

    public void SnapToGrid ()
    {
        if (GridObject == null) return;

        Vector2Int newPos = IsoGrid.Instance.WorldToGrid (transform.position, false);
        if (GridObject.GridPosition != newPos)
        {
            GridObject.SetPosition (IsoGrid.Instance.WorldToGrid (transform.position, false));
            gameObject.transform.position = IsoGrid.Instance.GridToWorld (GridObject.GridPosition);

            //if (EventManager.Instance != null)
            //    EventManager.Instance.GridChanged ();
        }
    }

    private void OnEnable ()
    {
        if (movable)
            interactable.OnInteract += OnInteract;
    }

    private void OnDisable ()
    {
        if (movable)
            interactable.OnInteract -= OnInteract;
        if (held) InputManager.Instance.OnEndTouch -= OnInteractEnd;
    }
}