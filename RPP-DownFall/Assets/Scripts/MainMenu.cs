using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Cenas")]
    [SerializeField] private string firstGameplayScene = "Testes";

    [Header("Painéis")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    public void NovoJogo()
    {
        SceneManager.LoadScene(firstGameplayScene);
    }

    public void CarregarJogo()
    {
        string scene = SaveSystem.GetSavedScene();
        if (!string.IsNullOrEmpty(scene))
            SceneManager.LoadScene(scene);
        else
            Debug.Log("Nenhum jogo salvo encontrado!");
    }

    public void AbrirOpcoes()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void FecharOpcoes()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void Sair()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}