using UnityEngine;
using UnityEngine.SceneManagement;

public class PortaSaida : MonoBehaviour
{
    public string proximaCena = "Fase2";
    public AudioClip somAbrir;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnInteracted()
    {
        Player player = FindAnyObjectByType<Player>();

        if (player == null) return;

        if (player.temChave)
        {

            if (audioSource != null && somAbrir != null)
                audioSource.PlayOneShot(somAbrir);

            SceneManager.LoadScene(proximaCena);
        }
        
    }
}