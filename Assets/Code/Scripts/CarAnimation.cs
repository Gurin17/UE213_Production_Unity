using UnityEngine;

public class CarAnimation : MonoBehaviour
{
    public AnimationCurve curve;               // Must go 0 → 1 → 0
    public float duration = 0.5f;              // Total time for the lean animation
    public float angle = 30f;                  // Angle of the lean
    public Vector3 rotationAxis = Vector3.forward; // Lean direction (e.g., Vector3.forward for Z)

    private Quaternion originalRotation;
    private float leanAmount = 0f;
    private float timer = 0f;
    private bool isLeaning = false;

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
        Debug.Log(isLeaning);

        if (!isLeaning) return;

        timer += Time.deltaTime;
        float t = timer / duration;


        if (t >= 1f)
        {
            t = 1f;
            isLeaning = false;
        }

        float curveValue = curve.Evaluate(t);
        float angle = leanAmount * curveValue;             // Final angle
        transform.localRotation = originalRotation * Quaternion.AngleAxis(angle, rotationAxis);
    }

    // Optional helpers
    public void LeanRight() => Lean(-angle);
    public void LeanLeft() => Lean(angle);
}
