using System;
using System.Reflection;
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

    public static bool isBetween (this Vector2Int point, Vector2Int min, Vector2Int max)
    {
        return point.x.isBetween (min.x, max.x) && point.y.isBetween (min.y, max.y);
    }

    public static int Wrap (int n, int m)
    {
        return n >= 0 ? n % m : (n % m + m) % m;
    }
    public static Vector3 ToVector3 (this Vector2 vector2, float z = 0) => new Vector3 (vector2.x, vector2.y, z);

    public static Quaternion IntToRot (int direction)
    {
        float z = direction * -45;
        return Quaternion.Euler(0, 0, z);
    }

    public static int RotToInt (float degrees, bool convertFromRad = false)
    {
        if (convertFromRad) degrees *= Mathf.Rad2Deg;
        return Wrap (Mathf.RoundToInt (degrees / 45), 8);
    }

    public static float GetSqrDist (Vector2 point1, Vector2 point2)
    {
        Vector2 displacement = point1 - point2;
        return Mathf.Pow (displacement.x, 2) + Mathf.Pow (displacement.y, 2);
    }

    public static bool SamePoint (this Vector2Int point, int x, int y) => point.x == x && point.y == y;
    //public static T GetAt<T> (this T[,] multiArray, Vector2Int index) => multiArray[index.x, index.y];
}
