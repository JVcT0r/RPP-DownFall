using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private AudioSource audioSource;
    public bool paused;
    public GameObject pauseScreen;

   
    public GameObject pausePanel;
    public GameObject optionsPanel;

    [Header("Audios")] 
    public AudioClip InitialOneShotMelody;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        audioSource.PlayOneShot(InitialOneShotMelody, 0.5f);
        if (!SceneManager.GetSceneByName("UI").isLoaded)
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Pause();
    }

    public void RestartGame()
    {
        GlobalRunData.Instance.LoadCheckpointState();

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void Pause()
    {
        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
        pauseScreen.SetActive(paused);

        
        if (paused && pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    public bool GetPaused()
    {
        return paused;
    }

   
    public void ToggleOptions(bool open)
    {
        if (optionsPanel == null || pausePanel == null) return;

        if (open)
        {
            optionsPanel.SetActive(true);
            pausePanel.SetActive(false);
        }
        else
        {
            optionsPanel.SetActive(false);
            pausePanel.SetActive(true);
        }
    }
}
