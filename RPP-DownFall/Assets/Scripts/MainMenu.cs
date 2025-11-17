using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Cenas")]
    [SerializeField] private string firstGameplayScene = "Testes";

    [Header("Painéis")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Configurações do Jogo")]
    [SerializeField] private GameSettings gameSettings;

    public void NovoJogo()
    {
        SaveSystem.DeleteSave();
        ApplyInitialSettings();
        SceneManager.LoadScene(firstGameplayScene);
    }

    private void ApplyInitialSettings()
    {
        // -------------------------------
        // ➤ POÇÕES INICIAIS (ZERO)
        // -------------------------------
        HealthManager.potionCount = 0;
        HealthManager.maxPotions = 9; 

        // -------------------------------
        // ➤ MUNIÇÃO INICIAL (ZERO)
        // -------------------------------
        AmmoManager.pistolBullets = 0;
        AmmoManager.pistolMagazine = 0;

        AmmoManager.shotgunBullets = 0;
        AmmoManager.shotgunMagazine = 0;

        // -------------------------------
        // ➤ ARMAS INICIAIS (bloqueadas)
        // -------------------------------
        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.pistolUnlocked = false;
            WeaponManager.Instance.shotgunUnlocked = false;
            WeaponManager.Instance.SetWeapon(WeaponType.None);
        }
        
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
