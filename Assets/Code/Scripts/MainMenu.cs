using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void EndGame()
    {
        Application.Quit();
    }
    void Start()
    {
               
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
