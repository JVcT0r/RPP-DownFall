using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GlobalUIManager : MonoBehaviour
{
    [Header("Prefab do painel de opções")]
    public GameObject optionsPanelPrefab;

    private static GlobalUIManager instance;
    private GameObject currentOptionsPanel;

    private void Awake()
    {
        
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (optionsPanelPrefab == null)
        {
            Debug.LogWarning("[GlobalUIManager] Prefab do painel de opções não atribuído!");
            return;
        }

        
        if (currentOptionsPanel == null)
        {
            
            Canvas canvasDaCena = FindObjectOfType<Canvas>();
            if (canvasDaCena == null)
            {
                Debug.LogWarning("[GlobalUIManager] Nenhum Canvas encontrado na cena " + scene.name + ". Criando um novo temporário.");
                GameObject novoCanvas = new GameObject("CanvasGlobalUI", typeof(Canvas), typeof(CanvasScaler), typeof(UnityEngine.UI.GraphicRaycaster));
                canvasDaCena = novoCanvas.GetComponent<Canvas>();
                canvasDaCena.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            
            currentOptionsPanel = Instantiate(optionsPanelPrefab, canvasDaCena.transform);
            currentOptionsPanel.name = "OptionsPanelGlobal";
            currentOptionsPanel.SetActive(false);

            Debug.Log("[GlobalUIManager] Painel de opções instanciado na cena: " + scene.name);
        }
        else
        {
            
            Canvas novoCanvas = FindObjectOfType<Canvas>();
            if (novoCanvas != null && currentOptionsPanel.transform.parent != novoCanvas.transform)
            {
                currentOptionsPanel.transform.SetParent(novoCanvas.transform, false);
            }
        }
    }
}
