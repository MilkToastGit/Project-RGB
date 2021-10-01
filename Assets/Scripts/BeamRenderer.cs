using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamRenderer : Singleton<BeamRenderer>
{
    private List<LightBeam> buffer = new List<LightBeam> ();

    [SerializeField]
    private GameObject rendererPrefab;
    private LineRenderer[] renderers;
    private int activeRenderer;

    private void Awake ()
    {
        AttachRenderers (20);
    }

    public void BufferBeam (LightBeam beam)
    {
        if (beam.Origin == null || beam.Termination == null)
            throw new System.Exception ("Uncast beam cannot be buffered");

        buffer.Add (beam);
    }

    private void AttachRenderers (int amount)
    {
        renderers = new LineRenderer[amount];
        for (int i = 0; i < amount; i++)
            renderers[i] = Instantiate (rendererPrefab, transform).GetComponent<LineRenderer> ();
    }

    private void RenderBeams ()
    {
        UnrenderBeams ();

        foreach (LightBeam beam in buffer)
        {
            LineRenderer line = renderers[activeRenderer++];
            line.enabled = true;
            line.SetPositions (beam.Positions);
            line.startColor = beam.Color;
            line.endColor = beam.Color;

            buffer.Remove (beam);

            if (++activeRenderer > renderers.Length - 1)
                throw new System.Exception ("All line renderers used up, try increasing pool size");
        }

        buffer.Clear ();
    }

    private void UnrenderBeams ()
    {
        for (; activeRenderer >= 0; activeRenderer--)
            renderers[activeRenderer].enabled = false;
    }
}
