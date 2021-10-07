using System.Collections.Generic;

[System.Serializable]
public class Splitter : GridObject.MultiInput
{
    public override GridObjectType Type => GridObjectType.Splitter;

    protected override BeamRadial GenerateOutputBeams (BeamRadial input)
    {
        BeamRadial output = new BeamRadial ();

        foreach (LightBeam beam in input)
        {
            //print ($"Splitting beam with {beam.ComponentCount} components");
            LightBeam[] components = beam.GetComponents ();
            if (components.Length == 1)
            {
                components = new LightBeam[] { components[0], new LightBeam (components[0]) };
                //print ($"Bifurcating beam of colour {beam.Colour}");
            }

            if (components.Length == 2)
            {
                //print ($"Splitting beam of colour {beam.Colour} into {components[0].Colour} and {components[1].Colour}");
                output[beam.Direction - 1] += components[0];
                output[beam.Direction + 1] += components[1];
            }
            else if (components.Length == 3)
            {
                //print ($"Splitting beam of colour {beam.Colour} into {components[0].Colour}, {components[1].Colour} and {components[2].Colour}");
                for (int i = 0; i < 3; i++)
                {
                    output[beam.Direction + i - 1] += components[i];
                    //print ($"Output direction: {beam.Direction + i - 1}");
                }
            }
        }

        return output;
    }
}
