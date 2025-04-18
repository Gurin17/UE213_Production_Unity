using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TMP_Text scoreText;
    public TMP_Text comboMultiplierText;
    public TMP_Text percentText;
    public Slider comboSlider;

    [Header("Score Settings")]
    public float baseScoreIncrement = 1f;
    public float scoreUpdateInterval = 0.1f;

    [Header("Game End Settings")]
    public GameObject UIManager;
    public AudioClip finishSound;

    [Header("Combo Settings")]
    public float comboDecayRate = 0.1f;
    public float comboDecayMultiplierFactor = 0.05f;
    public float comboIncrement = 0.2f;
    public float comboResetThreshold = 0.99f;

    [Header("Car Settings")]
    public GameObject vehicle;

    private int score;
    private int comboMultiplier = 1;
    private float comboProgress = 0f;
    private Coroutine scoreCoroutine;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI();
        scoreCoroutine = StartCoroutine(IncreaseScoreOverTime());
    }

    void Update()
    {
        AudioSource carAudioSource = vehicle.GetComponent<AudioSource>();

        if (carAudioSource != null && carAudioSource.clip != null)
        {
            float musicProgress = carAudioSource.time / carAudioSource.clip.length;
            percentText.text = $"{Mathf.RoundToInt(musicProgress * 100)}%";

            if (musicProgress >= 1f)
            {
                carAudioSource.Stop();
                vehicle.GetComponent<AudioSource>().PlayOneShot(finishSound);
                UIManager.GetComponent<EndMenu>().showFinalScore(score);
            }
        }

        if (comboProgress > 0f)
        {
            comboProgress -= Time.deltaTime * (comboDecayRate + (comboMultiplier - 1) * comboDecayMultiplierFactor);
            if (comboProgress < 0f)
            {
                comboProgress = 0f;
            }
        }
        else if (comboMultiplier > 1)
        {
            comboMultiplier--;
            comboProgress = comboResetThreshold;
        }

        UpdateUI();
    }

    public void collectElement(float progress)
    {
        comboProgress += progress;

        if (comboProgress >= 0.99f)
        {
            comboMultiplier++;
            comboProgress -= 1f;
        }

        comboSlider.value = comboProgress;

        UpdateUI();
    }

    public void looseCombo()
    {
        comboProgress = 0f;
        comboMultiplier = 1;

        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = $"{score}";
        comboMultiplierText.text = $"x{comboMultiplier}";   
        comboSlider.value = comboProgress;
    }

    private IEnumerator IncreaseScoreOverTime()
    {
        while (true)
        {
            score += Mathf.RoundToInt(baseScoreIncrement * comboMultiplier);

            UpdateUI();

            yield return new WaitForSeconds(scoreUpdateInterval);
        }
    }

    void OnDisable()
    {
        if (scoreCoroutine != null)
        {
            StopCoroutine(scoreCoroutine);
        }
    }
}