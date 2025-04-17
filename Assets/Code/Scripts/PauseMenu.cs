using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseGame()
    {

        AudioSource[] audioSources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Pause();
        }

        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        AudioSource[] audioSources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Play();
        }

        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void goToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        # if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        # else
            Application.Quit();
        # endif
    }


}
