using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private static readonly float moveThreshold = 0.15f;
    private static readonly float LongPressThreshold = 0.5f;

    public event Action<Vector2> OnInteractStart;
    public event Action<Vector2> OnInteractEnd;

    public event Action OnDragStart;
    public event Action OnDragEnd;

    public event Action OnPressDragStart;
    public event Action OnPressDragEnd;

    //public void Interact (Vector2 touchPos) => OnInteract?.Invoke (touchPos);
    private InteractState state = InteractState.NotTouching;
    private Timer heldTime = new Timer (0);
    private Vector2 firstTouchPos;
    private bool movedPastThresh => Tools.GetSqrDist (InputManager.Instance.WorldTouchPosition, firstTouchPos) > moveThreshold;

    public InteractState State => state;

    public void InteractStart (Vector2 touchPos)
    {
        OnInteractStart?.Invoke (touchPos);

        heldTime.Reset ();
        firstTouchPos = touchPos;
        state = InteractState.Touching;
        InputManager.Instance.OnEndTouch += InteractEnd;
    }

    private void InteractEnd (Vector2 touchPos)
    {
        OnInteractEnd?.Invoke (touchPos);
        if (state == InteractState.Dragging) OnDragEnd?.Invoke ();
        else if (state == InteractState.PressDragging) OnPressDragEnd?.Invoke ();

        state = InteractState.NotTouching;
        InputManager.Instance.OnEndTouch -= InteractEnd;
    }

    private void Update ()
    {
        if (state == InteractState.Touching)
        {
            if (movedPastThresh)
            {
                OnDragStart?.Invoke ();
                state = InteractState.Dragging;
            }
            else if (heldTime > LongPressThreshold)
            {
                OnPressDragStart?.Invoke ();
                state = InteractState.PressDragging;
            }
        }
    }
}

public enum InteractState
{
    NotTouching,
    Touching,
    Dragging,
    PressDragging
}