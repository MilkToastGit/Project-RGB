using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Output : GridObject
{
    public override GridObjectType Type => GridObjectType.Output;
    [SerializeField] private LightBeam requiredBeam;
    private SpriteRenderer outerBox, innerBox;

    private LightBeam currentInput;

    protected override void OnEnable ()
    {
        base.OnEnable ();
        SpriteRenderer[] boxes = GetComponentsInChildren<SpriteRenderer> ();
        outerBox = boxes[0];
        innerBox = boxes[1];
        outerBox.color = requiredBeam.Colour;
    }

    public override void ReceiveBeam (LightBeam beam) => currentInput += beam;

    private void ResetInput () => currentInput = new LightBeam (0, 0, 0);

    private void CheckInput ()
    {
        innerBox.color = currentInput.Colour;
        if (currentInput.SameColour (requiredBeam))
            Debug.Log ($"CORRECT BEAM ENTERED OUTPUT NODE {gameObject.name}");
    }

    protected override void OnManagersLoaded ()
    {
        base.OnManagersLoaded ();
        EventManager.Instance.OnAllBeamsRendered += CheckInput;
        EventManager.Instance.OnGridObjectMoved += ResetInput;
    }
}
