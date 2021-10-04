using UnityEngine;

public class Timer
{
    private float startTime;

    public float Elapsed => Time.time - startTime;

    public Timer (float startTime) => this.startTime = startTime;
    public void Reset () => startTime = Time.time;

    public static implicit operator float (Timer timer) => timer.Elapsed;

}
