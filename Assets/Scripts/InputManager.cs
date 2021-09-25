using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : SingletonPersistent<InputManager>
{
    private TouchInput touchInput;
    private Camera mainCam;

    public Vector2 WorldTouchPosition { get { return mainCam.ScreenToWorldPoint (touchInput.Touch.Position.ReadValue<Vector2> ()); } }

    public delegate void StartTouchEvent (Vector2 position);
    public event StartTouchEvent OnStartTouch;
    private void StartTouch (InputAction.CallbackContext context) => OnStartTouch?.Invoke (WorldTouchPosition);

    public delegate void EndTouchEvent (Vector2 position);
    public event EndTouchEvent OnEndTouch;
    private void EndTouch (InputAction.CallbackContext context) => OnEndTouch?.Invoke (WorldTouchPosition);

    private void Awake ()
    {
        touchInput = new TouchInput ();
        mainCam = Camera.main;
    }

    private void Start ()
    {
        touchInput.Touch.Press.started += ctx => StartTouch (ctx);
        touchInput.Touch.Press.canceled += ctx => EndTouch (ctx);
    }
    
    private void OnEnable ()
    {
        touchInput.Enable ();
    }

    private void OnDisable ()
    {
        touchInput.Disable ();
    }
}
