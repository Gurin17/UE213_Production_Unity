using PathCreation.Examples;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathFollower))]
public class PathFollowerEditor : Editor
{
   public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathFollower myScript = (PathFollower)target;

        if (GUILayout.Button("Compute Time Travel"))
        {
            myScript.ComputeTravelTime();
        }

        GUI.enabled = false;
        myScript.timeToTravel = EditorGUILayout.FloatField("Time to travel (sec)", myScript.timeToTravel);
    }
}
