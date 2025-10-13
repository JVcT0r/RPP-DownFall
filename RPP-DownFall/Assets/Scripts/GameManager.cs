using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.SceneManagement.SceneManager;


public class GameManager : MonoBehaviour
{
    public bool paused = false;   
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        //else Destroy(this.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }


    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void Pause()
    {
        if (!paused)
        {
            Time.timeScale = 0f;
            LoadScene("UI_PauseScreen", LoadSceneMode.Additive);
            paused = true;
        }
    }

    public void UnPause()
    {
        Time.timeScale = 1f;
        paused = false;
        UnloadSceneAsync("UI_PauseScreen");
    }
    


    private void Start()
    {
        LoadScene("UI", LoadSceneMode.Additive);
    }
}
