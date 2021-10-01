using UnityEngine;

[DefaultExecutionOrder (-1)]
public class InputManager : SingletonPersistent<InputManager>
{
    private Camera _mainCam;
    private Camera mainCam { get { if (_mainCam == null) _mainCam = Camera.main; return _mainCam; } }

    private Vector2 touchPosition;
    public Vector2 WorldTouchPosition { get { return mainCam.ScreenToWorldPoint (touchPosition); } }

    public delegate void StartTouchEvent (Vector2 position);
    public event StartTouchEvent OnStartTouch;

    public delegate void EndTouchEvent (Vector2 position);
    public event EndTouchEvent OnEndTouch;

    private void Update ()
    {
        if (Input.touchCount > 0)
        {
            Debug.DrawRay (WorldTouchPosition, Vector2.up * 0.5f, Color.green);
            Touch touch = Input.GetTouch (0);
            touchPosition = touch.position;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnStartTouch?.Invoke (WorldTouchPosition);
                    break;
                case TouchPhase.Ended:
                    OnEndTouch?.Invoke (WorldTouchPosition);
                    break;
            }
        }
    }
}
