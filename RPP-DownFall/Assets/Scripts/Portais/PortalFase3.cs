using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalFase3 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Fase3");
        }
    }
}