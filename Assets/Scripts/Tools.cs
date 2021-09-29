using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static bool isBetween (this int value, int value1, int value2)
    {
        return value >= Mathf.Min (value1, value2) && value <= Mathf.Max (value1, value2);
    }

    public static bool isBetween (this float value, float value1, float value2)
    {
        return value >= Mathf.Min (value1, value2) && value <= Mathf.Max (value1, value2);
    }

    public static int Wrap (int n, int m)
    {
        return n >= 0 ? n % m : (n % m + m) % m;
    }
}
