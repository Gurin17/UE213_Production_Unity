using UnityEngine;

public class CarAnimation : MonoBehaviour
{
    public static CarAnimation instance;

    public AnimationCurve curve;               // Must go 0 → 1 → 0
    public float duration = 0.5f;              // Total time for the lean animation
    public float angle = 30f;                  // Angle of the lean
    public Vector3 rotationAxis = Vector3.forward; // Lean direction (e.g., Vector3.forward for Z)

    private Quaternion originalRotation;
    private float leanAmount = 0f;
    private float timer = 0f;
    private bool isLeaning = false;

    [Header("Hit Animation")]
    public float hitAnimationDuration = 0.5f;
    public Material hitMaterial;
    public GameObject[] elementsToChangeColor; 

    private void Awake()
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
        originalRotation = transform.localRotation;
    }

    public void Lean(float amount)
    {
        if (isLeaning) return; // Prevent multiple leans at once

        leanAmount = amount;           // Angle multiplier
        timer = 0f;
        isLeaning = true;
    }

    void Update()
    {
        if (!isLeaning) return;

        timer += Time.deltaTime;
        float t = timer / duration;


        if (t >= 1f)
        {
            t = 1f;
            isLeaning = false;
        }

        float curveValue = curve.Evaluate(t);
        float angle = leanAmount * curveValue;
        transform.localRotation = originalRotation * Quaternion.AngleAxis(angle, rotationAxis);
    }

    // Optional helpers
    public void LeanRight() => Lean(-angle);
    public void LeanLeft() => Lean(angle);

    public void playHitAnimation()
    {
        // offset the car
        

        foreach (GameObject element in elementsToChangeColor)
        {
            Renderer renderer = element.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material[] originalMaterials = renderer.materials;
                Material[] updatedMaterials = new Material[originalMaterials.Length + 1];

                updatedMaterials[0] = hitMaterial;

                for (int i = 0; i < originalMaterials.Length; i++)
                {
                    updatedMaterials[i + 1] = originalMaterials[i];
                }

                renderer.materials = updatedMaterials;
            }
        }

        Invoke("ResetHitAnimation", hitAnimationDuration);
    }

    private void ResetHitAnimation()
    {
        foreach (GameObject element in elementsToChangeColor)
        {
            Renderer renderer = element.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material[] originalMaterials = renderer.materials;

                if (originalMaterials.Length > 1)
                {
                    Material[] restoredMaterials = new Material[originalMaterials.Length - 1];
                    for (int i = 1; i < originalMaterials.Length; i++)
                    {
                        restoredMaterials[i - 1] = originalMaterials[i];
                    }

                    renderer.materials = restoredMaterials;
                }
            }
        }
    }
}

