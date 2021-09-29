[System.Serializable]
public class LightBeam
{
    public static LightBeam Red => new LightBeam (true, false, false);
    public static LightBeam Green => new LightBeam (false, true, false);
    public static LightBeam Blue => new LightBeam (false, false, true);

    [UnityEngine.SerializeField]
    private bool r, g, b;

    public UnityEngine.Color Color => new UnityEngine.Color ((r?1:0), (g?1:0), (b?1:0));
    public bool R => r;
    public bool G => g;
    public bool B => b;

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

    public LightBeam[] GetComponents ()
    {
        LightBeam[] components = new LightBeam[ComponentCount];

        int i = 0;
        if (r) components[i++] = Red;
        if (g) components[i++] = Green;
        if (b) components[i++] = Blue;

        return components;
    }
}
