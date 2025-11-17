using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalFase2 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Fase2");
        }
    }
}