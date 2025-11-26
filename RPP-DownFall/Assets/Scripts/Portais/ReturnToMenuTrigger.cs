using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuTrigger : MonoBehaviour
{
    [Header("Nome da cena do menu principal")]
    public string menuSceneName = "Menu"; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(menuSceneName);
        }
    }
}