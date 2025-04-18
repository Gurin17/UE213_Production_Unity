using UnityEngine;

public class CollectibleMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float rotationSpeed = 30f;

    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float height = 1f;
    public float speed = 1f;

    private float timer;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
        timer = Random.Range(0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        timer += Time.deltaTime * speed;
        float t = movementCurve.Evaluate(timer % 1f);
        float yOffset = t * height;
        transform.localPosition = startPos + new Vector3(0, yOffset, 0);
    }
}
