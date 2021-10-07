using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Output : GridObject
{
    public override GridObjectType Type => GridObjectType.Output;
    [SerializeField] private LightBeam requiredBeam;
    private SpriteRenderer outerBox, innerBox;
    private bool correct = false;

    private BeamRadial currentInput = new BeamRadial ();
    public bool Correct => correct;

    protected override void OnEnable ()
    {
        base.OnEnable ();
        SpriteRenderer[] boxes = GetComponentsInChildren<SpriteRenderer> ();
        outerBox = boxes[0];
        innerBox = boxes[1];
        outerBox.color = requiredBeam.Colour;
        innerBox.color = Color.black;
    }

    public override void ReceiveBeam (LightBeam beam)
    {
        beam.OnBeamCancelled += OnBeamCancelled;
        currentInput.AddCombine (beam);
    }

    private void ResetInput () => currentInput.Clear ();

    private void CheckInput ()
    {
        LightBeam combinedInput = new LightBeam ();

        foreach (LightBeam beam in currentInput)
            combinedInput += beam;

        print ($"combined input {combinedInput.Colour}");
        innerBox.color = combinedInput.Colour;
        correct = combinedInput.SameColour (requiredBeam);
    }

    private void OnBeamCancelled (LightBeam beam)
    {
        beam.OnBeamCancelled -= OnBeamCancelled;
        currentInput.Remove (beam);
    }

    protected override void OnManagersLoaded ()
    {
        base.OnManagersLoaded ();
        EventManager.Instance.OnAllBeamsRendered += CheckInput;
        EventManager.Instance.OnGridObjectMoved += ResetInput;
    }

    protected override void OnDisable ()
    {
        base.OnDisable ();
        EventManager.Instance.OnAllBeamsRendered -= CheckInput;
        EventManager.Instance.OnGridObjectMoved -= ResetInput;
    }
}
