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
        HealthManager.potionCount = gameSettings.startPotions;
        HealthManager.maxPotions = gameSettings.maxPotions;

        
        AmmoManager.pistolBullets = gameSettings.pistolStartBullets;
        AmmoManager.pistolMagazine = gameSettings.pistolStartMagazine;

        
        AmmoManager.shotgunBullets = gameSettings.shotgunStartBullets;
        AmmoManager.shotgunMagazine = gameSettings.shotgunStartMagazine;

        // Futuro: desbloquear armas
        // WeaponManager.pistolUnlocked   = gameSettings.pistolUnlocked;
        // WeaponManager.shotgunUnlocked  = gameSettings.shotgunUnlocked;
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
