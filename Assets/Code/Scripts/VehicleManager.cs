using UnityEngine;

public class VehiclemManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isControlsReversed = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReverseControls()
    {
        isControlsReversed = !isControlsReversed;
    }
}
