using PathCreation;
using PathCreation.Examples;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
public class CollectibleCreator : MonoBehaviour
{
    public PathCreator pathCreator;
    public GameObject vehicle;
    public GameObject[] prefabs;
    public GameObject[] obstaclePrefabs;
    public float obstacleSpawnChance = 0;

    // Random Path Creator
    public GameObject prefab;
    public Int32 beatsBetweenSpawn;
    public Int32 spawnGroupSize;
    public Int32 beatsBetweenGroup;
    public Int32 beatsBeforeSpawning;

    public SpeedVariation[] speedVariationsList;

    // Saves Manager
    public string fileName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Load();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Save()
    {
        SaveObject saveObject = new SaveObject();

        saveObject.collectibles = new();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject prefab = transform.GetChild(i).gameObject;
            Collectible collectible = prefab.GetComponent<Collectible>();

            if (collectible != null)
            {
                CollectibleData item = new CollectibleData
                {
                    type = collectible.type,
                    beat = collectible.beat,
                    offset = collectible.offset,
                    heightOffset = collectible.heightOffset,
                    scale = collectible.transform.localScale
                };

                saveObject.collectibles.Add(item);
            }
        }

        PathFollower saveVehicle = vehicle.GetComponent<PathFollower>();
        if (saveVehicle != null)
        {
            VehicleData vehicleData = new VehicleData
            {
                speed = saveVehicle.speed,
                widthOffset = saveVehicle.widthOffset,
                heightOffset = saveVehicle.heightOffset,
                endOfPathInstruction = saveVehicle.endOfPathInstruction
            };

            saveObject.vehicleData = vehicleData;
        }

