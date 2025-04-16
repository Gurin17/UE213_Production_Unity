using PathCreation.Examples;
using UnityEngine;

public class InputInteractions : MonoBehaviour
{
    public GameObject vehicle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(100, 100, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Left()
    {
        PathFollower pathFollower = vehicle.GetComponent<PathFollower>();
        CarAnimation carAnimation = vehicle.GetComponentInChildren<CarAnimation>();

        if (vehicle.GetComponent<VehiclemManager>().isControlsReversed)
        {
            // Si les contrôles sont inversés, effectuez l'action de droite
            float tempOffset = pathFollower.offset + pathFollower.widthOffset;
            pathFollower.offset = Mathf.Min(tempOffset, pathFollower.widthOffset);
            carAnimation.LeanRight();
        }
        else
        {
            // Contrôle normal
            float tempOffset = pathFollower.offset - pathFollower.widthOffset;
            pathFollower.offset = Mathf.Max(tempOffset, -pathFollower.widthOffset);
            carAnimation.LeanLeft();
        }
    }

    public void Right()
    {
        PathFollower pathFollower = vehicle.GetComponent<PathFollower>();
        CarAnimation carAnimation = vehicle.GetComponentInChildren<CarAnimation>();

        if (vehicle.GetComponent<VehiclemManager>().isControlsReversed)
        {
            // Si les contrôles sont inversés, effectuez l'action de gauche
            float tempOffset = pathFollower.offset - pathFollower.widthOffset;
            pathFollower.offset = Mathf.Max(tempOffset, -pathFollower.widthOffset);
            carAnimation.LeanLeft();
        }
        else
        {
            // Contrôle normal
            float tempOffset = pathFollower.offset + pathFollower.widthOffset;
            pathFollower.offset = Mathf.Min(tempOffset, pathFollower.widthOffset);
            carAnimation.LeanRight();
        }
    }
}
