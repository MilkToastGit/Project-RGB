using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder (-1)]
public class InputManager : SingletonPersistent<InputManager>
{
    private TouchInput touchInput;
    private Camera _mainCam;
    private Camera mainCam { get { if (_mainCam == null) _mainCam = Camera.main; return _mainCam; } }

    public Vector2 WorldTouchPosition { get { return mainCam.ScreenToWorldPoint (touchInput.Touch.Position.ReadValue<Vector2> ()); } }

    public delegate void StartTouchEvent (Vector2 position);
    public event StartTouchEvent OnStartTouch;

    public delegate void EndTouchEvent (Vector2 position);
    public event EndTouchEvent OnEndTouch;

    private void Awake ()
    {
        touchInput = new TouchInput ();
    }

    //private void UpdateMainCam (Scene scene, LoadSceneMode loadSceneMode) => mainCam = Camera.main;

    private void Start ()
    {
        touchInput.Touch.Press.started += ctx => StartTouch (ctx);
        touchInput.Touch.Press.canceled += ctx => EndTouch (ctx);
    }

    private void StartTouch (InputAction.CallbackContext context)
    {
        print (touchInput.Touch.Position.ReadValue<Vector2> ());
        OnStartTouch?.Invoke (WorldTouchPosition);
    }

    private void EndTouch (InputAction.CallbackContext context)
    {
        OnEndTouch?.Invoke (WorldTouchPosition);
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
