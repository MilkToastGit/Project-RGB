//using UnityEngine;
//using UnityEditor;

//[CanEditMultipleObjects]
//[CustomEditor (typeof (GridObject), true)]
//public class GridObjectEditor : Editor
//{
//    GridObject obj;

//    private void OnEnable ()
//    {
//        obj = target as GridObject;
//    }

//    public override void OnInspectorGUI ()
//    {
//        base.OnInspectorGUI ();

//        if (obj.transform.hasChanged)
//        {
//            obj.SnapToGrid ();
//            obj.transform.hasChanged = false;
//        }
//    }
//}