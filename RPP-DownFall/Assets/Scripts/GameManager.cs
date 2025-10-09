using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.SceneManagement.SceneManager;


public class GameManager : MonoBehaviour
{
    
    public static GameManager instance;
    public Player player;

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
        Time.timeScale = 0f;
        LoadScene("UI_PauseScreen", LoadSceneMode.Additive);
        player.paused = true;
    }

    public void UnPause()
    {
        Time.timeScale = 1f;
        UnloadSceneAsync("UI_PauseScreen");
        player.paused = false;
    }
    


    private void Start()
    {
        LoadScene("UI", LoadSceneMode.Additive);
    }
}
