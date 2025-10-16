using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.SceneManagement.SceneManager;


public class GameManager : MonoBehaviour
{
    public bool paused = false;
    public Canvas PauseScreen;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
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
            PauseScreen.gameObject.SetActive(true);
            paused = true;
        }
    }

    public void UnPause()
    {
        Time.timeScale = 1f;
        PauseScreen.gameObject.SetActive(false);
        paused = false;
    }
    


    private void Start()
    {
        LoadScene("UI", LoadSceneMode.Additive);
    }
}
