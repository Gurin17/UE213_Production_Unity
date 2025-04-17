using PathCreation;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CollectibleCreator))]
public class CollectibleCreatorEditor : Editor
{
    SerializedProperty prefabs;
    SerializedProperty obstaclePrefabs;
    SerializedProperty speedChanges;


    private void OnEnable()
    {
        prefabs = serializedObject.FindProperty("prefabs");
        speedChanges = serializedObject.FindProperty("speedVariationsList");
        obstaclePrefabs = serializedObject.FindProperty("obstaclePrefabs");
    }

    public override void OnInspectorGUI()
    {
        CollectibleCreator myScript = (CollectibleCreator)target;
        myScript.pathCreator = (PathCreator)EditorGUILayout.ObjectField("Path", myScript.pathCreator, typeof(PathCreator), true);
        myScript.vehicle = (GameObject)EditorGUILayout.ObjectField("Vehicle", myScript.vehicle, typeof(GameObject), true);

        serializedObject.Update();
        EditorGUILayout.PropertyField(prefabs, true);
        serializedObject.ApplyModifiedProperties();

        myScript.obstacleSpawnChance = EditorGUILayout.Slider("Obstacle spawn chance", myScript.obstacleSpawnChance, 0.0f, 100.0f);
        
        serializedObject.Update();
        EditorGUILayout.PropertyField(obstaclePrefabs, true);
        serializedObject.ApplyModifiedProperties();
        
        GUILayout.Space(20);

        EditorGUILayout.LabelField("Random Path Generator", EditorStyles.boldLabel);

        myScript.prefab = (GameObject)EditorGUILayout.ObjectField("Collectible", myScript.prefab, typeof(GameObject), true);
        myScript.beatsBeforeSpawning = EditorGUILayout.IntField("Starting spawn beat", myScript.beatsBeforeSpawning);
        myScript.beatsBetweenSpawn = EditorGUILayout.IntField("Beat gap between spawn", myScript.beatsBetweenSpawn);
        myScript.spawnGroupSize = EditorGUILayout.IntField("Spawn group size", myScript.spawnGroupSize);
        myScript.beatsBetweenGroup = EditorGUILayout.IntField("Beat gap between group", myScript.beatsBetweenGroup);

        GUILayout.Space(10);
        
        serializedObject.Update();
        EditorGUILayout.PropertyField(speedChanges, true);
        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(5);


        if (GUILayout.Button("Generate Random Path"))
        {
            myScript.GenerateRandomPath();
        }

        GUILayout.Space(20);

        EditorGUILayout.LabelField("Saves Manager", EditorStyles.boldLabel);
        myScript.fileName = EditorGUILayout.TextField("File Name", myScript.fileName);

        if (GUILayout.Button("Save collectibles"))
        {
            myScript.Save();
        }

        if (GUILayout.Button("Load collectibles"))
        {
            myScript.Load();
        }

    }
}
