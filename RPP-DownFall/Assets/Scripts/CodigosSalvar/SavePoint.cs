using UnityEngine;
using TMPro;

public class SavePoint : MonoBehaviour
{
    [Header("Referências")]
    public GameObject saveUIPrompt; 
    public Vector3 textOffset = new Vector3(0, 1.5f, 0); 

    private bool playerNearby;
    private Transform playerTransform;

    private void Start()
    {
        if (saveUIPrompt != null)
        {
            saveUIPrompt.SetActive(false);
            saveUIPrompt.transform.localPosition = textOffset;
        }
    }

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            var player = FindAnyObjectByType<Player>();
            if (player != null)
            {
                SaveSystem.SaveGame(player);
                Debug.Log("[SavePoint] Jogo salvo no ponto!");
            }
        }

        
        if (saveUIPrompt != null && Camera.main != null)
        {
            saveUIPrompt.transform.rotation = Camera.main.transform.rotation;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = true;
            playerTransform = collision.transform;
            if (saveUIPrompt != null)
                saveUIPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = false;
            playerTransform = null;
            if (saveUIPrompt != null)
                saveUIPrompt.SetActive(false);
        }
    }
}