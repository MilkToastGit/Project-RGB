using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamRenderer : Singleton<BeamRenderer>
{
    private List<LightBeam> buffer = new List<LightBeam> ();

    [SerializeField]
    private GameObject rendererPrefab;
    [SerializeField]
    private int poolSize;

    private LineRenderer[] renderers;
    private int activeRenderer;

    private void Awake ()
    {
        AttachRenderers (poolSize);
    }

    public void BufferBeam (LightBeam beam)
    {
        if (beam.Origin == null || beam.Termination == null)
            throw new System.Exception ("Uncast beam cannot be buffered");

        buffer.Add (beam);
        beam.OnBeamCancelled += OnBeamCancelled;
    }

    public void Render ()
    {
        UnrenderBeams ();

        foreach (LightBeam beam in buffer)
        {
            print ($"Rendering Beam of Colour {beam.Colour}, starting at {beam.Origin}");
            LineRenderer line = renderers[activeRenderer];
            line.enabled = true;
            line.SetPositions (beam.Positions);
            line.startColor = beam.Colour;
            line.endColor = beam.Colour;

            if (++activeRenderer >= renderers.Length)
                throw new System.Exception ("All line renderers used up, try increasing pool size");
        }

        buffer.Clear ();
    }

    private void AttachRenderers (int amount)
    {
        renderers = new LineRenderer[amount];
        for (int i = 0; i < amount; i++)
            renderers[i] = Instantiate (rendererPrefab, transform).GetComponent<LineRenderer> ();
    }

    private void UnrenderBeams ()
    {
        for (; activeRenderer > 0; activeRenderer--)
        {
            print ($"Unrendering line {activeRenderer}");
            renderers[activeRenderer].enabled = false;
        }
    }

    private void OnBeamCancelled (LightBeam beam)
    {
        buffer.Remove (beam);
        beam.OnBeamCancelled -= OnBeamCancelled;
    }
}
