using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class GridObject : MonoBehaviour
{
    public abstract GridObjectType Type { get; }

    [SerializeField] [HideInInspector] private Vector2Int gridPos;
    [SerializeField] private bool movable = false;

    public Vector2Int GridPosition => gridPos;

    protected Interactable interactable;
    private SpriteRenderer sprite;
    private GameObject _ghost;
    private GameObject ghost 
    { 
        get 
        {
            if (_ghost == null) _ghost = GenerateGhost ();
            if (_ghost.transform.rotation != transform.rotation) _ghost.transform.rotation = transform.rotation;
            return _ghost;
        } 
    }

    public abstract void ReceiveBeam (LightBeam beam);

    public void SetPosition (int x, int y) => gridPos = new Vector2Int (x, y);

    public void SetPosition (Vector2Int position) => gridPos = position;

    protected void SetRotation (int direction) => transform.rotation = Tools.IntToRot(direction);

    public abstract class MonoInput : GridObject
    {
        private Dictionary<LightBeam, BeamRadial> beamIO = new Dictionary<LightBeam, BeamRadial> ();

        protected abstract BeamRadial GenerateOutputBeams (LightBeam beam);

        public override void ReceiveBeam (LightBeam beam)
        {
            BeamRadial output = GenerateOutputBeams (beam);
            beamIO.Add (beam, output);

            print ($"Casting {output.Count} beams");
            foreach (LightBeam outputBeam in output)
                outputBeam.Cast (GridPosition, outputBeam.Direction);
            
            beam.OnBeamCancelled += OnInputBeamCancelled;
        }

        private void OnInputBeamCancelled (LightBeam cancelledBeam)
        {
            cancelledBeam.OnBeamCancelled -= OnInputBeamCancelled;

            if (beamIO.TryGetValue (cancelledBeam, out BeamRadial outputBeams))
            {
                foreach (LightBeam outputBeam in outputBeams)
                    outputBeam.CancelBeam ();
            }
            else throw new System.Exception ("Beam {beam} could not be cancelled; not found in beamIO");
        }
    }

    public abstract class MultiInput : GridObject
    {
        private BeamRadial inputBuffer = new BeamRadial ();
        private BeamRadial outputBeams = new BeamRadial ();
        private bool awaitingOutput = false;

        protected abstract BeamRadial GenerateOutputBeams (BeamRadial input);

        public override void ReceiveBeam (LightBeam beam)
        {
            if (inputBuffer.Contains (beam) /*|| outputBeams[beam.Direction + 4] != null*/) return;

            if (!outputBeams.isEmpty)
            {
                print ($"{name} Contains Input? {inputBuffer.Contains (beam)}, Contains Output? {outputBeams[beam.Direction + 4] != null}");
                foreach (LightBeam outBeam in outputBeams)
                    outBeam.CancelBeam ();
                print ($"({name}) received new beam; cancelling outputs");
            }

            AwaitOutput ();

            inputBuffer.Add (beam);
            print ($"input count: {inputBuffer.Count}");
        }

        private void CastBeams ()
        {
            BeamRadial output = GenerateOutputBeams (inputBuffer);
            print ($"{name} casting {output.Count} beams");

            for (int dir = 0; dir < 8; dir++)
            {
                if (outputBeams[dir] != output[dir])
                    output[dir]?.Cast (GridPosition, dir);
            }

            EventManager.Instance.OnAllBeamsTerminated -= CastBeams;
            awaitingOutput = false;
            outputBeams = output;
        }

        private void AwaitOutput ()
        {
            if (!awaitingOutput)
            {
                EventManager.Instance.OnAllBeamsTerminated += CastBeams;
                awaitingOutput = true;
            }
        }

        private void ResetIO ()
        {
            inputBuffer.Clear ();
            outputBeams.Clear ();
            awaitingOutput = false;
        }

        protected override void OnManagersLoaded ()
        {
            EventManager.Instance.OnAllBeamsRendered += ResetIO;
        }

        private void OnInputBeamCancelled (LightBeam cancelledBeam)
        {
            cancelledBeam.OnBeamCancelled -= OnInputBeamCancelled;

            foreach (LightBeam outputBeam in outputBeams)
                outputBeam.CancelBeam ();
            inputBuffer.Remove (cancelledBeam);

            AwaitOutput ();
        }
    }


    private void Awake ()
    {
        sprite = GetComponent<SpriteRenderer> ();
        interactable = GetComponent<Interactable> ();
    }

    protected virtual void OnDrawGizmos () => SnapToGrid ();

    protected virtual void OnEnable ()
    {
        if (ManagerLoader.loaded)
            OnManagersLoaded ();
        else
            ManagerLoader.OnManagersLoaded += OnManagersLoaded;

        if (movable)
        {
            interactable.OnDragStart += OnDragStart;
            interactable.OnDragEnd += OnDragEnd;
        }

        interactable.OnPressDragStart += OnPressDragStart;
        interactable.OnPressDragEnd += OnPressDragEnd;
    }

    protected virtual void OnDisable ()
    {
        if (movable)
        {
            interactable.OnDragStart -= OnDragStart;
            interactable.OnDragEnd -= OnDragEnd;
        }

        interactable.OnPressDragStart -= OnPressDragStart;
        interactable.OnPressDragEnd -= OnPressDragEnd;
    }

    private void Update ()
    {
        if (interactable.State == InteractState.Dragging)
            DraggingUpdate ();
        else if (interactable.State == InteractState.PressDragging)
            LongPressedUpdate ();
    }

    protected virtual void DraggingUpdate ()
    {
        if (!movable) return;
        ghost.transform.position = GridManager.Instance.SnapToGrid (InputManager.Instance.WorldTouchPosition, GridPosition, true);
    }

    protected virtual void LongPressedUpdate () => DraggingUpdate ();

    private void OnDragStart ()
    {
        if (!movable) return;
        ghost.SetActive (true);
    }

    private void OnDragEnd ()
    {
        if (!movable) return;
        ghost.SetActive (false);
        Vector2Int ghostGridPoint = GridManager.Instance.WorldToGrid (ghost.transform.position, true);
        if (gridPos != ghostGridPoint)
        {
            GridManager.Instance.MoveObject (this, ghostGridPoint);
            EventManager.Instance.GridObjectMoved ();
        }
    }

    protected virtual void OnPressDragStart () => OnDragStart ();

    protected virtual void OnPressDragEnd () => OnDragEnd ();

    public void SnapToGrid ()
    {
        GridManager.Instance.SnapToGrid (this, transform.position, movable);
    }

    private GameObject GenerateGhost ()
    {
        GameObject ghost = new GameObject ($"{gameObject.name} ghost");
        ghost.transform.parent = transform.parent;

        SpriteRenderer ghostSprite = ghost.AddComponent<SpriteRenderer> ();
        ghostSprite.sprite = sprite.sprite;
        Color transparent = ghostSprite.color;
        transparent.a = 0.5f;
        ghostSprite.color = transparent;

        return ghost;
    }

    protected virtual void OnManagersLoaded () { }
}

public enum GridObjectType
{
    Emitter,
    Splitter,
    Redirector,
    Output,
    Wall
}
