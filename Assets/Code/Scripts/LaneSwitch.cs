using UnityEngine;

public class LaneSwitch : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Material leftLaneMaterial;
    public Material rightLaneMaterial;
    public Material centerLaneMaterial;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setMaterialbyLane(int lane)
    {
        switch (lane)
        {
            case 0:
                GetComponent<Renderer>().material = rightLaneMaterial;
                break;
            case 1:
                GetComponent<Renderer>().material = centerLaneMaterial;
                break;
            case 2:
                GetComponent<Renderer>().material = leftLaneMaterial;
                break;
        }
    }
}
