using UnityEngine;

[RequireComponent (typeof(Interactable))]
public abstract class Prism : MonoBehaviour
{
    protected Interactable interactable;
    protected bool held = false;

    protected void Awake ()
    {
        interactable = GetComponent<Interactable> ();
    }

    protected void OnInteract (Vector2 touchPos)
    {
        held = true;
        InputManager.Instance.OnEndTouch += OnInteractEnd;
    }

    protected void OnInteractEnd (Vector2 touchPos)
    {
        held = false;
        InputManager.Instance.OnEndTouch -= OnInteractEnd;
    }

    protected void Update ()
    {
        if (held)
        {
            transform.position = GridManager.Instance.SnapToGrid (InputManager.Instance.WorldTouchPosition);
        }
    }

    protected void OnEnable ()
    {
        interactable.OnInteract += OnInteract;
    }

    protected void OnDisable ()
    {
        interactable.OnInteract -= OnInteract;
        if (held) InputManager.Instance.OnEndTouch -= OnInteractEnd;
    }
}
