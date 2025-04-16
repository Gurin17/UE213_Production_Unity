using PathCreation.Examples;
using UnityEngine;

public class Obstacle_Reverse : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject vehicle;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            VehiclemManager vehicleManager = vehicle.GetComponent<VehiclemManager>();

            if (vehicleManager == null) return;
            
            vehicleManager.ReverseControls();
            
        }
    }
}
