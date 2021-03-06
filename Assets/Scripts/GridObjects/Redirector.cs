using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Redirector : GridObject.MultiInput
{
    public override GridObjectType Type => GridObjectType.Redirector;
    [SerializeField] private bool rotatable;
    [Range (0, 7)]
    [SerializeField] private int outputDirection;

    private SpriteRenderer arrow;
    private int preRotateDirection;

    protected override void OnEnable()
    {
        base.OnEnable();
        arrow = GetComponentsInChildren<SpriteRenderer> ()[1];
        arrow.color = rotatable ? Color.white : Color.grey;
        SetRotation (outputDirection);
    }

    protected override void OnDrawGizmos ()
    {
        base.OnDrawGizmos ();
        SetRotation (outputDirection);
    }

    protected override BeamRadial GenerateOutputBeams(BeamRadial input)
    {
        BeamRadial output = new BeamRadial();

        foreach (LightBeam beam in input)
        {
            output[outputDirection] += beam;
        }

        return output;
    }

    protected override void LongPressedUpdate ()
    {
        if (!rotatable)
        {
            base.LongPressedUpdate ();
            return;
        }

        Vector2 displacement = InputManager.Instance.WorldTouchPosition - (Vector2)transform.position;
        float rotation = Mathf.Atan2 (displacement.x, displacement.y);
        outputDirection = Tools.RotToInt (rotation, true);

        SetRotation (outputDirection);
    }

    protected override void OnPressDragStart ()
    {
        //print (outputDirection);
        if (!rotatable)
        {
            base.OnPressDragStart ();
            return;
        }

        transform.localScale = Vector3.one * 1.5f;
        preRotateDirection = outputDirection;
    }

    protected override void OnPressDragEnd ()
    {
        if (!rotatable)
        {
            base.OnPressDragEnd ();
            return;
        }

        transform.localScale = Vector3.one;
        if (outputDirection != preRotateDirection)
            EventManager.Instance.GridObjectMoved ();
    }
}
