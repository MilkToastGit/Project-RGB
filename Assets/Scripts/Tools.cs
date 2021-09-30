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

    public static int Wrap (int n, int m)
    {
        return n >= 0 ? n % m : (n % m + m) % m;
    }

    public static T Copy<T> (this Component comp, T other) where T : Component
    {
        Type type = comp.GetType ();
        if (type != other.GetType ()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties (flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue (comp, pinfo.GetValue (other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields (flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue (comp, finfo.GetValue (other));
        }
        return comp as T;
    }

    public static T AddComponent<T> (this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T> ().Copy (toAdd);
    }

    public static Vector3 ToVector3 (this Vector2 vector2, float z = 0) => new Vector3 (vector2.x, vector2.y, z);

    //public static T GetAt<T> (this T[,] multiArray, Vector2Int index) => multiArray[index.x, index.y];
}
