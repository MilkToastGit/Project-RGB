using UnityEngine;

[System.Serializable]
public class LightBeam
{
    public static LightBeam Red => new LightBeam (true, false, false);
    public static LightBeam Green => new LightBeam (false, true, false);
    public static LightBeam Blue => new LightBeam (false, false, true);
    public static LightBeam Cyan => new LightBeam (false, true, true);
    public static LightBeam Magenta => new LightBeam (true, false, true);
    public static LightBeam Yellow => new LightBeam (true, true, false);

    [SerializeField]
    private bool r, g, b;
    private Vector2Int origin, termination;
    private int direction;

    public Color Color => new Color ((r?1:0), (g?1:0), (b?1:0));
    public bool R => r;
    public bool G => g;
    public bool B => b;
    public Vector2Int Origin => origin;
    public Vector2Int Termination => termination;
    public int Direction => direction;
    public Vector3[] Positions => new Vector3[] {
        IsoGrid.Instance.GridToWorld (origin).ToVector3 (), 
        IsoGrid.Instance.GridToWorld (termination).ToVector3 () };

    public int ComponentCount => (r?1:0) + (g?1:0) + (b?1:0);

    public LightBeam (bool r, bool g, bool b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }

    public LightBeam (int r, int g, int b)
    {
        this.r = r > 0;
        this.g = g > 0;
        this.b = b > 0;
    }

    public LightBeam (LightBeam beam)
    {
        r = beam.r;
        g = beam.g;
        b = beam.b;

        origin = beam.origin;
        termination = beam.termination;
        direction = beam.direction;
    }

    public LightBeam Cast (Vector2Int origin, int direction)
    {
        this.direction = direction;
        this.origin = origin;
        termination = IsoGrid.Instance.CastBeam (this);
        BeamRenderer.Instance.BufferBeam (this);

        return this;
    }

    public LightBeam[] GetComponents ()
    {
        LightBeam[] components = new LightBeam[ComponentCount];

        int i = 0;
        if (r) components[i++] = Red;
        if (g) components[i++] = Green;
        if (b) components[i++] = Blue;

        return components;
    }

    public static LightBeam operator + (LightBeam beam1, LightBeam beam2)
    {
        if (beam1 == null) return beam2;
        if (beam2 == null) return beam1;

        if (beam2.r) beam1.r = true;
        if (beam2.g) beam1.g = true;
        if (beam2.b) beam1.b = true;
        return beam1;
    }
}
