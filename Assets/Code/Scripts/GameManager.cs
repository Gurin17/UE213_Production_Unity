using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SongData[] songData;
    public GameObject Collectibles;
    public GameObject Car;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SongData randomSong = songData[Random.Range(0, songData.Length)];

        Car.GetComponent<AudioSource>().clip = randomSong.musicSource;

        Collectibles.GetComponent<BeatAnalyzer>().musicSource = Car.GetComponent<AudioSource>();
        Collectibles.GetComponent<BeatAnalyzer>().songBpm = randomSong.bpm;
        Collectibles.GetComponent<BeatAnalyzer>().firstBeatOffset = 1f;
        Collectibles.GetComponent<CollectibleCreator>().fileName = randomSong.saveTrackName;
    }

    void Start()
    {
        
    }

    [System.Serializable]
    public struct SongData
    {
        public AudioClip musicSource;
        public int bpm;
        public string saveTrackName;
    }
}
