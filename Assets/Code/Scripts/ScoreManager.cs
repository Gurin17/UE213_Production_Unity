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
    public float baseScoreIncrement = 1f; // Points ajoutés par intervalle
    public float scoreUpdateInterval = 0.1f; // Intervalle en secondes pour augmenter le score

    [Header("Combo Settings")]
    public float comboDecayRate = 0.1f; // Vitesse de diminution de base du combo progress
    public float comboDecayMultiplierFactor = 0.05f; // Facteur de diminution supplémentaire par niveau de combo
    public float comboIncrement = 0.2f; // Valeur ajoutée au combo progress par collecte
    public float comboResetThreshold = 0.99f; // Valeur à laquelle le combo progress redémarre après une réduction du multiplicateur

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
        // Démarre la coroutine pour augmenter le score
        scoreCoroutine = StartCoroutine(IncreaseScoreOverTime());
    }

    void Update()
    {
        AudioSource carAudioSource = vehicle.GetComponent<AudioSource>();

        if (carAudioSource != null && carAudioSource.clip != null)
        {
            float musicProgress = carAudioSource.time / carAudioSource.clip.length;
            percentText.text = $"{Mathf.RoundToInt(musicProgress * 100)}%";
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
            // Réduit le combo multiplier si le slider est à 0
            comboMultiplier--;
            comboProgress = comboResetThreshold; // Redémarre le slider juste en dessous de 1
        }

        UpdateUI();
    }

    public void collectElement(float progress)
    {
        comboProgress += progress;

        // Si le slider dépasse 1, augmente le combo
        if (comboProgress >= 1f)
        {
            comboMultiplier++;
            comboProgress -= 1f; // Reporte l'excédent
        }

        // Met à jour le slider
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
        // Update score, combo multiplier text, and slider value
        scoreText.text = $"{score}";
        comboMultiplierText.text = $"x{comboMultiplier}";   
        comboSlider.value = comboProgress;
    }

    private IEnumerator IncreaseScoreOverTime()
    {
        while (true)
        {
            // Increase the score based on the comboMultiplier
            score += Mathf.RoundToInt(baseScoreIncrement * comboMultiplier);

            UpdateUI();

            // Wait for the defined interval before the next increment
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