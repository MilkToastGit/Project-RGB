using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class GridObject : MonoBehaviour
{
    public abstract GridObjectType Type { get; }

    [SerializeField]
    [HideInInspector]
    private Vector2Int gridPos;
    [SerializeField]
    private bool movable = false;
    protected BeamRadial outputBeams = new BeamRadial ();
    private SpriteRenderer sprite;

    public Vector2Int GridPosition => gridPos;

    private Interactable interactable;
    private bool held = false;

    public virtual void ReceiveBeam (LightBeam beam) { }

    public void SetPosition (int x, int y) => gridPos = new Vector2Int (x, y);

    public void SetPosition (Vector2Int position) => gridPos = position;

    protected void SetRotation (int direction) => transform.rotation = Tools.IntToRot(direction);


    private void Awake ()
    {
        if (movable)
            interactable = GetComponent<Interactable> ();
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

    protected virtual void OnEnable ()
    {
        InstantiatePersistentScene.OnManagersLoaded += OnManagersLoaded;

        if (movable)
            interactable.OnInteract += OnInteract;
    }

    protected virtual void OnDisable ()
    {
        if (movable)
            interactable.OnInteract -= OnInteract;
        if (held) InputManager.Instance.OnEndTouch -= OnInteractEnd;
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

    public void SnapToGrid ()
    {
        Vector2Int newPos = IsoGrid.Instance.WorldToGrid (transform.position, false);
        if (gridPos != newPos)
            IsoGrid.Instance.Reallocate (this, newPos);

        transform.position = IsoGrid.Instance.GridToWorld (GridPosition);
    }

    protected virtual void OnManagersLoaded () { }

    public abstract class MultiInput : GridObject
    {
        private List<LightBeam> inputBuffer = new List<LightBeam> ();

        public override void ReceiveBeam (LightBeam beam)
        {
            if (inputBuffer.Count == 0)
                EventManager.Instance.OnAllBeamsTerminated += CastBeams;

            inputBuffer.Add (beam);
            print ($"input count: {inputBuffer.Count}");
        }

        private void CastBeams ()
        {
            BeamRadial output = GenerateOutputBeams (inputBuffer);
            print ($"Casting {output.Count} beams");

            for (int dir = 0; dir < 8; dir++)
            {
                if (outputBeams[dir] != output[dir])
                    output[dir]?.Cast (GridPosition, dir);
            }

            EventManager.Instance.OnAllBeamsTerminated -= CastBeams;
            outputBeams = output;
        }

        private void ResetInputBuffer () => inputBuffer.Clear ();

        protected abstract BeamRadial GenerateOutputBeams (List<LightBeam> input);

        protected override void OnManagersLoaded ()
        {
            EventManager.Instance.OnAllBeamsRendered += ResetInputBuffer;
        }
    }
}

public enum GridObjectType
{
    Emitter,
    Splitter,
    Redirector,
    Node
}