        BeatAnalyzer beatAnalyzer = GetComponent<BeatAnalyzer>();
        if (beatAnalyzer != null )
        {
            BeatData beatData = new BeatData
            {
                audioClip = beatAnalyzer.musicClip,
                audioBpm = beatAnalyzer.songBpm,
                firstBeatOffset = beatAnalyzer.firstBeatOffset
            };

            saveObject.beatData = beatData;
        }

        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(Application.dataPath + "/Resources/" + fileName + ".json", json);

    }

    public void Load()
    {
        TextAsset targetFile = Resources.Load<TextAsset>(fileName);
        if (targetFile)
        {
            string saveString = targetFile.text;

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);

            GeneratePathFromData(saveObject);

        }
    }
    private void GeneratePathFromData(SaveObject saveObject)
    {
        VehicleData vehicleData = saveObject.vehicleData;
        BeatData beatData = saveObject.beatData;

        //Calculate the number of seconds in each beat
        float secPerBeat = 60f / beatData.audioBpm;

        float baseDistance = beatData.firstBeatOffset * getCurrentSpeed(0f, pathCreator.path.length);

        Dictionary<CollectibleType, GameObject> collectibleByType = new();
        foreach (GameObject coll in prefabs)
        {
            Collectible collectibleScript = coll.GetComponent<Collectible>();
            if (collectibleScript != null)
            {
                collectibleByType.Add(collectibleScript.type, coll);
            }
        }

        foreach (GameObject coll in obstaclePrefabs)
        {
            Collectible collectibleScript = coll.GetComponent<Collectible>();
            if (collectibleScript != null)
            {
                collectibleByType.Add(collectibleScript.type, coll);
            }
        }


        foreach (CollectibleData collectibleData in saveObject.collectibles)
        {
            //float distance = baseDistance + secPerBeat * vehicleData.speed * collectibleData.beat;
            
            float distance = baseDistance;

            for (int beat = 0; beat < collectibleData.beat; beat++)
            {
                float currentSpeed = getCurrentSpeed(distance, pathCreator.path.length);
                distance += secPerBeat * currentSpeed;
            }
            
            
            Debug.Log(distance);
            // Spawn the collectible
            Vector3 spawnPosition = new Vector3();
            Quaternion spawnRotation = pathCreator.path.GetRotationAtDistance(distance, vehicleData.endOfPathInstruction) * Quaternion.Euler(0, 0, 90);

            GameObject collectibleBase;
            collectibleByType.TryGetValue(collectibleData.type, out collectibleBase);

            if (collectibleBase != null)
            {
                GameObject collectible = Instantiate(collectibleBase, spawnPosition, spawnRotation);
                collectible.transform.localScale = collectibleData.scale;
                collectible.transform.position = pathCreator.path.GetPointAtDistance(distance, vehicleData.endOfPathInstruction) + (collectible.transform.right * collectibleData.offset) + (collectible.transform.up * collectibleData.heightOffset);
                collectible.transform.parent = transform;

                Collectible collectibleScript = collectible.GetComponent<Collectible>();

                if (collectibleScript != null)
                {
                    collectibleScript.type = collectibleData.type;
                    collectibleScript.heightOffset = collectibleData.heightOffset;
                    collectibleScript.beat = collectibleData.beat;
                    collectibleScript.offset = collectibleData.offset;
                }
            }

        }
    }

    public void GenerateRandomPath()
    {
        PathFollower currentVehicle = vehicle.GetComponent<PathFollower>();
        if (currentVehicle == null)
        {
            return;
        }

        BeatAnalyzer beatAnalyzer = GetComponent<BeatAnalyzer>();
        if (beatAnalyzer == null)
        {
            return;
        }

        Collectible collectible = prefab.GetComponent<Collectible>();
        if (collectible == null)
        {
            return;
        }

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        // Initialize base distance and offsets
        
        //float distance = beatAnalyzer.firstBeatOffset * currentVehicle.speed;
        
        float distance = beatAnalyzer.firstBeatOffset * getCurrentSpeed(0f, pathCreator.path.length);
        
        Int32 currentGroupSize = 0;
        float[] offsets = { -currentVehicle.widthOffset, 0f, currentVehicle.widthOffset };
        // Random offset
        float spawnOffset = offsets[UnityEngine.Random.Range(0, offsets.Length)];

        //Calculate the number of seconds in each beat
        float secPerBeat = 60f / beatAnalyzer.songBpm;
        Int32 totalBeats = (Int32)(beatAnalyzer.musicClip.length / secPerBeat);

        Int32 startingBeat = Mathf.Min(Mathf.Max(beatsBeforeSpawning, 0), totalBeats - 1);
        distance += secPerBeat * getCurrentSpeed(0f, pathCreator.path.length) * startingBeat;

        // Loop through all the beats of the music
        for (var i = startingBeat; i < totalBeats; i++)
        {

            if (distance < pathCreator.path.length)
            {
                // Spawn collectible
                Debug.Log($"Spawning collectible at distance: {distance}");
                GameObject selectedPrefab = getRandomPrefab();

                Vector3 spawnPosition = new Vector3();

                Quaternion spawnRotation = pathCreator.path.GetRotationAtDistance(distance, currentVehicle.endOfPathInstruction) * Quaternion.Euler(0, 0, 90);

                GameObject spawnedPrefab = Instantiate(selectedPrefab, spawnPosition, spawnRotation, selectedPrefab.transform.parent);
                spawnedPrefab.transform.localScale = selectedPrefab.transform.localScale;
                spawnedPrefab.transform.position = pathCreator.path.GetPointAtDistance(distance, currentVehicle.endOfPathInstruction) + (spawnedPrefab.transform.right * spawnOffset) + (spawnedPrefab.transform.up * selectedPrefab.GetComponent<Collectible>().heightOffset);
                spawnedPrefab.transform.parent = transform;

                spawnedPrefab.transform.rotation = spawnRotation * selectedPrefab.transform.rotation;

                Debug.Log($"Spawned : {spawnedPrefab.name} at distance: {distance}");
                Debug.Log($"Spawned : {spawnedPrefab.name} at position: {spawnedPrefab.transform.position}");


                Collectible currentCollectible = spawnedPrefab.GetComponent<Collectible>();
                if (currentCollectible != null)
                {
                    currentCollectible.beat = i;
                    currentCollectible.offset = spawnOffset;
                    currentCollectible.heightOffset = collectible.heightOffset;
                }

                currentGroupSize++;
            }


            Int32 multiplier = 1;

            // End of group
            if (currentGroupSize >= spawnGroupSize)
            {
                // Prepare gap between groups
                i = i + beatsBetweenGroup;
                multiplier = beatsBetweenGroup + 1;
                spawnOffset = offsets[UnityEngine.Random.Range(0, offsets.Length)];

                currentGroupSize = 0;
            }
            // Prepare gap between spawn if group is not finish
            else if (beatsBetweenSpawn > 0)
            {
                i = i + beatsBetweenSpawn;
                multiplier = beatsBetweenSpawn + 1;
            }

            distance += secPerBeat * getCurrentSpeed(distance, pathCreator.path.length) * multiplier;
        }
    }

    public void updateCollectible(Collectible inCollectible)
    {
        PathFollower currentVehicle = vehicle.GetComponent<PathFollower>();
        if (currentVehicle == null)
        {
            return;
        }

        BeatAnalyzer beatAnalyzer = GetComponent<BeatAnalyzer>();
        if (beatAnalyzer == null)
        {
            return;
        }

        //Calculate the number of seconds in each beat
        float secPerBeat = 60f / beatAnalyzer.songBpm;

        float baseDistance = beatAnalyzer.firstBeatOffset * currentVehicle.speed;

        float distance = baseDistance + secPerBeat * currentVehicle.speed * inCollectible.beat;

       
        // Spawn the collectible
        Quaternion spawnRotation = pathCreator.path.GetRotationAtDistance(distance, currentVehicle.endOfPathInstruction) * Quaternion.Euler(0, 0, 90);


        GameObject collectible = inCollectible.gameObject;
        collectible.transform.localScale = inCollectible.transform.localScale;
        collectible.transform.rotation = spawnRotation;
        collectible.transform.position = pathCreator.path.GetPointAtDistance(distance, currentVehicle.endOfPathInstruction) + (collectible.transform.right * inCollectible.offset) + (collectible.transform.up * inCollectible.heightOffset);
        collectible.transform.parent = transform;
    }

    private float getCurrentSpeed(float distance, float pathLength)
    {
        if (speedVariationsList.Length == 0) 
            return vehicle.GetComponent<PathFollower>().speed;

        float progressPercentage = (distance / pathLength) * 100f;

        float maxSpeed = speedVariationsList[0].speed;

        foreach (SpeedVariation variation in speedVariationsList)
        {
            if (progressPercentage >= variation.percentage)
            {
                maxSpeed = variation.speed;
            }
            else
            {
                break;
            }
        }

        return maxSpeed;
    }

    private GameObject getRandomPrefab()
    {
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        if ((randomValue < obstacleSpawnChance) && (obstaclePrefabs.Length > 0))
        {
            return obstaclePrefabs[UnityEngine.Random.Range(0, obstaclePrefabs.Length)];
        }
        else
        {
            return prefabs[UnityEngine.Random.Range(0, prefabs.Length)];
        }
    }    

    [System.Serializable]
    public struct SaveObject
    {
        public VehicleData vehicleData;
        public BeatData beatData;
        public List<CollectibleData> collectibles;
    }

    [System.Serializable]
    public struct CollectibleData
    {
        public CollectibleType type;
        public Int32 beat;
        public float offset;
        public float heightOffset;
        public Vector3 scale;
    }

    [System.Serializable]
    public struct BeatData
    {
        public AudioClip audioClip;
        public float audioBpm;
        public float firstBeatOffset;
    }

    [System.Serializable]
    public struct VehicleData
    {
        public float speed;
        // Offset of the vehicle when switching lane
        public float widthOffset;
        // Height offset of spawing vehicle
        public float heightOffset;
        public EndOfPathInstruction endOfPathInstruction;
    }

    [System.Serializable]

    public struct SpeedVariation
    {
        public int percentage;
        public float speed;
    }
}