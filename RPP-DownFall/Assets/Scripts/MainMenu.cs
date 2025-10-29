using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Cena inicial do jogo")]
    [SerializeField] private string firstGameplayScene = "Testes";

    public void NovoJogo()
    {
        SceneManager.LoadScene(firstGameplayScene);
    }

    public void CarregarJogo()
    {
        string scene = SaveSystem.GetSavedScene();
        if (!string.IsNullOrEmpty(scene))
        {
            SceneManager.LoadScene(scene);
        }
        else
        {
            Debug.Log("Nenhum jogo salvo encontrado!");
        }
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