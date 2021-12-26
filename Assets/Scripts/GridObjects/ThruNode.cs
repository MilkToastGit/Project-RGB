using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class ThruNode : GridObject
{
    public override GridObjectType Type => GridObjectType.ThruNode;
    [SerializeField] private LightBeam requiredBeam;
    private SpriteRenderer outerBox, innerBox;
    private bool correct = false;

    //toms shit he added to rotate
    [SerializeField] private bool rotatable;
    [Range(0, 7)]
    [SerializeField] private int outputDirection;

    private SpriteRenderer arrow;
    private int preRotateDirection;

    private List<LightBeam> currentInput = new List<LightBeam>();
    public bool Correct => correct;

    protected override void OnEnable()
    {
        base.OnEnable();
        SpriteRenderer[] boxes = GetComponentsInChildren<SpriteRenderer>();
        outerBox = boxes[0];
        innerBox = boxes[1];
        outerBox.color = requiredBeam.Colour;
        innerBox.color = Color.black;

        //toms shit he added
        arrow = GetComponentsInChildren<SpriteRenderer>()[1];
        arrow.color = rotatable ? Color.white : Color.grey;
        SetRotation(outputDirection);
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        SetRotation(outputDirection);
    }

    public override void ReceiveBeam(LightBeam beam)
    {
        if (gameObject.name == "TEST")
            print($"Received {beam.Colour} from point {beam.Origin} in direction {beam.Direction}");
        beam.OnBeamCancelled += OnBeamCancelled;
        currentInput.Add(beam);
    }

    private void ResetInput() => currentInput.Clear();

    private void CheckInput()
    {
        LightBeam combinedInput = new LightBeam();

        foreach (LightBeam beam in currentInput)
            combinedInput += beam;

        //print ($"combined input {combinedInput.Colour}");
        innerBox.color = combinedInput.Colour;
        correct = combinedInput.SameColour(requiredBeam);
    }

    private void OnBeamCancelled(LightBeam beam)
    {
        beam.OnBeamCancelled -= OnBeamCancelled;
        currentInput.Remove(beam);
    }

    protected override void OnManagersLoaded()
    {
        base.OnManagersLoaded();
        EventManager.Instance.OnAllBeamsRendered += CheckInput;
        EventManager.Instance.OnGridObjectMoved += ResetInput;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventManager.Instance.OnAllBeamsRendered -= CheckInput;
        EventManager.Instance.OnGridObjectMoved -= ResetInput;
    }
}
